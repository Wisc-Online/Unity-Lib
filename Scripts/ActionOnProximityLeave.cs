using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FVTC.LearningInnovations.Unity
{
    public class ActionOnProximityLeave : ActionOnProximity
    {
        protected override bool IsInProximityDistance()
        {
            return !base.IsInProximityDistance();
        }
    }
}