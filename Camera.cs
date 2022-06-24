using System;
using OpenTK;

namespace Template
{
    public class Camera
    {
        private static Camera _instance;
        public static Camera Instance => _instance ?? throw new Exception("Singleton was not initialized.");

        public Camera(Transform transform, int screenWidth, int screenHeight)
        {
            Transform = transform;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public static void SetAsCurrent(Camera c)
        {
            _instance = c;
        }

        public Transform Transform;
        
        public float Fov = 1.2f;
        
        public int ScreenWidth, ScreenHeight;
        
        public float Near = 0.1f;
        public float Far = 1000f;

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(Fov, (float) ScreenWidth / ScreenHeight, Near, Far);
        }

        public Vector3 Forward => (Matrix4.CreateFromQuaternion(Transform.Rotation) * new Vector4(-Vector3.UnitZ, 0)).Xyz;
        public Vector3 Right => Vector3.Cross(Forward, Vector3.UnitY).Normalized();        
      
        public Matrix4 GetCameraMatrix()
        {
            var t = Matrix4.CreateTranslation(-Transform.Position);
            var r = Matrix4.CreateFromQuaternion(Transform.Rotation);
            return t * r;
        }
        
    }
}