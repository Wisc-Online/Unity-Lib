using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity
{
    [RequireComponent(typeof(Collider))]
    public class ActionOnCollision : MonoBehaviour
    {
        public bool anyTarget = false;
        public Collider target;

        [SerializeField]
        public UnityEvent Actions;

        private void OnCollisionEnter(Collision collision)
        {
            if ((anyTarget || collision.collider == target) && Actions != null)
            {
                Actions.Invoke();
            }
        }
    }
}
