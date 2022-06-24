using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface Screen;                     // background surface for printing etc.
		private Object3D _sceneGraph;
		Stopwatch _timer;                          // timer for measuring frame duration
		Shader _shader;                            // shader to use for rendering
		Shader _postproc;                          // shader to use for post processing
		RenderTarget _target;                      // intermediate render target
		ScreenQuad _quad;                          // screen filling quad for post processing
		bool _useRenderTarget = true;

		MeshObject _teapot, _teapot2;

		private float _frameDuration;
		int lastMouseX = 0, lastMouseY = 0;	

		// initialize
		public void Init()
		{
			// load teapot
			var teapotMesh = new Mesh( "../../assets/teapot.obj" );
			var floorMesh = new Mesh( "../../assets/floor.obj" );

			// model from https://free3d.com/3d-model/intergalactic-spaceship-in-blender-28-eevee-394046.html
			var spaceshipMesh = new Mesh("../../assets/spaceship.obj");

			_shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
			_postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");

			// textures from https://drive.google.com/drive/folders/1yiku3561M7zgQAsc82QHJVV3ELmDgbAw
			var woodMat = new Material(
				Vector3.One,
				0.75f,
				albedoMap:   new Texture("../../assets/wood.jpg"),
				normalMap:   new Texture("../../assets/wood_n.jpg"),
				specularMap: new Texture("../../assets/wood_s.jpg")
			);

			var woodMat2 = new Material(
				new Vector3(1.0f, 0.4f, 0.4f),
				0.75f,
				albedoMap: new Texture("../../assets/wood.jpg"),
				normalMap: new Texture("../../assets/wood_n.jpg"),
				specularMap: new Texture("../../assets/wood_s.jpg")
			);

			// textures from https://drive.google.com/drive/folders/1fGI6KVYBTKqJrCf3Ot1WPVhcx3GP0cRm
			var cobbleMat = new Material(
				Vector3.One,
				1f,
				albedoMap:   new Texture("../../assets/cobble_a.jpg"),
				normalMap:   new Texture("../../assets/cobble_n.jpg"),
				specularMap: new Texture("../../assets/cobble_s.jpg")				
			);

			var plasticMat = new Material(
				new Vector3(0.2f, 0.22f, 0.25f),
				0.3f,
				albedoMap:   new Texture("../../assets/plastic_a.jpg"),
				normalMap:   new Texture("../../assets/plastic_n.jpg"),
				specularMap: new Texture("../../assets/plastic_s.jpg"),

				normalStrength: 0.5f
			);
			
			_sceneGraph = new Object3D(Transform.Identity);
			_sceneGraph.AddChild(new MeshObject(
				new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(8f)),
				floorMesh,
				cobbleMat
			));

			_teapot = new MeshObject(
				new Transform(new Vector3(0, -15, 0)),
				teapotMesh,
				woodMat
			);
			_sceneGraph.AddChild(_teapot);
			
			var spaceship = new MeshObject(
				new Transform(new Vector3(25, 5, 0), Quaternion.FromEulerAngles(0,0,35)),
				spaceshipMesh,
				plasticMat
			);
			_teapot2 = new MeshObject(
				new Transform(new Vector3(-25, 5, 0), Quaternion.Identity, new Vector3(0.5f)),
				teapotMesh,
				woodMat2
			);

			_teapot.AddChild(spaceship);
			_teapot.AddChild(_teapot2);

			// initialize stopwatch
			_timer = new Stopwatch();
			_timer.Reset();
			_timer.Start();			
			// create the render target
			
			//_target.lut = cobbleMat.AlbedoMap;
			var cam = new Camera(
				new Transform(new Vector3(0, 8.5f, 8.5f), Quaternion.FromAxisAngle(new Vector3(1,0,0), (float)Math.PI / 4)), 
				Screen.Width, Screen.Height
			);
			Camera.SetAsCurrent(cam);
			
			_quad = new ScreenQuad();
			_target = new RenderTarget(Screen.Width, Screen.Height, "../../assets/colorLut.png");
		}

		public void Tick()
		{
			_frameDuration = _timer.ElapsedMilliseconds;
			_timer.Reset();
			_timer.Start();
			

			_teapot.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -0.0003f * _frameDuration);
			_teapot2.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -0.0007f * _frameDuration);

			CameraMovement();
		}

		public void CameraMovement()
		{
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();
			if (keyboard.IsKeyDown(Key.W))
			{
				Camera.Instance.Transform.Position += Camera.Instance.Forward * _frameDuration * 0.02f;
			}
			if (keyboard.IsKeyDown(Key.S))
			{
				Camera.Instance.Transform.Position -= Camera.Instance.Forward * _frameDuration * 0.02f;
			}
			if (keyboard.IsKeyDown(Key.A))
			{
				Camera.Instance.Transform.Position -= Camera.Instance.Right * _frameDuration * 0.02f;
			}
			if (keyboard.IsKeyDown(Key.D))
			{
				Camera.Instance.Transform.Position += Camera.Instance.Right * _frameDuration * 0.02f;
			}

			// skip first frame
			if (lastMouseX == 0)
            {
				lastMouseX = mouse.X;
				lastMouseY = mouse.Y;
            }
			else
            {
				var deltaX = mouse.X - lastMouseX;
				var deltaY = mouse.Y - lastMouseY;
				lastMouseX = mouse.X;
				lastMouseY = mouse.Y;
				if ((deltaY < 0 && Camera.Instance.Forward.Y < 0.95) || (deltaY > 0 && Camera.Instance.Forward.Y > -0.95))
                {
					Camera.Instance.Transform.Rotation *=
					Quaternion.FromAxisAngle(Camera.Instance.Right, deltaY * _frameDuration * 0.0002f);
				}
				Camera.Instance.Transform.Rotation *=
					Quaternion.FromAxisAngle(Vector3.UnitY, deltaX * _frameDuration * 0.0002f);
			}
		}
		
		// tick for OpenGL rendering code
		public void RenderGL()
		{
			
			if( _useRenderTarget )
			{
				// enable render target
				_target.Bind();
				
				_shader.SetUniformMatrix4("view", Camera.Instance.GetCameraMatrix());
				_shader.SetUniformMatrix4("projection", Camera.Instance.GetProjectionMatrix());
				Settings.Upload(_shader);

				_shader.Use();
				_sceneGraph.Render(_shader);

				// render quad
				_target.Unbind();
				_quad.Render( _postproc, _target.GetTextureId(), _target.GetColorLutId() );

				GL.UseProgram(0);
			}
			else
			{
				// render scene directly to the screen
				_sceneGraph.Render(_shader);
			}
		}
	}
}