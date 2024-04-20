#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

layout(set = 0, binding = 0) uniform MVP {
    mat4 mvp;
};

void main()
{
    gl_Position = mvp * vec4(Position, 0, 1);
    fsin_Color = Color;
}