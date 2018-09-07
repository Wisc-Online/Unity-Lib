using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    public class DestroyOnProximity : MonoBehaviour
    {

        [SerializeField]
        public float destroyThreshold = float.Epsilon;

        [SerializeField]
        public GameObject target;

        private void Update()
        {
            if (target != null)
            {
                var d = this.transform.position - target.transform.position;

                if (d.magnitude <= destroyThreshold)
                    Destroy(gameObject);
            }
        }
    }

}
