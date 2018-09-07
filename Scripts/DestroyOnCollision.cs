using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    [RequireComponent(typeof(Collider))]
    public class DestroyOnCollision : MonoBehaviour
    {
        public bool anyTarget = false;
        public Collider target;


        private void OnCollisionEnter(Collision collision)
        {
            if (anyTarget || collision.collider == target)
            {
                Destroy(gameObject);
            }
        }
    }

}
