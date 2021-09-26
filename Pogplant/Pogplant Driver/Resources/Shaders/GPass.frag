#version 450 core

layout (location = 0) out vec4 outColor;

//out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;
uniform sampler2D gNoLight;

struct Light 
{
    vec3 Position;
    vec3 Color;
    
    float Linear;
    float Quadratic;
    float Radius;
};

struct DirectionalLightDir
{
    vec3 Direction;
    vec3 Color;

    float Diffuse;
    float Specular;
};

const int MAX_LIGHTS = 32;
uniform Light lights[MAX_LIGHTS];
uniform DirectionalLightDir directLight;
uniform vec3 viewPos;
uniform int activeLights;

void main()
{             
    // retrieve data from gbuffer
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec3 Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float Specular = texture(gAlbedoSpec, TexCoords).a;
    vec4 NoLight = texture(gNoLight, TexCoords);
    
    // then calculate lighting as usual
    vec3 lighting  = Diffuse * 0.42; // hard-coded ambient component
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

    outColor = NoLight + vec4(lighting, 1.0f);
}