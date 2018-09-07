using UnityEngine;
using UnityEngine.Events;

namespace FVTC.LearningInnovations.Unity
{
    public class ActionOnProximity : MonoBehaviour
    {
        public GameObject target;

        [SerializeField]
        public float targetDistance = float.Epsilon;

        [SerializeField]
        public UnityEvent Actions;

        private void Update()
        {
            if (target != null && Actions != null)
            {
                var distance = target.transform.position - gameObject.transform.position;

                if (distance.magnitude <= targetDistance)
                {
                    Actions.Invoke();
                }
            }
        }
    }

}
