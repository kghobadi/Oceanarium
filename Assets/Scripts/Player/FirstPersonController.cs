using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{

    public float mouseSensitivityX = 20;
    public float mouseSensitivityY = 20;
    public float moveSpeed = 8;
    public float runSpeed = 20;
    public float jumpForce = 220;
    public bool infiniteJump = false;
    public LayerMask groundedMask;

    Transform cameraT;
    float verticalLookRotation;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    Rigidbody rb;

    bool grounded;

    void Start()
    {
        cameraT = Camera.main.transform;
        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 targetMoveAmount = moveDir * moveSpeed;
        if (Input.GetButton("Sprint")) {
            targetMoveAmount = moveDir * runSpeed;
        }
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        if (Input.GetButtonDown("Jump"))
        {
            if (infiniteJump || grounded)
            {
                rb.AddForce(transform.forward * jumpForce);
            }
        }
        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            grounded = true;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
