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
uniform sampler2DArray gShadow;
//uniform sampler2D gShaft;
uniform sampler2D gCanvas;
uniform sampler2D gAO;

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
uniform int activeLights;

uniform float BloomDamp;
uniform float Exposure;
uniform float Gamma;
uniform bool Shadows;

uniform float Decay             = 0.69;
uniform float Shaft_Exposure    = 1.0;
uniform float Density           = 5.0;
uniform float Weight            = 0.69;
uniform vec2 ScreenSize;
uniform vec2 LightScreenPos;
int NUM_SAMPLES = 100;

/// CSM
layout (std140, binding = 0) uniform LightSpaceMatrices
{
    mat4 lightSpaceMatrices[16];
};
uniform float cascadePlaneDistances[16];
uniform int cascadeCount;
uniform vec3 lightDir;
uniform float farPlane;
uniform mat4 m4_InverseView;

float Shadow(vec3 normal)
{
    vec4 fragPosViewSpace = vec4(texture(gPosition, TexCoords).rgb, 1.0);
    float depthValue = abs(fragPosViewSpace.z);

    int layer = -1;
    for (int i = 0; i < cascadeCount; ++i)
    {
        if (depthValue < cascadePlaneDistances[i])
        {
            layer = i;
            break;
        }
    }
    if (layer == -1)
    {
        layer = cascadeCount;
    }

    // Map spaces, inverse to get back world space
    vec4 fragPosLightSpace = lightSpaceMatrices[layer] * vec4(vec3(m4_InverseView * fragPosViewSpace),1.0);
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;

    // Cull outside
    float currentDepth = projCoords.z;
    if (currentDepth  > 1.0)
    {
        return 0.0;
    }

    // Bias calc
    float bias = max(0.025 * (1.0 - dot(normal, lightDir)), 0.005);
    if (layer == cascadeCount)
    {
        bias *= 1 / (farPlane * 0.5f);
    }
    else
    {
        bias *= 1 / (cascadePlaneDistances[layer] * 0.5f);
    }

    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / vec2(textureSize(gShadow, 0));
    const int samples = 5;
    for(int x = -samples; x <= samples; ++x)
    {
        for(int y = -samples; y <= samples; ++y)
        {
            float pcfDepth = texture(gShadow, vec3(projCoords.xy + vec2(x, y) * texelSize, layer)).r; 
            shadow += (currentDepth - bias) > pcfDepth ? 1.0 : 0.0;        
        }    
    }
    
    return shadow /= pow((samples * 2 + 1), 2);;
}

void main()
{             
    // gBuffer sample
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec3 Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float Specular = texture(gAlbedoSpec, TexCoords).a;
    vec4 NoLight = texture(gNoLight, TexCoords);
    vec4 Canvas = texture(gCanvas, TexCoords);
    Canvas.a *= 6.9f;
    Canvas.a = clamp(Canvas.a, 0.0f, 1.0f);
    float AmbientOcclusion = texture(gAO, TexCoords).r;
    
    // Light calc
    const float ambient = 0.42f;

    float shadow = 1.0;
    vec3 lighting = Diffuse * ambient;
    vec3 colorDebug = vec3(0);
    if(Shadows)
    {
        shadow = 1.0 - Shadow(Normal);

        // Visualisation of the cascades
        int layer = int(Shadow(Normal));
        if(layer == 0)
            colorDebug = vec3(1,0,0);
        if(layer == 1)
            colorDebug = vec3(0,1,0);
        if(layer == 2)
            colorDebug = vec3(0,0,1);
    }
    
    vec3 viewDir = normalize(-FragPos);
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
    lighting += shadow * (diffuse + specular);
    lighting.rgb *= AmbientOcclusion;

    // HDR light
    lighting = vec3(1.0) - exp(-lighting.rgb * Exposure); 

    outColor = mix(vec4(lighting, 1.0),NoLight,NoLight.a);
    outColor = mix(outColor, Canvas, Canvas.a);
    
    // Gamma correct
    outColor.rgb = pow(outColor.rgb, vec3(1.0 / Gamma));

    // So that canvas/text does not have bleeding bloom/bloom, done this way as you cannot compare 0
    if(Canvas.a > 0)
    {
    }
    else
    {
        // Output bright bixels for bloom
        float brightness = dot(outColor.rgb, vec3(0.2126, 0.7152, 0.0722));
        brightColor = vec4(outColor.rgb * 1.0f / (brightness * BloomDamp), 1.0f);
        brightColor += vec4(texture(gEmissive, TexCoords).rgb,1.0);
    }
}