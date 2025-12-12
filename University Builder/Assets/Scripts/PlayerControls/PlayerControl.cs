using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerControl : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float jumpForce = 5f;
    public float extraGroundCheck = 0.1f; 

    public Transform orientation;
    public GroundCheck groundCheck;

    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = true;   
    }

    void Update()
    {
        if (UIManager.AnyMenuOpen)
            return;

        if (Keyboard.current == null) return;

        if (groundCheck == null) return;

        Vector3 moveDir = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) 
        { 
            moveDir += orientation.forward;
        }

        if (Keyboard.current.sKey.isPressed) 
        {
            moveDir -= orientation.forward;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            moveDir -= orientation.right;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveDir += orientation.right;
        }

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        moveDir *= movementSpeed * PlayerStats.Instance.GetMoveSpeedMultiplier();

        Vector3 velocity = rigidbody.linearVelocity;
        velocity.x = moveDir.x;
        velocity.z = moveDir.z;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && groundCheck.isGrounded)
        {
            velocity.y = jumpForce;
        }

        rigidbody.linearVelocity = velocity;

        if (Keyboard.current.tabKey.isPressed)
        {
            PlayerMenu.Instance.OpenMenu();
        }
    }
}