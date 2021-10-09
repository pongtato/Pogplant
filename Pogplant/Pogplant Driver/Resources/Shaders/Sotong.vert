#version 450 core

layout (location = 0) in vec2 v2_Pos;
layout (location = 1) in vec2 v2_TexCoords;

uniform bool horizontal;
uniform float targetWidth;
uniform float targetHeight;

out vec2 blurTexCoords[11];

void main() 
{
    gl_Position = vec4(v2_Pos, 0.0f, 1.0f);

    vec2 center = v2_TexCoords;
    if(horizontal)
    {
        float pixelSize = 1.0/targetWidth;

        for(int i = -5; i <= 5; ++i)
        {
            blurTexCoords[i+5] = center + vec2(pixelSize * i, 0.0);
        }
    }
    else
    {
        float pixelSize = 1.0/targetHeight;

        for(int i = -5; i <= 5; ++i)
        {
            blurTexCoords[i+5] = center + vec2(0.0, pixelSize * i);
        }
    }
}
