using System;
using UnityEngine;
using UnityEngine.Events;

namespace FVTC.LearningInnovations.Unity.Input
{
    [Serializable]
    public class InputManagerButtonEventMap
    {
        [SerializeField]
        public string buttonName;

        [SerializeField]
        public UnityEvent downEvent;

        [SerializeField]
        public UnityEvent holdEvent;

        [SerializeField]
        public UnityEvent upEvent;

        public void Invoke()
        {
            if (!string.IsNullOrWhiteSpace(buttonName))
            {
                if (downEvent != null && UnityEngine.Input.GetButtonDown(buttonName))
                {
                    downEvent.Invoke();
                }

                if (upEvent != null && UnityEngine.Input.GetButtonUp(buttonName))
                {
                    upEvent.Invoke();
                }

                if (holdEvent != null && UnityEngine.Input.GetButton(buttonName))
                {
                    holdEvent.Invoke();
                }
            }
        }
    }
}