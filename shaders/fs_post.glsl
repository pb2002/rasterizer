#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;


// magie \[T]/
float A = 0.15;
float B = 0.50;
float C = 0.10;
float D = 0.20;
float E = 0.02;
float F = 0.30;
float W = 11.2;

vec3 Uncharted2Tonemap(vec3 x)
{
   return ((x*(A*x+C*B)+D*E)/(x*(A*x+B)+D*F))-E/F;
}

void main()
{
	// retrieve input pixel
	vec3 c = texture( pixels, uv ).rgb;
	
	float exposureBias = 2.0;
	vec3 curr = Uncharted2Tonemap(exposureBias * c);
	
	vec3 whiteScale = 1.0/Uncharted2Tonemap(vec3(W));
	vec3 color = curr * whiteScale;
	
	outputColor = color;
}

// EOF