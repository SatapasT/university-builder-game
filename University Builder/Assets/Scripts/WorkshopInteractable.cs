using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Using the workbench!");
    }
}
