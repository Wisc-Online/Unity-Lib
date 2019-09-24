using FVTC.LearningInnovations.Unity.Events;
using UnityEngine;
using UnityEngine.Events;

namespace FVTC.LearningInnovations.Unity
{
    public class ActionOnTrigger : OnTriggerBase
    {
        [SerializeField]
        public UnityEventCollider Actions;

        protected override bool IsValid
        {
            get { return Actions != null; }
        }

        protected override void Execute(Collider collider)
        {
            Actions.Invoke(collider);
        }
    }
}
