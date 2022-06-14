#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
uniform sampler2D pixels;	// texture sampler
uniform vec3 lightDir;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    vec3 albedo = vec3(max(0, dot(normal.xyz, -lightDir)));
    vec3 ambient = vec3(0.1, 0.15, 0.1);
    outputColor = texture( pixels, uv ) * vec4(albedo + ambient, 1);
}