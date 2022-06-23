using System;
using System.Diagnostics;
using OpenTK;
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

		private Mesh _mesh, _floor;

		private float _frameDuration;

		// initialize
		public void Init()
		{
			// load teapot
			_mesh = new Mesh( "../../assets/teapot.obj" );
			_floor = new Mesh( "../../assets/floor.obj" );

			_shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );

			// textures from https://drive.google.com/drive/folders/1yiku3561M7zgQAsc82QHJVV3ELmDgbAw

			var woodMat = new Material(
				new Texture("../../assets/wood.jpg"),
				new Texture("../../assets/wood_n.jpg"),
				new Texture("../../assets/wood_s.jpg"),
				Vector3.One,
				0.4f
			);
			var cobbleMat = new Material(
				new Texture("../../assets/cobble_a.jpg"),
				new Texture("../../assets/cobble_n.jpg"),
				new Texture("../../assets/cobble_s.jpg"),
				Vector3.One,
				0.3f
			);
			
			_sceneGraph = new Object3D(Transform.Identity, null, null);
			_sceneGraph.AddChild(new Object3D(
				new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(0.5f)),
				_mesh,
				woodMat
			));

			_sceneGraph.AddChild(new Object3D(
				new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(4f)),
				_floor,
				cobbleMat
			));

			// initialize stopwatch
			_timer = new Stopwatch();
			_timer.Reset();
			_timer.Start();
			// create shaders
			_postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
			// load a texture
			// create the render target
			_target = new RenderTarget( Screen.Width, Screen.Height, "../../assets/colorLut.png" );
			//_target.lut = cobbleMat.AlbedoMap;
			var cam = new Camera(
				new Transform(new Vector3(0, 8.5f, 8.5f), Quaternion.FromAxisAngle(new Vector3(1,0,0), (float)Math.PI / 4), Vector3.Zero), 
				Screen.Width, Screen.Height
			);
			Camera.SetAsCurrent(cam);
			
			_quad = new ScreenQuad();
			
		}

		// tick for background surface
		public void Tick()
		{
			_frameDuration = _timer.ElapsedMilliseconds;
			_timer.Reset();
			_timer.Start();

			// _sceneGraph.Transform.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), 0.001f * _frameDuration);
			
			CameraMovement();
		}

		public void CameraMovement()
		{
			KeyboardState keyboard = Keyboard.GetState();

			if (keyboard.IsKeyDown(Key.W))
			{
				Camera.Instance.Transform.Position += Camera.Instance.ViewDirection * _frameDuration * 0.006f;
			}
			if (keyboard.IsKeyDown(Key.S))
			{
				Camera.Instance.Transform.Position -= Camera.Instance.ViewDirection * _frameDuration * 0.01f;
			}
			if (keyboard.IsKeyDown(Key.A))
			{
				Camera.Instance.Transform.Position -= Camera.Instance.Right * _frameDuration * 0.01f;
			}
			if (keyboard.IsKeyDown(Key.D))
			{
				Camera.Instance.Transform.Position += Camera.Instance.Right * _frameDuration * 0.01f;
			}

			if (keyboard.IsKeyDown(Key.Up))
			{
				Camera.Instance.Transform.Rotation *=
					Quaternion.FromAxisAngle(Camera.Instance.Right, -_frameDuration * 0.0007f);
			}
			if (keyboard.IsKeyDown(Key.Down))
			{
				Camera.Instance.Transform.Rotation *=
					Quaternion.FromAxisAngle(Camera.Instance.Right, _frameDuration * 0.0007f);
			}
			if (keyboard.IsKeyDown(Key.Left))
			{
				Camera.Instance.Transform.Rotation *=
					Quaternion.FromAxisAngle(Vector3.UnitY, -_frameDuration * 0.0009f);	
			}
			if (keyboard.IsKeyDown(Key.Right))
			{
				Camera.Instance.Transform.Rotation *=
					Quaternion.FromAxisAngle(Vector3.UnitY, _frameDuration * 0.0009f);
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
				_sceneGraph.Render(_shader);

				// render quad
				_target.Unbind();
				_quad.Render( _postproc, _target.GetTextureId(), _target.GetColorLutId() );
			}
			else
			{
				// render scene directly to the screen
				_sceneGraph.Render(_shader);
			}
		}
	}
}