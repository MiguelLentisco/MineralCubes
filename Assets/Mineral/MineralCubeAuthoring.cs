using Unity.Entities;
using UnityEngine;

namespace Mineral
{
    public struct MineralCube : IComponentData
    {
        public float CurrentHardness;
        public int MineralType;
    }

    public class MineralCubeAuthoring : MonoBehaviour
    {
        public class MineralCubeBaker : Baker<MineralCubeAuthoring>
        {
            public override void Bake(MineralCubeAuthoring cubeAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MineralCube>(entity);
            }
        }
    }
}