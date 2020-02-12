using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FVTC.LearningInnovations.Unity.Input.Pointer
{
    public class PointerEnterHandler : MonoBehaviour, IPointerEnterHandler
    {
        [Header("Ensure an Event System exists in the Scene")]
        [Header("And a PhysicsRaycaster is attached to the Camera.")]

        [SerializeField]
        public PointerEvent PointerEvent;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PointerEvent != null)
                PointerEvent.Invoke(eventData);
        }
    }




}
