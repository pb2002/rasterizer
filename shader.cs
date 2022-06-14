using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Template
{
	public class Shader
	{
		// data members
		public int ProgramId, VsId, FsId;
		public int AttributeVpos;
		public int AttributeVnrm;
		public int AttributeVuvs;
		public int UniformMview;
		public int UniformLightDir;

		// constructor
		public Shader( string vertexShader, string fragmentShader )
		{
			// compile shaders
			ProgramId = GL.CreateProgram();
			Load( vertexShader, ShaderType.VertexShader, ProgramId, out VsId );
			Load( fragmentShader, ShaderType.FragmentShader, ProgramId, out FsId );
			GL.LinkProgram( ProgramId );
			Console.WriteLine( GL.GetProgramInfoLog( ProgramId ) );

			// get locations of shader parameters
			AttributeVpos = GL.GetAttribLocation( ProgramId, "vPosition" );
			AttributeVnrm = GL.GetAttribLocation( ProgramId, "vNormal" );
			AttributeVuvs = GL.GetAttribLocation( ProgramId, "vUV" );
			UniformMview = GL.GetUniformLocation( ProgramId, "transform" );
			UniformLightDir = GL.GetUniformLocation( ProgramId, "lightDir" );
		}

		// loading shaders
		void Load( String filename, ShaderType type, int program, out int id )
		{
			// source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
			id = GL.CreateShader( type );
			using( StreamReader sr = new StreamReader( filename ) ) GL.ShaderSource( id, sr.ReadToEnd() );
			GL.CompileShader( id );
			GL.AttachShader( program, id );
			Console.WriteLine( GL.GetShaderInfoLog( id ) );
		}
	}
}
