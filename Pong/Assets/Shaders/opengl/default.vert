#version 330 core
layout (location = 0) in vec4 aPos;
layout (location = 1) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 proj;
uniform mat4 view;

out vec2 texCoord;

void main()
{
    gl_Position = proj * view * model * vec4(aPos.x, aPos.y, 5, 1.0);
    texCoord = aTexCoord;
}