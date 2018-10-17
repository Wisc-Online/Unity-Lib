using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    [RequireComponent(typeof(Rigidbody))]
    public class InputManagerForce : InputManagerAxesBase
    {
        [HideInInspector]
        public Rigidbody RigidBody { get; private set; }

        public float? MaxSpeed
        {
            get
            {
                return _maxSpeed;
            }

            set
            {
                _maxSpeed = value;
            }
        }

        [SerializeField]
        private float? _maxSpeed;

        private void Start()
        {
            this.RigidBody = GetComponent<Rigidbody>();
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

                var currentRotation = this.transform.rotation.eulerAngles;

                this.transform.rotation = Quaternion.Euler(currentRotation + rotation);
            }
        }

        private void UpdateMovement()
        {
            Vector3 force = new Vector3(GetAxis(MoveRightLeftAxisName), GetAxis(MoveUpDownAxisName), GetAxis(MoveForwardBackwardAxisName));

            if (force.sqrMagnitude > 0)
            {
                Vector3 forward, up, right;


                forward = this.transform.forward;
                right = this.transform.right;
                up = this.transform.up;

                if (LockMovementToXZPlane)
                {
                    forward = new Vector3(forward.x, 0, forward.y);
                    up = Vector3.up;
                }

                forward = forward.normalized;
                right = Vector3.Cross(forward.normalized, up.normalized);

                force = (right * force.x) + (up * force.y) + (forward * force.z);

                force = Vector3.Scale(force, MovementScale);

                this.RigidBody.AddForce(force);
            }
        }
    }
}