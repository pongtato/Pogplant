#version 450 core

out vec4 FragColor;

in vec2 TexCoords;

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
    FragColor = texture(material.texture_diffuse[0], TexCoords);

    // Texture not included yet
    if(noTex == 1)
    {
        FragColor = vec4(0.3f,0.3f,0.3f,1.0f);
    }
}