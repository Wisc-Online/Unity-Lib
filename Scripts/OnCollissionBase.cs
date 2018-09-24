using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    [RequireComponent(typeof(Collider))]
    public abstract class OnCollissionBase : MonoBehaviour
    {
        public bool anyTarget = false;
        public Collider target;

        private void OnCollisionEnter(Collision collision)
        {
            if ((anyTarget || collision.collider == target) && IsValid)
            {
                Execute();
            }
        }

        protected abstract bool IsValid { get; }

        protected abstract void Execute();
    }
}
