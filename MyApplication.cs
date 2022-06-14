using System.Diagnostics;
using OpenTK;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface Screen;                  // background surface for printing etc.
		private Object3D _sceneGraph;
		const float Pi = 3.1415926535f;         // PI
		float _a = 0;                            // teapot rotation angle
		Stopwatch _timer;                        // timer for measuring frame duration
		Shader _shader;                          // shader to use for rendering
		Shader _postproc;                        // shader to use for post processing
		Texture _wood;                           // texture to use for rendering
		RenderTarget _target;                    // intermediate render target
		ScreenQuad _quad;                        // screen filling quad for post processing
		bool _useRenderTarget = true;

		private Mesh _mesh, _floor;
		// initialize
		public void Init()
		{
			// load teapot
			_mesh = new Mesh( "../../assets/teapot.obj" );
			_floor = new Mesh( "../../assets/floor.obj" );

			_shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
			_wood = new Texture( "../../assets/wood.jpg" );
			
			_sceneGraph = new Object3D(Transform.Identity, null, null);
			_sceneGraph.AddChild(new Object3D(
				new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(0.5f)),
				_mesh,
				_wood
			));

			_sceneGraph.AddChild(new Object3D(
				new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(4f)),
				_floor,
				_wood
			));

			// initialize stopwatch
			_timer = new Stopwatch();
			_timer.Reset();
			_timer.Start();
			// create shaders
			_postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
			// load a texture
			// create the render target
			_target = new RenderTarget( Screen.Width, Screen.Height );
			
			var cam = new Camera(
				new Transform(new Vector3(0, 14.5f, 0), Quaternion.FromAxisAngle(new Vector3(1,0,0), Pi / 2), Vector3.Zero), 
				Screen.Width, Screen.Height
			);
			Camera.SetAsCurrent(cam);
			
			_quad = new ScreenQuad();
		}

		// tick for background surface
		public void Tick()
		{
		}

		// tick for OpenGL rendering code
		public void RenderGL()
		{
			// measure frame duration
			float frameDuration = _timer.ElapsedMilliseconds;
			_timer.Reset();
			_timer.Start();

			// prepare matrix for vertex shader

			// update rotation
			_a += 0.001f * frameDuration;
			if( _a > 2 * Pi ) _a -= 2 * Pi;

			if( _useRenderTarget )
			{
				// enable render target
				_target.Bind();

				_sceneGraph.Render(_shader);

				// render quad
				_target.Unbind();
				_quad.Render( _postproc, _target.GetTextureId() );
			}
			else
			{
				// render scene directly to the screen
				_sceneGraph.Render(_shader);
			}
		}
	}
}