#version 450 core

layout (location = 3) out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D text;
uniform vec3 textColor;
uniform vec2 offset;
uniform float distanceRange;

const float imageSize = 512.0;

float median(float r, float g, float b) 
{
    return max(min(r, g), min(max(r, g), b));
}

void main()
{   
    vec2 flipped_texCoords = vec2(TexCoords.x, 1.0 - TexCoords.y) + offset;
    vec2 pos = flipped_texCoords;
    vec3 distance = texture(text, flipped_texCoords).rgb;
    ivec2 sz = textureSize(text, 0).xy;
    float dx = dFdx(pos.x) * sz.x; 
    float dy = dFdy(pos.y) * sz.y;
    float sigDist = median(distance.r, distance.g, distance.b);
    float w = fwidth(sigDist);
    float opacity = smoothstep(0.5 - w, 0.5 + w, sigDist);

    if(opacity > 0)
    {
        FragColor = vec4(textColor, opacity);
    }
    else
    {
        discard;
    }
}