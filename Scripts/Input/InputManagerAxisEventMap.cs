using FVTC.LearningInnovations.Unity.Events;
using System;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    [Serializable]
    public class InputManagerAxisEventMap
    {
        [SerializeField]
        public string axisName;

        [SerializeField]
        public float multiplier;

        [SerializeField]
        public bool useMultiplier;

        [SerializeField]
        public UnityEventFloat unityEvent;

        public void Invoke()
        {
            if (!string.IsNullOrEmpty(axisName) && this.unityEvent != null)
            {
                float reading = UnityEngine.Input.GetAxis(axisName);

                if (useMultiplier)
                    reading *= multiplier;

                this.unityEvent.Invoke(reading);
            }
        }
    }
}