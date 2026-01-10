using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private SpriteRenderer playerRender;
    [Header("WallCheck")]
    public Transform wallCheck;
    public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
    public LayerMask wallLayer;
    private bool isGrounded;
    private bool isWalljumping;
    private Vector2 wallJumpDirection;
    
    [Header("Camera Change")]
    [SerializeField] private GameObject leftCam;
    [SerializeField] private GameObject rightCam;
    
    [Header("Movement")]
    [SerializeField] private float movementForce = 2;
    [SerializeField] private float jumpForce = 4;
    [SerializeField] private float wallJumpForce = 4;
    [SerializeField] private float wallSlideSpeed = 2;
    [SerializeField] private bool isWallSliding;
    private float hInput;
    private Animator anim;
    private Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerRender = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
            hInput = Input.GetAxisRaw("Horizontal");
            
            wallJumpDirection = new Vector2(-hInput, 0);
                
                anim.SetFloat("xSpeed", Mathf.Abs(hInput));
                anim.SetFloat("ySpeed", rb.linearVelocityY);
                
                TurnCheck();
        
                    
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    isGrounded = false;
                }

                if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    rb.AddForce(wallJumpDirection * wallJumpForce, ForceMode2D.Impulse);
                }
                
                ProccessWallSlide();
                

    }
    void FixedUpdate()
    {
        rb.AddForce(new  Vector2(hInput * movementForce, 0) , ForceMode2D.Force);
    }
    private void TurnCheck()
    {
        if (hInput > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (hInput < 0 && transform.eulerAngles.y == 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer);
    }

    private void ProccessWallSlide()
    {
        if (!isGrounded && WallCheck() && hInput != 0)
        {
            anim.Play("Player_WallSlide");
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Math.Max(rb.linearVelocity.y, -wallSlideSpeed));

            playerRender.flipX = true;

        }
        else
        {
            isWallSliding = false;
            playerRender.flipX = false;
        }
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("changecamera left"))
        {
            rightCam.SetActive(false);
            leftCam.SetActive(true);
            Debug.Log("left");
        }
        else if (other.CompareTag("changecamera right"))
        {
            rightCam.SetActive(true);
            leftCam.SetActive(false);
            Debug.Log("right");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
