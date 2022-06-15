#version 330
 
// shader input
layout( location = 0 ) in vec3 vPosition;		// untransformed vertex position
layout( location = 1 ) in vec3 vNormal;		// untransformed vertex normal
layout( location = 2 ) in vec2 vUV;			// vertex uv coordinate
layout( location = 3 ) in vec3 vTangent;
layout( location = 4 ) in vec3 vBitangent;

// shader output
out vec3 normal;		// transformed vertex normal
out vec3 fragPos;
out vec2 uv;		
out mat3 TBN;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = projection * view * model * vec4(vPosition, 1.0);
	fragPos = vec3(model) * vPosition;
	
	mat3 modelVector = mat3(model);
	normal = normalize(modelVector * vNormal);
	uv = vUV;

    vec3 T = normalize(modelVector * vTangent);
    vec3 B = normalize(modelVector * vBitangent);
    vec3 N = normalize(modelVector * vNormal.xyz);

	TBN = mat3(T, B, N);
}