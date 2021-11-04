#version 450 core

layout (location = 3) out vec4 gNoLight;

in vec4 ColorCoords;
in vec2 TexCoords;
flat in int TexID;

uniform sampler2D Textures[32];

void main() 
{
    if(TexID >= 0)
    {
        gNoLight = texture(Textures[int(TexID)],TexCoords);
        gNoLight.rgb *= ColorCoords.rgb;
    }
    else
    {
        gNoLight = ColorCoords;
    }  

    if(gNoLight.a < 1.0f)
    {
         discard;
    }
    else
    {
         // Emissive pass
    }
}
