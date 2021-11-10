#version 450 core

//out vec4 FragColor;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;
layout (location = 3) out vec4 gNoLight;
layout (location = 4) out vec3 gEmissive;
layout (location = 5) out vec3 gShaft;

in vec3 FragPos;
in vec3 Normal;
in vec3 ColorTint;
in vec3 EmissiveTint;
in vec2 TexCoords;
flat in int UseLight;
flat in int UseTexture;
flat in int ToDraw;

struct Material
{
    sampler2D texture_diffuse[3];
    sampler2D texture_specular[3];
    sampler2D texture_normal[3];
    sampler2D texture_emissive[3];
    sampler2D texture_height[3];
    int emissive_count;
};

uniform Material material;

void main()
{    
    if(ToDraw != 1)
    {
        discard;
    }

    if(UseLight == 1)
    {
        gPosition = FragPos;
        gNormal = normalize(Normal);
        gNoLight = vec4(0.0f);
        if(UseTexture == 1)
        {
            gAlbedoSpec.rgb = texture(material.texture_diffuse[0], TexCoords).rgb * ColorTint;
            gAlbedoSpec.a = texture(material.texture_specular[0], TexCoords).r;
        }
        else
        {
            gAlbedoSpec = vec4(ColorTint,0.0f);
        }
    }
    else
    {
        gPosition = vec3(0.0f);
        gNormal = vec3(0.0f);
        gAlbedoSpec = vec4(0.0f);
        if(UseTexture == 1)
        {
            gNoLight = vec4(texture(material.texture_diffuse[0], TexCoords).rgb * ColorTint,1.0);
        }
        else
        {
            gNoLight = vec4(ColorTint,1.0f);
        }
    }
    
    // Regardless of light or not
    if(material.emissive_count > 0)
    {
        // If not data from other samples would be used
        gEmissive = texture(material.texture_emissive[0], TexCoords).rgb * EmissiveTint;
    }
    else
    {
        gEmissive = vec3(0.0f);
    }

    // Block the light which is using the shaft shader 
    gShaft = vec3(0.0f);
}