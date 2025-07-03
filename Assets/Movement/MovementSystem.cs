using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Movement
{
    public partial struct MovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MovementComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MovementComponent>>())
            {
                float sanitizedInput = math.clamp(math.abs(movement.ValueRO.Input), 0.0f, 1.0f);
                movement.ValueRW.Input = 0.0f;

                movement.ValueRW.Direction = math.normalizesafe(movement.ValueRO.Direction);
                float3 deltaMove = sanitizedInput * movement.ValueRO.Speed * movement.ValueRO.Direction;
                deltaMove -= math.up() * math.abs(movement.ValueRO.Gravity);
                deltaMove *= SystemAPI.Time.DeltaTime;

                transform.ValueRW.Position += deltaMove;

                if (math.lengthsq(movement.ValueRW.Direction) > 0.01f)
                {
                    transform.ValueRW.Rotation = quaternion.LookRotation(movement.ValueRO.Direction, math.up());
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}