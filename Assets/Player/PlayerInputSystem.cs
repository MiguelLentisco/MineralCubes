using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public partial struct PlayerInputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            // Hack to enable input for the moment
            var playerInputAction = SystemAPI.ManagedAPI.GetSingleton<PlayerInputAction>();
            playerInputAction.MoveAction.Enable();
            playerInputAction.CameraAction.Enable();
            playerInputAction.PickaxeAction.Enable();
            
            // Read values onto unmanaged component
            ref var playerInput = ref SystemAPI.GetSingletonRW<PlayerInputComponent>().ValueRW;
            playerInput.MoveValue = playerInputAction.MoveAction.ReadValue<Vector2>();
            playerInput.CameraValue = playerInputAction.CameraAction.ReadValue<Vector2>();
            playerInput.PickaxeActivated = playerInputAction.PickaxeAction.ReadValue<bool>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}