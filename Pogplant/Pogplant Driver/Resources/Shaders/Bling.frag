#version 450 core

layout (location = 0) out vec4 outColor;

in vec2 TexCoords;

uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform sampler2D debug;
uniform bool bloom;

const float exposure = 0.69f;

void main()
{             
    const float gamma = 2.2;
    vec4 hdrColor = texture(scene, TexCoords);      
    vec4 bloomColor = texture(bloomBlur, TexCoords);
    vec4 debugColor = texture(debug, TexCoords);
  

    if(bloom)
    {
        hdrColor += bloomColor; // additive blending

        // tone mapping
        vec3 result = vec3(1.0) - exp(-hdrColor.rgb * exposure);
        // also gamma correct while we're at it       
        result = pow(hdrColor.rgb, vec3(1.0 / gamma));
        outColor = vec4(result.rgb, 1.0);
    }
    else
    {
        outColor = vec4(hdrColor.rgb, 1.0);
    }

    if(length(debugColor.rgb) > 0)
        outColor = debugColor;
}