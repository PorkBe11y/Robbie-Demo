using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermove : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    
    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;
    public float hangingJumpForce = 15f;
    float jumpTime;
    
    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlocked;
    public bool isHanging;

    [Header("环境检测")]
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundDistence = 0.2f;
    float playerHeight;
    public float eyeHeight = 1.5f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;
    
    public LayerMask groundLayer;
    
    public float xVelocity;
    
    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;

    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffSet;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffSet;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        

        playerHeight = coll.size.y;

        colliderStandSize = coll.size;
        colliderStandOffSet = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        colliderCrouchOffSet = new Vector2(coll.offset.x, coll.offset.y / 2f);
        
    }

    
    void Update()
    {
        if (GameManager.GameOver())
        {
            return;
        }
        if (!jumpPressed)
        {
            jumpPressed = Input.GetButtonDown("Jump");
        }

        jumpPressed = Input.GetButton("Jump");
        jumpHeld = Input.GetButton("Jump"); 
        crouchHeld = Input.GetButton("Crouch");
        
    }
    private void FixedUpdate()
    {
        /*if (GameManager.GameOver())
        {
            return;
        }*/
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        /*Vector2 pos = transform.position;
        Vector2 offset = new Vector2(-footOffset, 0f);

        RaycastHit2D leftCheck = Physics2D.Raycast(pos + offset, Vector2.down, groundDistence, groundLayer);
        Debug.DrawRay(pos +offset,Vector2.down,Color.red,0.2f);*/

        //左右脚射线
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistence, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistence, groundLayer);
        
        if (leftCheck || rightCheck)
            isOnGround = true;
        else isOnGround = false;

        //头顶射线
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);
        if (headCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;

        //
        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);
        
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset*direction, playerHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset*direction, eyeHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset*direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if (!isOnGround && rb.velocity.y<0f && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;

            pos.x += (wallCheck.distance-0.05f)* direction;

            pos.y -= ledgeCheck.distance;

            transform.position = pos;
            
            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }

    }
    void GroundMovement()
    {
        if (isHanging)
            return;
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)
            Standup();
        else if(!isOnGround && isCrouch)
            Standup();

        xVelocity = Input.GetAxis("Horizontal");//-1至1浮点型

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FlipDirection();
    }

    void MidAirMovement()
    {
        if (isHanging)
        {
            if (jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
            }
        }

        if (crouchHeld)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            isHanging = false;
        }
        
        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked)
        {
            if (isCrouch)
            {
                Standup();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;
            
            rb.AddForce(new Vector2(0f,jumpForce),ForceMode2D.Impulse);
            jumpPressed = false;
            
            AudioManager.PlayJumpAudio();

        }
        else if (isJump)
        {
            if (jumpPressed)
                rb.AddForce(new Vector2(0f,jumpHoldForce),ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }
        
    }
    void FlipDirection()
    {
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1,1);
        if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1,1);
    }

    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffSet;
    }

    void Standup()
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffSet;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDeraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDeraction, length, layer);

        Color color = hit ? Color.red : Color.green; 
        
        Debug.DrawRay(pos + offset,rayDeraction*length, color);

        return hit;
    }
    
}
