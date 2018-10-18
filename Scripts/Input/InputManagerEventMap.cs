using System.Linq;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    public abstract class InputManagerEventMap : MonoBehaviourBase
    {
        [SerializeField]
        private string _gamepadName;

        public string GamepadName
        {
            get
            {
                return _gamepadName;
            }

            private set
            {
                _gamepadName = value;
            }
        }


        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsGamepadConnected)
            {
                InvokeMappedEvents();
            }
        }

        protected abstract void InvokeMappedEvents();

        bool IsGamepadConnected { get { return UnityEngine.Input.GetJoystickNames().Any(x => x == GamepadName); } }

    }

}