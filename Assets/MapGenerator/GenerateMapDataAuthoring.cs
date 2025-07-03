using System.Collections.Generic;
using Mineral;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace MapGenerator
{
    public struct GenerateMapData : IComponentData
    {
        // Mineral cube data
        public struct GenerateMineralCubeData
        {
            public float InitialHardness;
            public float PerlinNoiseThreshold;
            public Color MineralColor;
        }
        
        public Entity BaseMineralCube;
        public FixedList512Bytes<GenerateMineralCubeData> MineralCubeData;
    
        // Map configuration
        public float CubeSize;
        public uint3 CubeCountSize;
        public float3 MapOrigin;

        // Perlin noise parameters
        public float Frequency;
        public float Amplitude;
        public float Persistence;
        public float Lacunarity;
        public uint Octaves;
    }

    public struct MineralCubesRuntimeData : IComponentData
    {
        public struct MineralCubeRuntimeData
        {
            public int MineralMinAmount;
            public int MineralMaxAmount;
        }
        
        public FixedList512Bytes<MineralCubeRuntimeData> MineralCubeData;
    }
    
    public class GenerateMapDataAuthoring : MonoBehaviour
    {
        public GameObject BaseMineralCube;
        public MineralDatabase MineralDB;
    
        // Map configuration
        [Min(0.0f)] public float CubeSize = 1;
        public uint3 CubeCountSize = new(100, 10, 50);
        public float3 MapOrigin = new(0, 0, 0);

        // Perlin noise parameters
        [Min(0.0f)] public float Amplitude = 1.0f;
        [Min(0.001f)] public float Frequency = 1.0f;
        [Range(0.0f, 1.0f)] public float Persistence = 0.5f;
        [Range(1.0f, 5.0f)] public float Lacunarity = 2.0f;
        [Range(1, 12)] public uint Octaves = 8;
    
        public class GenerateMapAuthoringBaker : Baker<GenerateMapDataAuthoring>
        {
            public override void Bake(GenerateMapDataAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                
                var mineralCubesData = authoring.MineralDB.MineralTypeData;
                var mineralCubesRuntimeData = new FixedList512Bytes<MineralCubesRuntimeData.MineralCubeRuntimeData>();
                var generateMineralCubesData = new FixedList512Bytes<GenerateMapData.GenerateMineralCubeData>();

                uint index = 0;
                foreach (var mineralCubeData in mineralCubesData)
                {
                    mineralCubesRuntimeData.Add(new MineralCubesRuntimeData.MineralCubeRuntimeData
                    {
                        MineralMinAmount = mineralCubeData.MineralMinAmount,
                        MineralMaxAmount = mineralCubeData.MineralMaxAmount,
                    });
                    generateMineralCubesData.Add(new GenerateMapData.GenerateMineralCubeData
                    {
                        InitialHardness = mineralCubeData.InitialHardness,
                        PerlinNoiseThreshold = mineralCubeData.PerlinNoiseThreshold,
                        MineralColor = mineralCubeData.MineralColor
                    });

                    ++index;
                }

                AddComponent(entity, new MineralCubesRuntimeData
                {
                    MineralCubeData = mineralCubesRuntimeData,
                });
                
                AddComponent(entity, new GenerateMapData
                {
                    BaseMineralCube = GetEntity(authoring.BaseMineralCube, TransformUsageFlags.Dynamic),
                    MineralCubeData = generateMineralCubesData,
                    CubeSize = authoring.CubeSize,
                    CubeCountSize = authoring.CubeCountSize,
                    MapOrigin = authoring.MapOrigin,
                    Frequency = authoring.Frequency,
                    Amplitude = authoring.Amplitude,
                    Persistence = authoring.Persistence,
                    Lacunarity = authoring.Lacunarity,
                    Octaves = authoring.Octaves,
                });
            }
        }
    }
}




