using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    public abstract class InputManagerAxesBase : MonoBehaviour
    {
        // disable never-assigned-to warnings (they get assigned in Unity)
#pragma warning disable 0649
        [Header("Movement")]
        [SerializeField]
        private string _moveForwardBackwardAxisName;

        [SerializeField]
        private string _moveRightLeftAxisName;

        [SerializeField]
        private string _moveUpDownAxisName;

        [SerializeField]
        private Vector3 _movementScale = Vector3.one;

        [SerializeField]
        private bool _lockMovementToXZPlane;

        [Header("Rotation")]
        [SerializeField]
        private string _rotateXAxisName;

        [SerializeField]
        private string _rotateYAxisName;

        [SerializeField]
        private string _rotateZAxisName;


        [SerializeField]
        private Vector3 _rotationScale = Vector3.one;

        // restore never-assigned-to warnings (they get assigned in Unity)
#pragma warning restore 0649

        public string MoveForwardBackwardAxisName
        {
            get
            {
                return _moveForwardBackwardAxisName;
            }
        }

        public string MoveRightLeftAxisName
        {
            get
            {
                return _moveRightLeftAxisName;
            }
        }



        public string RotateXAxisName { get { return _rotateXAxisName; } }

        public string RotateYAxisName { get { return _rotateYAxisName; } }

        public string RotateZAxisName { get { return _rotateZAxisName; } }

        public bool LockMovementToXZPlane { get { return _lockMovementToXZPlane; } }

        public Vector3 MovementScale
        {
            get
            {
                return _movementScale;
            }

            set
            {
                _movementScale = value;
            }
        }

        public Vector3 RotationScale
        {
            get
            {
                return _rotationScale;
            }

            set
            {
                _rotationScale = value;
            }
        }

        public string MoveUpDownAxisName
        {
            get
            {
                return _moveUpDownAxisName;
            }

            set
            {
                _moveUpDownAxisName = value;
            }
        }

        protected float GetAxis(string axisName)
        {
            if (string.IsNullOrWhiteSpace(axisName))
                return 0;

            return UnityEngine.Input.GetAxis(axisName);
        }
    }
}
