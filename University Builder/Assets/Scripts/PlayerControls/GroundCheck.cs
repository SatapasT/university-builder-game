using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded;

    void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }

    void OnTriggerStay(Collider other)
    {
        isGrounded = true;
    }

    void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
