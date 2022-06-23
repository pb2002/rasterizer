using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

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

			// https://www.youtube.com/watch?v=hOLLh80hDmw&ab_channel=BrianWill
			// calculate tangent and bitangent vectors (used for normal mapping)
			foreach (var t in Triangles)
            {
				Vector3 tangent, bitangent;
				
				ObjVertex v1 = Vertices[t.Index0];
				ObjVertex v2 = Vertices[t.Index1];
				ObjVertex v3 = Vertices[t.Index2];


				//  \[T]/  DARK MAGIC, DO NOT EDIT  \[T]/  -------------------------

				Vector3 edge1 = v2.Vertex - v1.Vertex;
				Vector3 edge2 = v3.Vertex - v1.Vertex;

				Vector2 deltaUV1 = v2.TexCoord - v1.TexCoord;
				Vector2 deltaUV2 = v3.TexCoord - v1.TexCoord;

				float f = 1f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

				tangent.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
				tangent.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
				tangent.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);
				tangent.Normalize();

				bitangent.X = f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X);
				bitangent.Y = f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y);
				bitangent.Z = f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z);
				bitangent.Normalize();
				
				// -----------------------------------------------------------------

				Vertices[t.Index0].Tangent = tangent;
				Vertices[t.Index1].Tangent = tangent;
				Vertices[t.Index2].Tangent = tangent;

				Vertices[t.Index0].Bitangent = bitangent;
				Vertices[t.Index1].Bitangent = bitangent;
				Vertices[t.Index2].Bitangent = bitangent;
			}
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
		public void Render( Shader shader )
		{
			// on first run, prepare buffers
			Prepare( shader );
			// bind interleaved vertex data
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);

			GL.EnableVertexAttribArray(shader.AttributeVpos);			
			GL.VertexAttribPointer(shader.AttributeVpos, 3, VertexAttribPointerType.Float, false, 56, 0);

			GL.EnableVertexAttribArray(shader.AttributeVnrm);
			GL.VertexAttribPointer(shader.AttributeVnrm, 3, VertexAttribPointerType.Float, true, 56, 12);

			// link vertex attributes to shader parameters 
			GL.EnableVertexAttribArray(shader.AttributeVuvs);
			GL.VertexAttribPointer(shader.AttributeVuvs, 2, VertexAttribPointerType.Float, false, 56, 24);

			GL.EnableVertexAttribArray(shader.AttributeVtng);
			GL.VertexAttribPointer(shader.AttributeVtng, 3, VertexAttribPointerType.Float, false, 56, 32);

			GL.EnableVertexAttribArray(shader.AttributeVbtg);
			GL.VertexAttribPointer(shader.AttributeVbtg, 3, VertexAttribPointerType.Float, false, 56, 44);

			
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
		}

		// layout of a single vertex
		[StructLayout( LayoutKind.Sequential )]
		public struct ObjVertex
		{
			public Vector3 Vertex;
			public Vector3 Normal;
			public Vector2 TexCoord;		
			public Vector3 Tangent;
			public Vector3 Bitangent;			
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