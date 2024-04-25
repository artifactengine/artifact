#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D tex0;

uniform vec4 tint;

void main()
{
    FragColor = texture(tex0, texCoord) * tint;

    if (FragColor == vec4(0, 0, 0, 1)) {
        discard;
    }
}