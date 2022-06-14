using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Template
{
    public class Object3D
    {
        public Transform Transform;

        public Object3D Parent;

        public Matrix4 WorldMatrix => (Transform.Matrix * Parent?.WorldMatrix) ?? Transform.Matrix;

        private List<Object3D> _children = new List<Object3D>();

        public Mesh ObjectMesh;

        public Texture MainTexture;

        public virtual void Update()
        {
            foreach (var c in _children)
            {            
                c.Update();
            }   
        }

        public Object3D(Transform transform, Mesh objectMesh, Texture mainTexture)
        {
            Transform = transform;
            ObjectMesh = objectMesh;
            MainTexture = mainTexture;
        }

        public Object3D[] GetChildren()
        {
            return _children.ToArray();
        }

        public void AddChild(Object3D o)
        {
            _children.Add(o);
            o.Parent = this;
        }
        public virtual void Render(Shader shader)
        {
            var cm = Camera.Instance.GetCameraMatrix();
            ObjectMesh?.Render(shader, WorldMatrix * Camera.Instance.GetCameraMatrix() * Camera.Instance.GetProjectionMatrix(), MainTexture);
            foreach (var c in _children)
            {            
                c.Render(shader);
            }
        }
        
    }
}