using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace FVTC.LearningInnovations.Unity.Networking
{
    public abstract class NetworkBahaviourBase : NetworkBehaviour , IBehaviour
    {


        public virtual void Awake()
        {
            OnAwake();
        }

        void Start()
        {
            OnStart();
        }

        void FixedUpdate()
        {
            OnFixedUpdate();
        }

        void Update()
        {
            OnUpdate();
        }

        void LateUpdate()
        {
            OnLateUpdate();
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
