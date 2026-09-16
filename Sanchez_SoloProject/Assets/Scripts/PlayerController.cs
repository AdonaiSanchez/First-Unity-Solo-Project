using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 10.0f;
    public float jumpDetectDistance = 1f;

    Ray jumpRay;
    Vector2 moveInput = Vector2.zero;

    public GameObject currentWeaponObj;
    Transform weaponSlot;
    Camera playerCam;
    PlayerInput input;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        weaponSlot = transform.GetChild(0);

        jumpRay = new Ray();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        Quaternion playerRotation = Quaternion.identity;
        playerRotation.y = playerCam.transform.rotation.y;
        playerRotation.w = playerCam.transform.rotation.w;
        transform.rotation = playerRotation;
    }

    // Update is called once per frame
    void Update()
    {
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;
        
        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = moveInput.x * speed;
        tempMove.z = moveInput.y * speed;

        rb.linearVelocity = (tempMove.x * transform.right) + (tempMove.y * transform.up) + (tempMove.z * transform.forward);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump()
    {
        if (Physics.Raycast(jumpRay, jumpDetectDistance))
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

}
