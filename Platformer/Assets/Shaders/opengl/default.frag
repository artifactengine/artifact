#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D tex0;

uniform vec4 tint;

void main()
{
    FragColor = tint * texture(tex0, texCoord);

    if (FragColor.a == 0) {
        discard;
    }
}