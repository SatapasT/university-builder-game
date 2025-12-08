using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class InteractWorkshop : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float interactableRange = 3f;

        private IInteractable currentTargetedInteractable;

        void Update()
        {
            if (Keyboard.current == null) return;
            Ray playerRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(playerRay, out RaycastHit hit, interactableRange))
            {
                currentTargetedInteractable = hit.collider.GetComponent<IInteractable>();
                currentTargetedInteractable.OnFocus();
            }
            else
            {
                currentTargetedInteractable.OnLoseFocus();
                currentTargetedInteractable = null;
            }

            if (currentTargetedInteractable != null &&
                Keyboard.current.fKey.wasPressedThisFrame)
            {
                Debug.Log($"Interacting with {hit.collider.name}");
                currentTargetedInteractable.Interact();
            }
        }
    }
}
