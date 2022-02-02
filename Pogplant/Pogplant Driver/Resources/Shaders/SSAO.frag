#version 450 core

layout (location = 0) out float FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform vec3 v3_Samples[64];

int kernelSize = 64;

uniform vec2 v2_Noise;
uniform mat4 m4_Projection;
uniform float Radius;
uniform float Bias;

void main()
{
    // Input for SSAO algorithm
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = normalize(texture(gNormal, TexCoords).rgb);
    vec3 randomVec = normalize(texture(texNoise, TexCoords * v2_Noise).xyz);

    // Tangent to view space
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);

    float occlusion = 0.0;
    for(int i = 0; i < kernelSize; ++i)
    {
        vec3 samplePos = TBN * v3_Samples[i]; 
        samplePos = fragPos + samplePos * Radius;
        
        vec4 offset = vec4(samplePos, 1.0);
        // View to clip
        offset = m4_Projection * offset; 
        // Perspective
        offset.xyz /= offset.w; 
        // Range 0.0 - 1.0
        offset.xyz = offset.xyz * 0.5 + 0.5; 
        
        // Depth
        float sampleDepth = texture(gPosition, offset.xy).z; // get depth value of kernel sample
        
        // Range check & accumulate
        float rangeCheck = smoothstep(0.0, 1.0, Radius / abs(fragPos.z - sampleDepth));
        occlusion += (sampleDepth >= samplePos.z + Bias ? 1.0 : 0.0) * rangeCheck;           
    }
    occlusion = 1.0 - (occlusion / kernelSize);
    
    FragColor = occlusion;
}