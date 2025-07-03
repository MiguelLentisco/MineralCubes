using Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Player
{
    [UpdateAfter(typeof(PlayerInputSystem))]
    [UpdateBefore(typeof(MovementSystem))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (movement, input) in SystemAPI.Query<RefRW<MovementComponent>, RefRO<PlayerInputComponent>>())
            {
                movement.ValueRW.Direction = new float3(input.ValueRO.MoveValue.x, 0.0f, input.ValueRO.MoveValue.y);
                movement.ValueRW.Input = math.length(input.ValueRO.MoveValue);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}