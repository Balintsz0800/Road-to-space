using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public float walkSpeed = 2f;
    public float sprintSpeed = 7f;
    public float crouchigSpeed = 2f;
    public float jumpHeight = 3f;
    public float mouseSensivity = 2f;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float groundCheckDistance = 0.3f;
    public float crouchSmooth = 6f;
    
    
    private bool isMoving = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isGrounded = false;

    private Rigidbody rigid;
    private CapsuleCollider capsule;
    
    public Transform groundCheck;
    public Transform playerModel;
    public LayerMask groundLayer;
    Vector3 standingCenter;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpeed = walkSpeed;
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        standingCenter = capsule.center;
    }
        
    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerSpeed = crouchigSpeed;
            capsule.height = crouchingHeight;
            capsule.center = new Vector3(standingCenter.x, standingCenter.y - (standingHeight - crouchingHeight) / 2f, standingCenter.y);
            playerModel.localScale = new Vector3(1, 0.5f, 1);
        }
        else
        {
            playerSpeed = walkSpeed;
            capsule.height = standingHeight;
            capsule.center = standingCenter;
            playerModel.localScale = new Vector3(1, 1, 1);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 inputDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        playerSpeed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            playerSpeed = sprintSpeed;
        }

        if (isCrouching)
        {
            playerSpeed = crouchigSpeed;
        }
        
        Vector3 targetVelocity = inputDirection * playerSpeed;
        
        Vector3 velocity = rigid.linearVelocity;
        Vector3 velocityChange = new Vector3(targetVelocity.x, 0, targetVelocity.z) - new Vector3(velocity.x, 0, velocity.z);
        
        rigid.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Jump()
    {
            rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundLayer);
    }
}
