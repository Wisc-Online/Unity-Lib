using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity.Input.Pointer
{
    public class PointerDownHandler : MonoBehaviour, IPointerDownHandler
    {
        public PointerEventData.InputButton button = PointerEventData.InputButton.Left;

        [Header("Ensure an Event System exists in the Scene")]
        [Header("And a PhysicsRaycaster is attached to the Camera.")]
        public UnityEvent Event;

        [SerializeField]
        public PointerEvent PointerEvent;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == button)
            {
                if (Event != null)
                    Event.Invoke();

                if (PointerEvent != null)
                    PointerEvent.Invoke(eventData);
            }
        }
    }
}
