using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    [RequireComponent(typeof(Collider))]
    public abstract class OnTriggerBase : MonoBehaviour
    {
        public bool anyTarget = false;
        public GameObject target;

        protected abstract bool IsValid { get; }

        protected abstract void Execute(Collider collider);

        private void OnTriggerEnter(Collider collider)
        {
            if ((anyTarget || collider.gameObject == target) && IsValid)
            {
                Execute(collider);
            }
        }
    }
}
