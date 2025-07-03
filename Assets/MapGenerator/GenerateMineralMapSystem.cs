using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace MapGenerator
{
    partial struct GenerateMineralMapSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GenerateMapData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var generateMapDataConfig = SystemAPI.GetSingleton<GenerateMapData>();
            uint totalCubesCount = generateMapDataConfig.CubeCountSize.x * generateMapDataConfig.CubeCountSize.y * generateMapDataConfig.CubeCountSize.z;
            state.EntityManager.Instantiate(generateMapDataConfig.BaseMineralCube, (int)totalCubesCount, Allocator.Temp);

            state.Dependency = new GenerateMineralMapJob
            {
                MineralCubeData = generateMapDataConfig.MineralCubeData,
                CubeSize = generateMapDataConfig.CubeSize,
                CubeCountSize = (int3)generateMapDataConfig.CubeCountSize,
                MapOrigin = generateMapDataConfig.MapOrigin,
                Frequency = generateMapDataConfig.Frequency,
                Amplitude = generateMapDataConfig.Amplitude,
                Persistence = generateMapDataConfig.Persistence,
                Lacunarity = generateMapDataConfig.Lacunarity,
                Octaves = generateMapDataConfig.Octaves,
            }.ScheduleParallel(state.Dependency);
            
            // Destroy generate map data?
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }

    [BurstCompile]
    [WithAll(typeof(MineralCube))]
    public partial struct GenerateMineralMapJob : IJobEntity
    {
        // Mineral cube data
        [ReadOnly] public FixedList512Bytes<GenerateMapData.GenerateMineralCubeData> MineralCubeData;
        
        // Map configuration
        [ReadOnly] public float CubeSize;
        [ReadOnly] public int3 CubeCountSize;
        [ReadOnly] public float3 MapOrigin;

        // Perlin noise parameters
        [ReadOnly] public float Frequency;
        [ReadOnly] public float Amplitude;
        [ReadOnly] public float Persistence;
        [ReadOnly] public float Lacunarity;
        [ReadOnly] public uint Octaves;

        // Generate perlin noise [-1, 1] for a 3D pos
        private float Noise3D(float3 pos, float frequency, float amplitude, float lacunarity, float persistence, uint octaves)
        {
            float total = 0.0f;
            float maxValue = 0.0f;

            for (int i = 0; i < octaves; ++i)
            {
                // Offset the coordinates for each octave to introduce variation.
                float offset = i * 2.0f;
                // Get all permutations of noise for each individual axis
                float noiseVal = noise.cnoise(new float3(pos.x + offset, pos.y + offset, pos.z) * frequency) * amplitude;
                total += noiseVal;
            
                // Accumulate the maximum possible value for normalization purposes.
                maxValue += amplitude;

                // Adjust the amplitude and frequency for the next octave according to the persistence and lacunarity.
                amplitude *= persistence; // Persistence decreases amplitude with each octave, making higher octaves contribute less.
                frequency *= lacunarity; // Lacunarity increases frequency with each octave, adding more detail with each layer.
            }

            // Normalize the total noise value to be within [-1.0, 1.0] and return it.
            return total / maxValue;
        }
        
        private void Execute([EntityIndexInQuery] int entityInQueryIndex, Entity entity, ref LocalTransform transform,
            ref MineralCube mineralCube, ref URPMaterialPropertyBaseColor materialColor)
        {
            // Set position to mineral cube in the grid
            float3 offset = MapOrigin - CubeCountSize / 2;
            int3 cubeIndex = new int3(entityInQueryIndex / (CubeCountSize.y * CubeCountSize.z),
                (entityInQueryIndex / CubeCountSize.z) % CubeCountSize.y, entityInQueryIndex % CubeCountSize.z) + 1;
            float3 cubePos = (float3)cubeIndex * CubeSize + offset;
            transform.Position = cubePos;

            // Calc relative position (0, 1) and perlin noise, then multiply noise by center distance
            float3 cubePosNormal = cubeIndex / (float3)CubeCountSize;
            float noiseValue = Noise3D(cubePosNormal, Frequency, Amplitude, Lacunarity, Persistence, Octaves);
            float relativeDist = math.length(cubePos - MapOrigin) / math.cmax(CubeCountSize);
            noiseValue *= 1.0f + relativeDist;
            noiseValue = math.clamp(noiseValue, -1.0f, 1.0f);

            // Calculate mineral type based on noise value and setup data
            int index = 0;
            foreach (var mineralCubeData in MineralCubeData)
            {
                if (noiseValue > mineralCubeData.PerlinNoiseThreshold)
                {
                    ++index;
                }
                else
                {
                    mineralCube.MineralType = index;
                    mineralCube.CurrentHardness = MineralCubeData[index].InitialHardness;
                    var color = MineralCubeData[index].MineralColor;
                    materialColor.Value = new float4(color.r, color.g, color.b, color.a);
                }
            }
        }
    }
}