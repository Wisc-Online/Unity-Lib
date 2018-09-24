using System;
using UnityEngine;
using UnityEngine.Events;

namespace FVTC.LearningInnovations.Unity
{
    public class ActionOnProximity : OnProximityBase
    {
        [SerializeField]
        public UnityEvent Actions;

        protected override bool IsValid
        {
            get
            {
                return Actions != null;
            }
        }

        protected override void Execute()
        {
            Actions.Invoke();
        }
    }

}
