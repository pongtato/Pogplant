#version 450 core

layout (location = 0) out vec4 Frag_Colour;
layout (location = 1) out vec4 Bright_Colour;

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
        Frag_Colour = texture(Textures[int(TexID)],newTex);
        Frag_Colour.rgb *= ColorCoords.rgb;
    }
    else
    {
        Frag_Colour = ColorCoords;
    }
    // vec3 mixColor = mix(Frag_Colour.rgb,ColorCoords.rgb,ColorCoords.a);
    // Frag_Colour.rgb = mixColor;

    if(Frag_Colour.a < 0.01f)
    {
        discard;
    }

    // Bloom handle
    bool b_Bloom = bool(Bloom);
    if(!b_Bloom)
    {
        Bright_Colour = vec4(0);
    }
    else
    {
        Bright_Colour = ColorCoords;
    }
}
