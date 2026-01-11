using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;


public class Player : MonoBehaviour, IDamagable
{
    [Header("HealthBar")]
    [SerializeField] Slider healthBar;
    [SerializeField] int health = 100;
    
    [SerializeField] GameObject Door;
    
    private Scene sceneManager;
    private SpriteRenderer playerRender;
    [Header("WallCheck")]
    public Transform wallCheck;
    public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
    public LayerMask wallLayer;
    private bool isGrounded;
    private bool isWalljumping;
    private Vector2 wallJumpDirection;
    
    private Player playerScript;

    [Header("AttackCheck")] 
    [SerializeField] private GameObject attackPoint;
    
    
    [Header("Camera Change")]
    [SerializeField] private GameObject leftCam;
    [SerializeField] private GameObject rightCam;
    [SerializeField] private GameObject finalCam;
    [Header("Movement")]
    [SerializeField] private float movementForce = 2;
    [SerializeField] private float jumpForce = 4;
    [SerializeField] private float wallJumpForce = 4;
    [SerializeField] private float wallSlideSpeed = 2;
    [SerializeField] private bool isWallSliding;
    private float hInput;
    private Animator anim;
    private Rigidbody2D rb;
    
    
    private IEnumerator AttackCheck()
    {
        attackPoint.SetActive(true);
        yield return new WaitForSeconds(1f);
        attackPoint.SetActive(false);
    }
    
    private IEnumerator KnockBack(Vector3 knockBackDirection)
    {
        rb.AddForce(knockBackDirection.normalized * 10f, ForceMode2D.Impulse);
        anim.Play("Player_Hit");
        yield return new WaitForSeconds(0.5f);
        
        rb.linearVelocity = Vector2.zero;
    }
    public void TakeDamage(GameObject dealer, int damage)
    {
        Vector3 knockBackDirection = transform.position - dealer.transform.position;
        knockBackDirection.y = 0;
        StartCoroutine(KnockBack(knockBackDirection));
        health -= damage;

        if (health <= 0)
        {
            healthBar.value = 0;
            anim.Play("Player_Death");
            Destroy(playerScript);
        }
        
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerScript = GetComponent<Player>();
        playerRender = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
        
            hInput = Input.GetAxisRaw("Horizontal");
            
            wallJumpDirection = new Vector2(-hInput, 0);
                
                anim.SetFloat("xSpeed", Mathf.Abs(hInput));
                anim.SetFloat("ySpeed", rb.linearVelocityY);
                
                TurnCheck();
        
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    anim.Play("Player_Attack");
                    StartCoroutine(AttackCheck());
                }
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
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(other.gameObject, 25);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private IEnumerator Ending()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(Ending());
            

        }
        if (other.gameObject.CompareTag("Key"))
        {
            Destroy(other.gameObject);
            Door.GetComponent<ExitDoor>().OpenDoor();
        }
        if (other.gameObject.CompareTag("Trap"))
        {
            healthBar.value = 0;
            anim.Play("Player_Death");
            Destroy(playerScript);
        }
        if (other.CompareTag("changecamera left"))
        {
            rightCam.SetActive(false);
            leftCam.SetActive(true);
        }
        else if (other.CompareTag("changecamera right"))
        {
            rightCam.SetActive(true);
            leftCam.SetActive(false);
            finalCam.SetActive(false);
        }
        else if (other.CompareTag("FinalCam"))
        {
            rightCam.SetActive(false);
            finalCam.SetActive(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
