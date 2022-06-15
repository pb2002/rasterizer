using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Template
{
	public class Shader
	{
		// data members
		public int ProgramId, VsId, FsId;
		public int AttributeVpos;
		public int AttributeVnrm;
		public int AttributeVuvs;
		public int AttributeVtng;
		public int AttributeVbtg;

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
			AttributeVtng = GL.GetAttribLocation( ProgramId, "vTangent");
			AttributeVbtg = GL.GetAttribLocation( ProgramId, "vBitangent");
		}

		public void SetUniformFloat(string name, float value)
        {
			var loc = GL.GetUniformLocation(ProgramId, name);
			GL.ProgramUniform1(ProgramId, loc, value);
		}
		public void SetUniformInt(string name, int value)
		{
			var loc = GL.GetUniformLocation(ProgramId, name);
			GL.ProgramUniform1(ProgramId, loc, value);
		}		
		public void SetUniformVector2(string name, Vector2 value)
		{
			var loc = GL.GetUniformLocation(ProgramId, name);
			GL.ProgramUniform2(ProgramId, loc, value);
		}
		public void SetUniformVector3(string name, Vector3 value)
		{
			var loc = GL.GetUniformLocation(ProgramId, name);
			GL.ProgramUniform3(ProgramId, loc, value);
		}
		public void SetUniformVector4(string name, Vector4 value)
        {
			var loc = GL.GetUniformLocation(ProgramId, name);
			GL.ProgramUniform4(ProgramId, loc, value);
		}
		public void SetUniformMatrix4(string name, Matrix4 value)
        {
            var loc = GL.GetUniformLocation(ProgramId, name);
			GL.ProgramUniformMatrix4(ProgramId, loc, false, ref value);
		}

        // loading shaders
        void Load( String filename, ShaderType type, int program, out int id )
		{
			// source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
			id = GL.CreateShader( type );
			using (StreamReader sr = new StreamReader(filename))
			{
				GL.ShaderSource( id, sr.ReadToEnd() );
			}
			GL.CompileShader( id );
			GL.AttachShader( program, id );
			Console.WriteLine( GL.GetShaderInfoLog( id ) );
		}
	}
}
