using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
	// mesh and loader based on work by JTalton; http://www.opentk.com/node/642

	public class Mesh
	{
		// data members
		public ObjVertex[] Vertices;            // vertex positions, model space
		public ObjTriangle[] Triangles;         // triangles (3 vertex indices)
		public ObjQuad[] Quads;                 // quads (4 vertex indices)
		int _vertexBufferId;                     // vertex buffer
		int _triangleBufferId;                   // triangle buffer
		int _quadBufferId;                       // quad buffer

		// constructor
		public Mesh( string fileName )
		{
			MeshLoader loader = new MeshLoader();
			loader.Load( this, fileName );
		}

		// initialization; called during first render
		public void Prepare( Shader shader )
		{
			if( _vertexBufferId == 0 )
			{
				// generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
				GL.GenBuffers( 1, out _vertexBufferId );
				GL.BindBuffer( BufferTarget.ArrayBuffer, _vertexBufferId );
				GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Marshal.SizeOf( typeof( ObjVertex ) )), Vertices, BufferUsageHint.StaticDraw );

				// generate triangle index array
				GL.GenBuffers( 1, out _triangleBufferId );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, _triangleBufferId );
				GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(Triangles.Length * Marshal.SizeOf( typeof( ObjTriangle ) )), Triangles, BufferUsageHint.StaticDraw );

				// generate quad index array
				GL.GenBuffers( 1, out _quadBufferId );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, _quadBufferId );
				GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(Quads.Length * Marshal.SizeOf( typeof( ObjQuad ) )), Quads, BufferUsageHint.StaticDraw );
			}
		}

		// render the mesh using the supplied shader and matrix
		public void Render( Shader shader, Matrix4 transform, Texture texture )
		{
			// on first run, prepare buffers
			Prepare( shader );

			// safety dance
			GL.PushClientAttrib( ClientAttribMask.ClientVertexArrayBit );

			// enable texture
			int texLoc = GL.GetUniformLocation( shader.ProgramId, "pixels" );
			GL.Uniform1( texLoc, 0 );
			GL.ActiveTexture( TextureUnit.Texture0 );
			GL.BindTexture( TextureTarget.Texture2D, texture.Id );

			// enable shader
			GL.UseProgram( shader.ProgramId );

			// pass transform to vertex shader
			GL.UniformMatrix4( shader.UniformMview, false, ref transform );
			
			GL.Uniform3(shader.UniformLightDir, Vector3.Normalize(new Vector3(1, -1, -0.5f)));

			// enable position, normal and uv attributes
			GL.EnableVertexAttribArray( shader.AttributeVpos );
			GL.EnableVertexAttribArray( shader.AttributeVnrm );
			GL.EnableVertexAttribArray( shader.AttributeVuvs );

			// bind interleaved vertex data
			GL.EnableClientState( ArrayCap.VertexArray );
			GL.BindBuffer( BufferTarget.ArrayBuffer, _vertexBufferId );
			GL.InterleavedArrays( InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf( typeof( ObjVertex ) ), IntPtr.Zero );

			// link vertex attributes to shader parameters 
			GL.VertexAttribPointer( shader.AttributeVuvs, 2, VertexAttribPointerType.Float, false, 32, 0 );
			GL.VertexAttribPointer( shader.AttributeVnrm, 3, VertexAttribPointerType.Float, true, 32, 2 * 4 );
			GL.VertexAttribPointer( shader.AttributeVpos, 3, VertexAttribPointerType.Float, false, 32, 5 * 4 );

			// bind triangle index data and render
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _triangleBufferId );
			GL.DrawArrays( PrimitiveType.Triangles, 0, Triangles.Length * 3 );

			// bind quad index data and render
			if( Quads.Length > 0 )
			{
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, _quadBufferId );
				GL.DrawArrays( PrimitiveType.Quads, 0, Quads.Length * 4 );
			}

			// restore previous OpenGL state
			GL.UseProgram( 0 );
			GL.PopClientAttrib();
		}

		// layout of a single vertex
		[StructLayout( LayoutKind.Sequential )]
		public struct ObjVertex
		{
			public Vector2 TexCoord;
			public Vector3 Normal;
			public Vector3 Vertex;
		}

		// layout of a single triangle
		[StructLayout( LayoutKind.Sequential )]
		public struct ObjTriangle
		{
			public int Index0, Index1, Index2;
		}

		// layout of a single quad
		[StructLayout( LayoutKind.Sequential )]
		public struct ObjQuad
		{
			public int Index0, Index1, Index2, Index3;
		}
	}
}