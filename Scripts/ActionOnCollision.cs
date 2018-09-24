using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity
{
    
    public class ActionOnCollision : OnCollissionBase
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
