using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    public abstract class OnProximityBase : MonoBehaviour
    {
        public GameObject target;

        [SerializeField]
        public float distance;

        [HideInInspector]
        bool _invoked = false;

        [HideInInspector]
        protected abstract bool IsValid { get; }

        private void Update()
        {
            if (target != null && IsValid)
            {
                bool isInProximityDistance = IsInProximityDistance();

                if (isInProximityDistance && !_invoked)
                {
                    Execute();
                    _invoked = true;
                }
                else if (isInProximityDistance && _invoked)
                {
                    _invoked = false;
                }
            }
        }

        protected virtual bool IsInProximityDistance()
        {
            var distance = target.transform.position - gameObject.transform.position;

            return distance.magnitude <= this.distance;
        }

        protected abstract void Execute();
    }
}
