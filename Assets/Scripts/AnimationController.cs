using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    public float movementSpeed = 8f;
    private Player_InputAction controls;
    private Animator animator;
    private Vector2 inputVector;
    private Vector2 delta;
    private Rigidbody rb;
    public new Camera camera;
    private float rotationSpeed = 1.0f;
    private bool _attackPressed;
    private bool _isAttacking;
    private float _originalSpeed;
    public GameObject player;
    public GameObject hitbox;
    public new AudioSource audio;
    public AudioClip clip;


    private void Awake()
    {
        controls = new Player_InputAction();
        controls.Player.Move.performed += ctx => inputVector = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => inputVector = Vector2.zero;
        controls.Player.Jump.performed += ctx => _attackPressed = true;
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

        rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime);

        // Only rotate horizontally with camera (yaw), ignore pitch and roll
        Vector3 cameraEuler = camera.transform.eulerAngles;
        rb.MoveRotation(Quaternion.Euler(0, cameraEuler.y, 0));
        


        animator.SetFloat("XSpeed", inputVector.x);
        animator.SetFloat("YSpeed", inputVector.y);

        if (_attackPressed && !_isAttacking)
        {
            _attackPressed = false;
            Attack();
        }
   
    }

    private void Attack()
    {
        _isAttacking = true;
        _originalSpeed = movementSpeed;
        animator.SetBool("Jump", true);
        animator.SetBool("Air", false);
        movementSpeed = 0;
        StartCoroutine("DeactivateJump");
    }

    private IEnumerator DeactivateJump()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        animator.SetBool("Jump", false);
        animator.SetBool("Air", true);
        movementSpeed = _originalSpeed;
        _isAttacking = false;
    }

    private void AttackAnimEvent()
    {
        hitbox.SetActive(true);
        audio.PlayOneShot(clip);
    }

    private void UnAttackAnimEvent()
    {
        hitbox.SetActive(false);
    }

    
}
