using OpenTK;

namespace Template
{
    /// <summary>
    /// Describes the transformation of a 3D object.
    /// </summary>
    public struct Transform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        /// <summary>
        /// Transform describing no transformation.
        /// </summary>
        public static readonly Transform Identity = new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One);

        /// <summary>
        /// The matrix corresponding to this transform.
        /// </summary>
        public Matrix4 Matrix => Matrix4.CreateScale(Scale) * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position);        
        
        public Transform(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }
        public Transform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = Vector3.One;
        }
        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}