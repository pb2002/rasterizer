using System.IO;
using System.Collections.Generic;
using OpenTK;

namespace Template
{
	// mesh and loader based on work by JTalton; http://www.opentk.com/node/642

	public class MeshLoader
	{
		public bool Load( Mesh mesh, string fileName )
		{
			try
			{
				using( StreamReader streamReader = new StreamReader( fileName ) )
				{
					Load( mesh, streamReader );
					streamReader.Close();
					return true;
				}
			}
			catch { return false; }
		}

		char[] _splitCharacters = new char[] { ' ' };

		List<Vector3> _vertices;
		List<Vector3> _normals;
		List<Vector2> _texCoords;
		List<Mesh.ObjVertex> _objVertices;
		List<Mesh.ObjTriangle> _objTriangles;
		List<Mesh.ObjQuad> _objQuads;

		void Load( Mesh mesh, TextReader textReader )
		{
			_vertices = new List<Vector3>();
			_normals = new List<Vector3>();
			_texCoords = new List<Vector2>();
			_objVertices = new List<Mesh.ObjVertex>();
			_objTriangles = new List<Mesh.ObjTriangle>();
			_objQuads = new List<Mesh.ObjQuad>();
			string line;
			while( (line = textReader.ReadLine()) != null )
			{
				line = line.Trim( _splitCharacters );
				line = line.Replace( "  ", " " );
				string[] parameters = line.Split( _splitCharacters );
				switch( parameters[0] )
				{
				case "p": // point
					break;
				case "v": // vertex
					float x = float.Parse( parameters[1] );
					float y = float.Parse( parameters[2] );
					float z = float.Parse( parameters[3] );
					_vertices.Add( new Vector3( x, y, z ) );
					break;
				case "vt": // texCoord
					float u = float.Parse( parameters[1] );
					float v = float.Parse( parameters[2] );
					_texCoords.Add( new Vector2( u, v ) );
					break;
				case "vn": // normal
					float nx = float.Parse( parameters[1] );
					float ny = float.Parse( parameters[2] );
					float nz = float.Parse( parameters[3] );
					_normals.Add( new Vector3( nx, ny, nz ) );
					break;
				case "f":
					switch( parameters.Length )
					{
					case 4:
						Mesh.ObjTriangle objTriangle = new Mesh.ObjTriangle();
						objTriangle.Index0 = ParseFaceParameter( parameters[1] );
						objTriangle.Index1 = ParseFaceParameter( parameters[2] );
						objTriangle.Index2 = ParseFaceParameter( parameters[3] );
						_objTriangles.Add( objTriangle );
						break;
					case 5:
						Mesh.ObjQuad objQuad = new Mesh.ObjQuad();
						objQuad.Index0 = ParseFaceParameter( parameters[1] );
						objQuad.Index1 = ParseFaceParameter( parameters[2] );
						objQuad.Index2 = ParseFaceParameter( parameters[3] );
						objQuad.Index3 = ParseFaceParameter( parameters[4] );
						_objQuads.Add( objQuad );
						break;
					}
					break;
				}
			}
			mesh.Vertices = _objVertices.ToArray();
			mesh.Triangles = _objTriangles.ToArray();
			mesh.Quads = _objQuads.ToArray();
			_vertices = null;
			_normals = null;
			_texCoords = null;
			_objVertices = null;
			_objTriangles = null;
			_objQuads = null;
		}

		char[] _faceParamaterSplitter = new char[] { '/' };
		int ParseFaceParameter( string faceParameter )
		{
			Vector3 vertex = new Vector3();
			Vector2 texCoord = new Vector2();
			Vector3 normal = new Vector3();
			string[] parameters = faceParameter.Split( _faceParamaterSplitter );
			int vertexIndex = int.Parse( parameters[0] );
			if( vertexIndex < 0 ) vertexIndex = _vertices.Count + vertexIndex;
			else vertexIndex = vertexIndex - 1;
			vertex = _vertices[vertexIndex];
			if( parameters.Length > 1 ) if( parameters[1] != "" )
				{
					int texCoordIndex = int.Parse( parameters[1] );
					if( texCoordIndex < 0 ) texCoordIndex = _texCoords.Count + texCoordIndex;
					else texCoordIndex = texCoordIndex - 1;
					texCoord = _texCoords[texCoordIndex];
				}
			if( parameters.Length > 2 )
			{
				int normalIndex = int.Parse( parameters[2] );
				if( normalIndex < 0 ) normalIndex = _normals.Count + normalIndex;
				else normalIndex = normalIndex - 1;
				normal = _normals[normalIndex];
			}
			return AddObjVertex( ref vertex, ref texCoord, ref normal );
		}

		int AddObjVertex( ref Vector3 vertex, ref Vector2 texCoord, ref Vector3 normal )
		{
			Mesh.ObjVertex newObjVertex = new Mesh.ObjVertex();
			newObjVertex.Vertex = vertex;
			newObjVertex.TexCoord = texCoord;
			newObjVertex.Normal = normal;
			_objVertices.Add( newObjVertex );
			return _objVertices.Count - 1;
		}
	}
}
