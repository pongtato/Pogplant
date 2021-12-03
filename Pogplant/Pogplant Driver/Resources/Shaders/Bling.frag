#version 450 core

layout (location = 0) out vec4 outColor;

in vec2 TexCoords;

uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform sampler2D debug;
uniform bool bloom;

void main()
{             
    vec4 hdrColor = texture(scene, TexCoords);      
    vec4 bloomColor = texture(bloomBlur, TexCoords);
    vec4 debugColor = texture(debug, TexCoords);
  
    if(bloom)
    {
        hdrColor += bloomColor;
        outColor = vec4(hdrColor.rgb, 1.0);
    }
    else
    {
        outColor = vec4(hdrColor.rgb, 1.0);
    }

    //outColor = bloomColor;

    if(length(debugColor.rgb) > 0)
    {
        outColor = debugColor;
    }
}