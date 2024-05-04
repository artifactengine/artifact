#version 450

layout(location = 0) in vec4 Position;
layout(location = 1) in vec2 TexCoord;

layout(location = 0) out vec2 fsin_TexCoord;

layout(set = 0, binding = 0) uniform MVP {
    mat4 mvp;
};

void main()
{
    gl_Position = mvp * Position;
    fsin_TexCoord = TexCoord;
}