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

const int MAX_TEX = 3;

uniform sampler2D texture_diffuse[MAX_TEX];
uniform sampler2D texture_normal[MAX_TEX];
uniform sampler2D texture_disp[MAX_TEX];
uniform sampler2D texture_specular[MAX_TEX];

uniform float blend;
uniform int activeTextures;

void main()
{   
    gPosition = FragPos;

    vec3 sampleDiff[MAX_TEX];
    vec3 sampleNorms[MAX_TEX];
    vec3 sampleBump[MAX_TEX];
    float sampleSpec[MAX_TEX];

    float scale = 1/activeTextures;
    int blendIndex = int(Height/scale);

    for(int i = blendIndex; i < blendIndex + 1; ++i)
    {
        sampleDiff[i] = texture(texture_diffuse[i], TexCoords).rgb;
        sampleNorms[i] = texture(texture_normal[i], TexCoords).rgb;
        sampleBump[i] = texture(texture_disp[i], TexCoords).rgb;
        sampleSpec[i] = texture(texture_specular[i], TexCoords).r;
    }

    const int ci = blendIndex;  // curr index
    const int ni = ci + 1;      // next index

    vec3 diffBlend = mix(sampleDiff[ci], sampleDiff[ni], blend);
    vec3 normalBlend = mix(sampleNorms[ci], sampleNorms[ni], blend);
    vec3 bumpBlend = mix(sampleBump[ci], sampleBump[ni], blend);
    float specBlend = mix(sampleSpec[ci], sampleSpec[ni], blend);

    gNormal = normalize(vec3(Normal.xy + normalBlend.xy, Normal.z));
    gAlbedoSpec.rgb = diffBlend;
    gAlbedoSpec.a = specBlend;
    gAlbedoSpec = vec4(Color,1.0f);
    gNoLight = vec4(Color,1.0f);
}