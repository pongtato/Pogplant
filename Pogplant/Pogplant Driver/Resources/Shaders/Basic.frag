#version 450 core

layout (location = 3) out vec4 gNoLight;
layout (location = 5) out vec4 gCanvas;

in vec4 ColorCoords;
in vec2 TexCoords;
in vec2 Tiling;
in vec2 UV_Offset;
flat in int TexID;

uniform sampler2D Textures[64];

void main() 
{
    gNoLight = vec4(0);

    if(TexID >= 0)
    {
        vec2 TiledCoords = vec2(TexCoords.x * Tiling.x, TexCoords.y * Tiling.y);
        vec2 NewTex = vec2(TiledCoords.x, 1 - TiledCoords.y) + UV_Offset;
        gCanvas = texture(Textures[TexID],NewTex);

        //gNoLight = texture(Textures[int(TexID)],vec2(TexCoords.x,1-TexCoords.y));
        gCanvas *= ColorCoords;
    }
    else
    {
        gCanvas = ColorCoords;
    }  

    if(gCanvas.a < 0.169)
    {
        discard;
    }
    
}
