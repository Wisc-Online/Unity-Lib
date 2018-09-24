using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    public class DestroyOnProximityLeave : DestroyOnProximity
    {
        protected override bool IsInProximityDistance()
        {
            return !base.IsInProximityDistance();
        }
    }

}
