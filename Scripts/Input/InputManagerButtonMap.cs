using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    public class InputManagerButtonMap : InputManagerEventMap
    {

        [SerializeField]
        private InputManagerButtonEventMap[] _mapping;

        public InputManagerButtonEventMap[] Mapping
        {
            get
            {
                return _mapping ?? (_mapping = new InputManagerButtonEventMap[] { });
            }
            set
            {
                _mapping = value;
            }
        }

        protected override void InvokeMappedEvents()
        {
            foreach(var map in Mapping)
            {
                map.Invoke();
            }
        }
    }
}