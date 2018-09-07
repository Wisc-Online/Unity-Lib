using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity.Input.Pointer
{
    public class PointerExitHandler : MonoBehaviour, IPointerExitHandler
    {
        [Header("Ensure an Event System exists in the Scene")]
        [Header("And a PhysicsRaycaster is attached to the Camera.")]
        public UnityEvent Event;

        [SerializeField]
        public PointerEvent PointerEvent;

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Event != null)
                Event.Invoke();

            if (PointerEvent != null)
                PointerEvent.Invoke(eventData);
        }
    }




}
