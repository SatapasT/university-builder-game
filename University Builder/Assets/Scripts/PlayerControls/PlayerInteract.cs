using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class InteractWorkshop : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float interactableRange = 3f;

        private IInteractable currentTarget = null;

        void Update()
        {
            if (Keyboard.current == null) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            IInteractable newTarget = null;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactableRange))
            {
                newTarget = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (newTarget != currentTarget)
            {
                if (currentTarget != null)
                    currentTarget.OnLoseFocus();

                if (newTarget != null)
                    newTarget.OnFocus();

                currentTarget = newTarget;
            }

            if (currentTarget != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                currentTarget.Interact();
            }
        }
    }
}
