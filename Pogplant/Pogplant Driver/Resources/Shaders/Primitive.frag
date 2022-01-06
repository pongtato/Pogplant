#version 450 core

//out vec4 FragColor;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;
layout (location = 3) out vec4 gNoLight;
layout (location = 5) out vec4 gCanvas;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;
in vec3 Color;
in float Height;
in mat3 TBN;
in mat3 nmMtx;

in vec3 iViewPos; 
in vec3 iFragPos;

const int MAX_TEX = 3;

uniform sampler2D texture_diffuse;
uniform sampler2D texture_normal;
uniform sampler2D texture_disp;
uniform sampler2D texture_specular;

uniform sampler2D texture_diffuse2;
uniform sampler2D texture_normal2;
uniform sampler2D texture_disp2;
uniform sampler2D texture_specular2;

uniform float tile;
uniform int activeTextures;

uniform vec3 v3_ViewPosition;

vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir)
{ 
    float dscale = 0.1f;
    float dbias = -dscale/2.0f;
    float sampledDepth = mix(texture(texture_disp, texCoords).rgb, texture(texture_disp2, texCoords).rgb, Height).r;
    return texCoords + viewDir.xy * sampledDepth * dscale + dbias;
}

void main()
{   
    // Bump
    vec3 viewDir = normalize(v3_ViewPosition - FragPos);
    vec2 nTexCoords = ParallaxMapping(TexCoords,  nmMtx * viewDir);
    if(nTexCoords.x > 1.0 || nTexCoords.y > 1.0 || nTexCoords.x < 0.0 || nTexCoords.y < 0.0)
    {
        discard;
    }
    // Tile
    nTexCoords *= tile;
    
    // Sampling
    vec3 difflBlend =  mix(texture(texture_diffuse, nTexCoords).rgb, texture(texture_diffuse2, nTexCoords).rgb, Height);
    vec3 normalBlend =  mix(texture(texture_normal, nTexCoords).rgb, texture(texture_normal2, nTexCoords).rgb, Height);
    float specBlend =  mix(texture(texture_specular, nTexCoords).rgb, texture(texture_specular2, nTexCoords).rgb, Height).r;

    // Set
    gNormal = normalize(TBN * (normalBlend * 2.0 - 1.0));
    gPosition = FragPos;
    gAlbedoSpec.rgb = difflBlend * Color;
    gAlbedoSpec.a = 1.0f - specBlend;
    gNoLight = vec4(0.0f);
    gCanvas = vec4(0.0f);
}