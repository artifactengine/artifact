#version 450

layout(location = 0) in vec2 fsin_TexCoord;
layout(location = 0) out vec4 fsout_Color;

layout(set = 0, binding = 1) uniform texture2D aTexture;
layout(set = 0, binding = 2) uniform sampler aSampler;

void main()
{
    vec2 rotatedTexCoord = vec2(fsin_TexCoord.y, 1.0 + fsin_TexCoord.x); // Rotate 90 degrees to the left
    fsout_Color = texture(sampler2D(aTexture, aSampler), rotatedTexCoord);

    if (fsout_Color == vec4(0, 0, 0, 1)) {
        discard;
    }
}
