using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    public class InputManagerMovement : InputManagerAxesBase
    {
        [SerializeField]
        private GameObject _target;

        public GameObject Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }


        private void Update()
        {
            UpdateMovement();

            UpdateRotation();
        }

        private void UpdateRotation()
        {

            Vector3 rotation = new Vector3(GetAxis(RotateXAxisName), GetAxis(RotateYAxisName), GetAxis(RotateZAxisName));

            if (rotation.sqrMagnitude > 0)
            {
                rotation = Vector3.Scale(rotation, RotationScale);

                var target = GetTarget();

                var currentRotation = target.transform.rotation.eulerAngles;

                target.transform.rotation = Quaternion.Euler(currentRotation + rotation);

            }
        }

        private void UpdateMovement()
        {
            Vector3 movementDelta = new Vector3(GetAxis(MoveRightLeftAxisName), GetAxis(MoveUpDownAxisName), GetAxis(MoveForwardBackwardAxisName));

            if (movementDelta.sqrMagnitude > 0)
            {
                movementDelta = Vector3.Scale(movementDelta, MovementScale);

                var target = GetTarget();

                Transform targetTransform = target.transform;

                if (LockMovementToXZPlane)
                {
                    Vector3 forward = new Vector3(targetTransform.forward.x, 0, targetTransform.forward.z).normalized;
                    Vector3 right = new Vector3(targetTransform.right.x, 0, targetTransform.right.z).normalized;

                    targetTransform.position += (right * movementDelta.x);
                    targetTransform.position += (forward * movementDelta.z);
                }
                else
                {
                    targetTransform.Translate(movementDelta);
                }
            }
        }
        
        GameObject GetTarget()
        {
            return this.Target ?? this.gameObject;
        }
    }
}