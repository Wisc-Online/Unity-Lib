using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    public abstract class MonoBehaviourBase : MonoBehaviour, IBehaviour
    {
        private void Awake()
        {
            OnAwake();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void Start()
        {
            OnStart();
        }

        public virtual void OnAwake()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnLateUpdate()
        {
            
        }

        public virtual void OnStart()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }
    }
}
