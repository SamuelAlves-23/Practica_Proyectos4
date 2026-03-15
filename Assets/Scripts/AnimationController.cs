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
    private Rigidbody rb;

    private bool jump;

    private void Awake()
    {
        controls = new Player_InputAction();
        controls.Player.Move.performed += ctx => inputVector = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => inputVector = Vector2.zero;
        controls.Player.Jump.performed += ctx => jump = true;
        controls.Player.Jump.canceled += ctx => jump = false;

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
        moveDirection.Normalize();
        rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);

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
