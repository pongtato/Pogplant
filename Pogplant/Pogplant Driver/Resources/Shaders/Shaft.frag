#version 450 core

//out vec4 FragColor;

layout (location = 3) out vec4 gNoLight;
layout (location = 5) out vec4 gShaft;

uniform vec3 colorTint;
uniform sampler2D S2D_Texture;

in vec2 TexCoords;

void main()
{   
    gNoLight = texture(S2D_Texture, vec2(TexCoords.x,1-TexCoords.y));

    if(gNoLight.a < 1.0f)
    {
         discard;
    }

    gNoLight.rgb *= colorTint;
    gShaft = vec4(colorTint,1);
}