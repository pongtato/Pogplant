#version 450 core

//out vec4 FragColor;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

struct Material
{
    sampler2D texture_diffuse[3];
    sampler2D texture_specular[3];
    sampler2D texture_normal[3];
    sampler2D texture_height[3];
};

uniform Material material;
uniform int noTex;

void main()
{    
    gPosition = FragPos;
    gNormal = normalize(Normal);
    if(noTex == 0)
    {
        gAlbedoSpec.rgb = texture(material.texture_diffuse[0], TexCoords).rgb;
        gAlbedoSpec.a = texture(material.texture_specular[0], TexCoords).r;
    }
    else
    {
        gAlbedoSpec = vec4(gNormal,1.0f);
    }
}