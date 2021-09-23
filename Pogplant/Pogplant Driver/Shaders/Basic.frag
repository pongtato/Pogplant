#version 450 core

layout (location = 3) out vec4 FragColor;

in vec4 ColorCoords;
in vec2 TexCoords;
in vec2 UV;
in vec2 UV_Offset;
flat in int TexID;
flat in int Bloom;

uniform sampler2D Textures[32];

void main () 
{
    vec2 tiledCoords = vec2(TexCoords.x * UV.x, TexCoords.y * UV.y);
    vec2 newTex = vec2(tiledCoords.x, tiledCoords.y) + UV_Offset;
    
    if(TexID >= 0)
    {
        FragColor = texture(Textures[int(TexID)],newTex);
        FragColor.rgb *= ColorCoords.rgb;
    }
    else
    {
        FragColor = ColorCoords;
    }

    if(FragColor.a < 0.01f)
    {
        discard;
    }
}
