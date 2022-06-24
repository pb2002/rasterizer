using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
	public class ScreenQuad
	{
		// data members
		int _vboIdx = 0, _vboVert = 0;
		float[] _vertices = { -1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, -1, 0, 1, 0, -1, -1, 0, 0, 0 };
		int[] _indices = { 0, 1, 2, 3 };
		// constructor
		public ScreenQuad()
		{
		}

		// initialization; called during first render
		public void Prepare( Shader shader )
		{
			if( _vboVert == 0 )
			{
				// prepare VBO for quad rendering
				GL.GenBuffers( 1, out _vboVert );
				GL.BindBuffer( BufferTarget.ArrayBuffer, _vboVert );
				GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(4 * 5 * 4), _vertices, BufferUsageHint.StaticDraw );
				GL.GenBuffers( 1, out _vboIdx );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, _vboIdx );
				GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(16), _indices, BufferUsageHint.StaticDraw );
			}
		}

		// render the mesh using the supplied shader and matrix
		public void Render( Shader shader, int textureId, int colorLutId )
		{
			// on first run, prepare buffers
			Prepare( shader );

			// enable shader
			GL.UseProgram( shader.ProgramID );

			// enable texture
			shader.BindTexture("pixels", textureId, 0);
			shader.BindTexture("lut", colorLutId, 1);

			// enable position and uv attributes
			GL.EnableVertexAttribArray( shader.AttributeVpos );
			GL.EnableVertexAttribArray( shader.AttributeVuvs );

			// bind interleaved vertex data
			GL.BindBuffer( BufferTarget.ArrayBuffer, _vboVert );

			// link vertex attributes to shader parameters 
			GL.VertexAttribPointer( shader.AttributeVpos, 3, VertexAttribPointerType.Float, false, 20, 0 );
			GL.VertexAttribPointer( shader.AttributeVuvs, 2, VertexAttribPointerType.Float, false, 20, 3 * 4 );

			// bind triangle index data and render
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _vboIdx );
			GL.DrawArrays( PrimitiveType.Quads, 0, 4 );

			// disable shader
			GL.UseProgram( 0 );
		}
	}
}