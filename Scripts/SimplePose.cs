using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FVTC.LearningInnovations.Scripts.Unity
{
    public struct SimplePose
    {
        public SimplePose(Vector3 position, Quaternion rotation)
        {
            this._position = position;
            this._rotation = rotation;
        }

        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private Quaternion _rotation;
        
        public Vector3 Position { get { return _position; } }
        public Quaternion Rotation { get { return _rotation; } }

        //  User-defined conversion from double to Digit
        public static implicit operator SimplePose(Pose p)
        {
            return new SimplePose(p.position, p.rotation);
        }
    }
}
