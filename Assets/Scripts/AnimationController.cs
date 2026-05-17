using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    public float movementSpeed = 5f;
    private Player_InputAction controls;
    private Animator animator;
    private Vector2 inputVector;
    private Vector2 delta;
    private Rigidbody rb;
    public Camera camera;
    private float rotationSpeed = 1.0f;
    private bool jump;
    public GameObject player;

    private void Awake()
    {
        controls = new Player_InputAction();
        controls.Player.Move.performed += ctx => inputVector = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => inputVector = Vector2.zero;
        controls.Player.Jump.performed += ctx => jump = true;
        controls.Player.Jump.canceled += ctx => jump = false;
        controls.Player.Camera.performed += ctx => delta = Mouse.current.delta.ReadValue();
        controls.Player.Camera.canceled += ctx => delta = Vector2.zero;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
       
        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        moveDirection = camera.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        
        moveDirection.Normalize();

        // Rotate 90° around Y-axis
       

        Vector3 cameraR = new Vector3(camera.transform.rotation.x, 0, camera.transform.rotation.z);
        Quaternion rotation = Quaternion.Euler(cameraR.x, 0, cameraR.z);

       
        rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);

        camera.transform.rotation = Quaternion.Euler(cameraR.x, 0, cameraR.z);
        rb.MoveRotation(camera.transform.rotation);
        rb.rotation = rotation;


        animator.SetFloat("XSpeed", inputVector.x);
        animator.SetFloat("YSpeed", inputVector.y);

        if (jump == true)
        {
            Jump();
            
        }
   
    }

    private void Jump()
    {
        animator.SetBool("Jump", true);
        animator.SetBool("Air", false);
        StartCoroutine("DeactivateJump");
    }

    private IEnumerator DeactivateJump()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        animator.SetBool("Jump", false);
        animator.SetBool("Air", true);
    }
}
