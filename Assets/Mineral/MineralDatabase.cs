using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mineral
{
    [CreateAssetMenu(fileName = "MineralDatabase", menuName = "ScriptableObjects/MineralDatabase")]
    public class MineralDatabase : ScriptableObject
    {
       public List<MineralCubeData> MineralTypeData;

        [Serializable]
        public struct MineralCubeData
        {
            public MineralType Type;
            [Min(0.0f)] public float InitialHardness;
            [Min(0)] public int MineralMinAmount;
            [Min(1)] public int MineralMaxAmount;
            [Range(-1.0f, 1.0f)] public float PerlinNoiseThreshold;
            public Color MineralColor;
        }
    }
}