#version 450 core

layout (location = 0) out vec4 outColor;
layout (location = 1) out vec4 brightColor;

//out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;
uniform sampler2D gNoLight;
uniform sampler2D gShadow;

struct Light 
{
    vec3 Position;
    vec3 Color;
    
    float Linear;
    float Quadratic;
    float Radius;
};

struct DirectionalLight
{
    vec3 Direction;
    vec3 Color;

    float Diffuse;
    float Specular;
};

const int MAX_LIGHTS = 32;
uniform Light lights[MAX_LIGHTS];
uniform DirectionalLight directLight;
uniform vec3 viewPos;
uniform int activeLights;
uniform mat4 m4_LightProjection;

float ShadowCalculation()
{
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec4 fragPosLightSpace = m4_LightProjection * vec4(FragPos,1.0f);
    // perform perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(gShadow, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // calculate bias (based on depth map resolution and slope)
    vec3 normal = texture(gNormal, TexCoords).rgb;
    vec3 lightDir = normalize(-directLight.Direction);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    // check whether current frag pos is in shadow
    // float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;
    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(gShadow, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(gShadow, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    shadow /= 9.0;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;
}

void main()
{             
    // retrieve data from gbuffer
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec3 Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float Specular = texture(gAlbedoSpec, TexCoords).a;
    vec4 NoLight = texture(gNoLight, TexCoords);
    
    // then calculate lighting as usual
    float shadow = 1-ShadowCalculation();
    vec3 lighting  = Diffuse * 0.1 * clamp(shadow,0.0f,1.0f); // hard-coded ambient component
    vec3 viewDir  = normalize(viewPos - FragPos);
    for(int i = 0; i < activeLights; ++i)
    {
        // calculate distance between light source and current fragment
        float distance = length(lights[i].Position - FragPos);
        if(distance < lights[i].Radius)
        {
            // diffuse
            vec3 lightDir = normalize(lights[i].Position - FragPos);
            vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * lights[i].Color;
            // specular
            vec3 halfwayDir = normalize(lightDir + viewDir);  
            float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
            vec3 specular = lights[i].Color * spec * Specular;
            // attenuation
            float attenuation = 1.0 / (1.0 + lights[i].Linear * distance + lights[i].Quadratic * distance * distance);
            diffuse *= attenuation;
            specular *= attenuation;
            lighting += diffuse + specular;
        }
    }

    // Apply directional light
    vec3 lightDir = normalize(-directLight.Direction);
    vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * directLight.Color;
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
    vec3 specular = directLight.Color * spec * Specular;
    diffuse *= directLight.Diffuse;
    specular *= directLight.Specular;
    lighting += diffuse + specular;

    outColor = mix(vec4(lighting, 1.0),NoLight,NoLight.a);

    // check whether result is higher than some threshold, if so, output as bloom threshold color
    float brightness = dot(outColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.9)
        brightColor = vec4(outColor.rgb,1.0);
    else
        brightColor = vec4(0.0, 0.0, 0.0, 1.0);
}