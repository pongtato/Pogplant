#version 450 core

//out vec4 FragColor;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;
layout (location = 3) out vec4 gNoLight;
layout (location = 4) out vec3 gEmissive;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

struct Material
{
    sampler2D texture_diffuse[3];
    sampler2D texture_specular[3];
    sampler2D texture_normal[3];
    sampler2D texture_emissive[3];
    sampler2D texture_height[3];
};

uniform Material material;
uniform vec3 colorTint;
uniform bool noTex;
uniform bool useLight;

void main()
{    
    if(useLight)
    {
        gPosition = FragPos;
        gNormal = normalize(Normal);
        if(!noTex)
        {
            gAlbedoSpec.rgb = texture(material.texture_diffuse[0], TexCoords).rgb;
            gAlbedoSpec.a = texture(material.texture_specular[0], TexCoords).r;
        }
        else
        {
            gAlbedoSpec = vec4(colorTint,0.0f);
        }
    }
    else
    {
        gNoLight = vec4(colorTint,1.0f);
        gPosition = vec3(0.0f);
        gNormal = vec3(0.0f);
        gAlbedoSpec = vec4(colorTint,0.0f);
    }

    // Regardless of light or not
    if(material.emissive_count > 0)
    {
        // If not data from other samples would be used
        gEmissive = texture(material.texture_emissive[0], TexCoords).rgb;
    }
    else
    {
        gEmissive = vec3(0.0f);
    }
}