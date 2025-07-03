using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Movement
{
    public struct MovementComponent : IComponentData
    {
        public float Speed;
        public float3 Direction;
        public float Input;

        public float Gravity;
    }

    public class MovementAuthoring : MonoBehaviour
    {
        [Min(0.0f)] public float Speed;
        public bool GravityEnabled = true;

        public class MovementComponentBaker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MovementComponent
                {
                    Speed = authoring.Speed,
                    Gravity = authoring.GravityEnabled ? 9.8f : 0.0f,
                    Direction = float3.zero
                });
            }
        }
    }
}