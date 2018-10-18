using System;
using UnityEngine;

namespace FVTC.LearningInnovations.Scripts.Unity
{
    [Serializable]
    public struct SimplePose
    {
        public SimplePose(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        [SerializeField]
        public readonly Vector3 Position;

        [SerializeField]
        public readonly Quaternion Rotation;
        
        public static implicit operator SimplePose(Pose p)
        {
            return new SimplePose(p.position, p.rotation);
        }
    }
}
