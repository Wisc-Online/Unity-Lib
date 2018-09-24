using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    public class DestroyOnCollision : OnCollissionBase
    {
        protected override bool IsValid
        {
            get
            {
                return true;
            }
        }

        protected override void Execute()
        {
            Destroy(this.gameObject);
        }
    }
}