using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputAction : IComponentData
    {
        public InputAction MoveAction;
        public InputAction CameraAction;
        public InputAction PickaxeAction;
    }

    public struct PlayerInputComponent : IComponentData
    {
        public float2 MoveValue;
        public float2 CameraValue;
        public bool PickaxeActivated;
    }

    public class PlayerInputActionAuthoring : MonoBehaviour
    {
        public InputAction MoveAction;
        public InputAction CameraAction;
        public InputAction PickaxeAction;
        
        public class PlayerInputActionBaker : Baker<PlayerInputActionAuthoring>
        {
            public override void Bake(PlayerInputActionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponentObject(entity, new PlayerInputAction
                {
                    MoveAction = authoring.MoveAction,
                    CameraAction = authoring.CameraAction,
                    PickaxeAction = authoring.PickaxeAction,
                });
                
                AddComponent<PlayerInputComponent>(entity);
            }
        }
    }
}