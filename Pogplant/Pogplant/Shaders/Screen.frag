#version 450 core

layout (location = 0) out vec4 Frag_Colour;
layout (location = 1) out vec4 Bright_Colour;

in vec2 TexCoords;

uniform sampler2D s2D_ScreenTexture;

const float offset = 1.0 / 300.0;  

void main () 
{
    Frag_Colour = texture(s2D_ScreenTexture, TexCoords);
    float brightness = dot(Frag_Colour.rgb, vec3(0.2126, 0.7152, 0.0722));
    Bright_Colour = vec4(0,0,0,1);
    if(brightness > 0.1)
    {
        Bright_Colour = vec4(Frag_Colour.rgb * 0.75f,1.0);
    }
}
