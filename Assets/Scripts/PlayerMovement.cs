using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public float walkSpeed = 2f;
    public float sprintSpeed = 7f;
    public float crouchingSpeed = 2f;
    public float jumpHeight = 3f;
    public float groundCheckDistance = 0.3f;
    
    
    private bool isSprinting = false;
    [HideInInspector] public bool isCrouching = false;
    private bool isGrounded = false;

    private Rigidbody rigid;
    private CapsuleCollider capsule;
    
    public Transform groundCheck;
    public LayerMask groundLayer;

    public float standingScale;
    public float crouchingScale;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpeed = walkSpeed;
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        standingScale =  transform.localScale.y;
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
            isCrouching = true;
            playerSpeed = crouchingSpeed;
            transform.localScale = new Vector3(transform.localScale.x, crouchingScale, transform.localScale.z);
            rigid.AddForce(Vector3.down, ForceMode.Impulse);
        }
        else
        {
            isCrouching = false;
            playerSpeed = walkSpeed;
            transform.localScale = new Vector3(transform.localScale.x, standingScale, transform.localScale.z);
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
            playerSpeed = crouchingSpeed;
        }
        
        Vector3 targetVelocity = inputDirection * playerSpeed;
        
        Vector3 velocity = rigid.linearVelocity;
        Vector3 velocityChange = new Vector3(targetVelocity.x - velocity.x, 0, targetVelocity.z - velocity.z);
        
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
