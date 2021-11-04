#version 450 core

layout (location = 0) out vec4 outColor;
layout (location = 1) out vec4 brightColor;

//out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;
uniform sampler2D gNoLight;
uniform sampler2D gEmissive;
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

float Shadow()
{
    vec3 fragPos = texture(gPosition, TexCoords).rgb;

    // Map to correct space 
    vec4 fragPosLightSpace = m4_LightProjection * vec4(fragPos,1.0f);
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;
    float closestDepth = texture(gShadow, projCoords.xy).r; 
    float currDepth = projCoords.z;

    // Bias calc
    vec3 normal = texture(gNormal, TexCoords).rgb;
    vec3 lightDir = normalize(-directLight.Direction);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);

    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(gShadow, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(gShadow, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
        
    // Cull outside frustum
    if(projCoords.z > 1.0)
    {
        return 0;
    }
    else
    {
        return shadow /= 12.0;
    }
}

void main()
{             
    // gBuffer sample
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec3 Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float Specular = texture(gAlbedoSpec, TexCoords).a;
    vec4 NoLight = texture(gNoLight, TexCoords);
    
    // Light calc
    const float ambient = 0.42f;
    float shadow = 1 - Shadow();
    vec3 lighting  = Diffuse * ambient * clamp(shadow,0.0f,1.0f);
    vec3 viewDir  = normalize(viewPos - FragPos);
    for(int i = 0; i < activeLights; ++i)
    {
        // Distance cutoff
        float distance = length(lights[i].Position - FragPos);
        if(distance < lights[i].Radius)
        {
            // Diffuse
            vec3 lightDir = normalize(lights[i].Position - FragPos);
            vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * lights[i].Color;
            // Specular
            vec3 halfwayDir = normalize(lightDir + viewDir);  
            float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
            vec3 specular = lights[i].Color * spec * Specular;
            // Attenuation
            const float k = 1.0f;
            float attenuation = k / (k + lights[i].Linear * distance + lights[i].Quadratic * distance * distance);
            diffuse *= attenuation;
            specular *= attenuation;
            lighting += diffuse + specular;
        }
    }

    // Directional light
    vec3 lightDir = normalize(-directLight.Direction);
    vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * directLight.Color;
    
    // Specular
    vec3 halfway = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(Normal, halfway), 0.0), 16.0);
    vec3 specular = directLight.Color * spec * Specular;
    diffuse *= directLight.Diffuse;
    specular *= directLight.Specular;
    lighting += diffuse + specular;

    outColor = mix(vec4(lighting, 1.0),NoLight,NoLight.a);

    // Output bright bixels for bloom
    //float brightness = dot(outColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    //if(brightness > 0.69f)
    //    brightColor = vec4(outColor.rgb,1.0);
    //else
    //    brightColor = vec4(0.0, 0.0, 0.0, 1.0);

    brightColor = vec4(texture(gEmissive, TexCoords).rgb,1.0);
}