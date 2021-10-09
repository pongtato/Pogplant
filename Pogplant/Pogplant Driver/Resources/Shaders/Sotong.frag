#version 450 core

layout (location = 0) out vec4 outColor;

in vec2 blurTexCoords[11];

uniform sampler2D image;

uniform float weights[11] = float[] 
(
    0.009,
    0.028,
    0.066,
    0.122,
    0.176,
    0.199,
    0.176,
    0.122,
    0.066,
    0.028,
    0.009
);

void main()
{             
    vec3 result = vec3(0.0);

    for(int i = 0; i < 11; ++i)
    {
        if(blurTexCoords[i].x < 0 || blurTexCoords[i].x > 1 || blurTexCoords[i].y < 0 || blurTexCoords[i].y > 1)
        {
            continue;
        }

        result += texture(image, blurTexCoords[i]).rgb * weights[i];
    }

    outColor = vec4(result, 1.0);
}