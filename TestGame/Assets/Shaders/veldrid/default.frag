#version 450

layout(location = 0) in vec2 fsin_TexCoord;
layout(location = 0) out vec4 fsout_Color;

layout(set = 0, binding = 1) uniform texture2D aTexture;
layout(set = 0, binding = 2) uniform sampler aSampler;

void main()
{
    fsout_Color = texture(sampler2D(aTexture, aSampler), fsin_TexCoord);
}