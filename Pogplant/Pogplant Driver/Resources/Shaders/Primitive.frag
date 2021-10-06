#version 450 core

//out vec4 FragColor;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;
layout (location = 3) out vec4 gNoLight;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;
in vec3 Color;
in float Height;
in mat3 TBN;

const int MAX_TEX = 3;

uniform sampler2D texture_diffuse;
uniform sampler2D texture_normal;
uniform sampler2D texture_disp;
uniform sampler2D texture_specular;

uniform sampler2D texture_diffuse2;
uniform sampler2D texture_normal2;
uniform sampler2D texture_disp2;
uniform sampler2D texture_specular2;

uniform float blend;
uniform int activeTextures;

void main()
{   
    gPosition = FragPos;

    vec3 difflBlend =  mix(texture(texture_diffuse, TexCoords).rgb, texture(texture_diffuse2, TexCoords).rgb, Height);
    vec3 normalBlend =  mix(texture(texture_normal, TexCoords).rgb, texture(texture_normal2, TexCoords).rgb, Height);
    float specBlend =  mix(texture(texture_specular, TexCoords).rgb, texture(texture_specular2, TexCoords).rgb, Height).r;
    //gNormal =  normalize(vec3(Normal.xy + normalBlend.xy, Normal.z));
    gNormal = normalize(normalBlend * 2.0 - 1.0);
    gNormal = normalize(TBN * gNormal);
    
    gAlbedoSpec.rgb = difflBlend;
    gAlbedoSpec.a = 1.0f - specBlend;
    gNoLight = vec4(0.0f);
}