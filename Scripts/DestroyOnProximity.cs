using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    /// <summary>
    /// Destroys the Game Object when it comes within the specified distance to
    /// a specified target Game Object
    /// </summary>
    public class DestroyOnProximity : OnProximityBase
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
            Destroy(gameObject);
        }
    }

}
