using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Input
{
    
    public class InputManagerAxisMap : InputManagerEventMap
    {
        [SerializeField]
        private InputManagerAxisEventMap[] _mapping;

        public InputManagerAxisEventMap[] Mapping
        {
            get
            {
                return _mapping ?? (_mapping = new InputManagerAxisEventMap[] { });
            }
            set
            {
                _mapping = value;
            }
        }

        protected override void InvokeMappedEvents()
        {
            foreach (var map in Mapping)
            {
                map.Invoke();
            }
        }
    }
}