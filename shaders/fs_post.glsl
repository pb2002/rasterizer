#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform sampler2D lut;

#define CG_COLORS 16.0
#define CG_MAX 15.0
#define CG_W 256.0
#define CG_H 16.0

// shader output
out vec3 outputColor;

// uncharted 2 tonemapper:
// http://filmicworlds.com/blog/filmic-tonemapping-operators/

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

// color grading: https://defold.com/tutorials/grading/
vec3 ColorGrading(vec3 color){
    
    // get lut cell
    float cell = color.g * CG_MAX;
	float cell_l = floor(cell);
	float cell_h = ceil(cell);
	
	// pixel offset
	float halfw = 0.5 / CG_W;
	float halfh = 0.5 / CG_H;
	
	float lut_x = halfw + color.r / CG_COLORS * (CG_MAX / CG_COLORS);
	float lut_y = halfh + color.b * (CG_MAX / CG_COLORS);
	
	// sample twice to interpolate green channel
	vec2 lut_pos_l = vec2(cell_l / 16 + lut_x, lut_y);
	vec2 lut_pos_h = vec2(cell_h / CG_COLORS + lut_x, lut_y);
	
	vec3 col_l = texture(lut, lut_pos_l).rgb;
	vec3 col_h = texture(lut, lut_pos_h).rgb;
	
	// mix by cell fraction
	return mix(col_l, col_h, fract(cell));
}

void main()
{
	// retrieve input pixel
	vec3 c = texture( pixels, uv ).rgb; 		
	
	float exposureBias = 2.0;
	vec3 curr = Uncharted2Tonemap(exposureBias * c);
	
	vec3 whiteScale = 1.0/Uncharted2Tonemap(vec3(W));
	vec3 color = clamp(curr * whiteScale, 0, 1);				
	
	
	outputColor = ColorGrading(color);
}

// EOF