using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    enum PlayerState {Idle, Hurt, Moving, Rolling, Attacking}
    [SerializeField] PlayerState playerState;
    [SerializeField] float hp, rollCd, moveSpeed;
    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    Transform attackPivot;
    WeaponScript weapon;
    public float AttackCd{get{return attackCd;}set{if(value < 0.1f) attackCd = 0.1f; else attackCd = value;}}
    float rollTimer, attackTimer, attackCd, xinput, yinput;
    [SerializeField] Vector3 inputDirection, lookAt;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        sprite = transform.GetComponent<SpriteRenderer>();
        attackPivot = transform.GetChild(0);
        weapon = attackPivot.GetChild(0).GetComponent<WeaponScript>();
        if(weapon == null)
            Debug.LogError("Ples gib wepon D:");
        else
        {
            attackCd = weapon.AttackSpeed;

        }

        lookAt = Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {

        xinput = Input.GetAxisRaw("Horizontal");
        yinput = Input.GetAxisRaw("Vertical");
        
        xinput = (Mathf.Round((xinput*10)/5)*5)/10;
        yinput = (Mathf.Round((yinput*10)/5)*5)/10;

        inputDirection = new Vector3(xinput,yinput);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1);
        

        switch (playerState)
        {
            case PlayerState.Moving:
                if(inputDirection == Vector3.zero)
                {
                    playerState = PlayerState.Idle;
                    break;
                }
                anim.SetBool("Walkin", true);
                lookAt = inputDirection.normalized;
                if(xinput<0)
                {
                    transform.GetComponent<SpriteRenderer>().flipX = true;
                    attackPivot.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;

                }
                else if(xinput>0)
                {
                    transform.GetComponent<SpriteRenderer>().flipX = false;
                    attackPivot.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                }
                break;

            case PlayerState.Idle:
                anim.SetBool("Walkin", false);
                if(inputDirection != Vector3.zero)
                    playerState = PlayerState.Moving;
                break;

            case PlayerState.Rolling:
                rollTimer -= Time.deltaTime;
                if(rollTimer < 0)
                    playerState = PlayerState.Idle; 
                break;

            case PlayerState.Attacking:
                attackTimer -= Time.deltaTime;
                if(attackTimer < 0)
                    playerState = PlayerState.Idle; 
                break;

            case PlayerState.Hurt:
                sprite.color = (Time.time % 0.2f < 0.1f) ? Color.red : Color.white;
                break;
                
            default:
                break;
        }


        if(Input.GetButtonDown("Jump") && rollTimer <= 0)
        {
            rb.AddForce(lookAt*moveSpeed*15, ForceMode2D.Impulse);
            rollTimer = rollCd;
            playerState = PlayerState.Rolling;
        }

        
        if(Input.GetButtonDown("Fire1") && attackTimer <= 0)
        {
            attackTimer = attackCd;
            playerState = PlayerState.Attacking;
            attackPivot.position = transform.position + lookAt * 0.7f;
            rb.AddForce(lookAt*moveSpeed*5, ForceMode2D.Impulse);
            if(lookAt.x > 0)
                anim.Play("Gobbo_Attack_Left");
            else if(lookAt.y == 1)
                anim.Play("Gobbo_Attack_Up");
            else if(lookAt.y == -1)
                anim.Play("Gobbo_Attack_Down");
            else
                anim.Play("Gobbo_Attack_Right");
        }
    }

    void CheckLiving()
    {
        if(hp>0)
        {   
            playerState = PlayerState.Idle;
            sprite.color = Color.white;
        }
        else
            Destroy(gameObject);
    }

    public void GetHit(float dmg, float kb, Vector3 dir)
    {
        if(playerState != PlayerState.Hurt)
        {
            CancelInvoke("CheckLiving");
            Invoke("CheckLiving", 0.2f);
            hp -= dmg;
            playerState = PlayerState.Hurt;
            rb.AddForce((transform.position-dir).normalized * kb,ForceMode2D.Impulse);
        }
    }
    
    void FixedUpdate()
    {
        if(playerState != PlayerState.Hurt && playerState != PlayerState.Rolling)
            rb.AddForce(inputDirection * moveSpeed,ForceMode2D.Impulse);
        
    }
}
