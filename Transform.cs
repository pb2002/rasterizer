using OpenTK;

namespace Template
{
    public struct Transform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public static readonly Transform Identity = new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One);

        public Matrix4 Matrix => Matrix4.CreateScale(Scale) * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position);

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }
    }
}