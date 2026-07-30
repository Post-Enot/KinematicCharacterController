using System;
using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    [Serializable]
    public struct CharacterControllerShape
    {
        public CharacterControllerShape(float height, float radius)
        {
            KCC_Utility.CalculateSafeCharacterDimensions(ref height, ref radius);
            Height = height;
            Radius = radius;
        }

        [field: SerializeField] public float Height { get; set; }
        [field: SerializeField] public float Radius { get; set; }

        public readonly float HalfHeight => Height * 0.5f;
    }
}
