using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchingSpeed;
    
    [Header("Jump")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private bool isGrounded = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    
    [Header("Crouch")]
    [SerializeField] private float standingScale;
    [SerializeField] private float crouchingScale;
    [HideInInspector] public bool isCrouching = false;

    public float maxSlopeAngle = 50f;
    public Transform Orientation;
    private Rigidbody rigid;
    private CapsuleCollider capsule;
    private Vector3 groundNormal = Vector3.up;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        playerSpeed = walkSpeed;
        rigid.freezeRotation = true;
        standingScale =  transform.localScale.y;
        rigid.maxAngularVelocity = 0f;
    }
        
    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        Crouch();
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Move();
        LimitVerticalVelocity();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 inputDirection = (Orientation.forward * vertical + Orientation.right * horizontal);
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

    private void Crouch()   
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            playerSpeed = crouchingSpeed;
            transform.localScale = new Vector3(transform.localScale.x, crouchingScale, transform.localScale.z);
        }
        else
        {
            isCrouching = false;
            playerSpeed = walkSpeed;
            transform.localScale = new Vector3(transform.localScale.x, standingScale, transform.localScale.z);
        }
    }

    private void Jump()
    {
        Vector3 velocity = rigid.linearVelocity;

        velocity.y = 0f;

        rigid.linearVelocity = velocity;
        
        rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            groundNormal =  hit.normal;

            float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);

            if (slopeAngle > maxSlopeAngle)
            {
                isGrounded = false;
                groundNormal = Vector3.up;
            }
            else
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    private void LimitVerticalVelocity()
    {
        Vector3 velocity = rigid.linearVelocity;

        if (velocity.y > 8f)
        {
            velocity.y = 8f;
            rigid.linearVelocity = velocity;
        }
    }
}