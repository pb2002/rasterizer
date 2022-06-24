using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    internal class MeshObject : Object3D
    {
        public Mesh ObjectMesh;

        public Material Mat;

        public MeshObject(Transform transform, Mesh objectMesh, Material material) : base(transform)
        {
            ObjectMesh = objectMesh;
            Mat = material;            
        }
        // 
        public override void Render(Shader shader)
        {
            Mat?.Upload(shader);
            shader.SetUniformMatrix4("model", WorldMatrix);
            ObjectMesh?.Render(shader);

            base.Render(shader);
        }
    }
}
