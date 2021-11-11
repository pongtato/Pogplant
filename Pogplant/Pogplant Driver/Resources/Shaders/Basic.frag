#version 450 core

layout (location = 3) out vec4 gNoLight;
layout (location = 5) out vec3 gShaft;

in vec4 ColorCoords;
in vec2 TexCoords;
flat in int TexID;

uniform sampler2D Textures[32];

void main() 
{
    if(TexID >= 0)
    {
        gNoLight = texture(Textures[int(TexID)],vec2(TexCoords.x,1-TexCoords.y));
        gNoLight.rgb *= ColorCoords.rgb;
    }
    else
    {
        gNoLight = ColorCoords;
    }  

    if(gNoLight.a < 0.42 )
    {
        discard;
    }
    else
    {
        //discard;
    }

    gShaft = vec3(0);
}
