using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    enum PlayerState {Idle, Hurt, Moving, Rolling, Attacking}
    [SerializeField] PlayerState playerState;
    [SerializeField] float hp, rollCd, moveSpeed, maxHp = 4;
    Slider hpSlider;
    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    Transform attackPivot;
    WeaponScript weapon;
    public float AttackCd{get{return attackCd;}set{if(value < 0.1f) attackCd = 0.1f; else attackCd = value;}}
    public float Keys{get{return keys;}set{if(value>0) keys = value; else keys = 0;}}
    public float Hp {get{return hp;} set{if(value>maxHp) hp = maxHp; else hp = value;}}
    float rollTimer, attackTimer, attackCd, xinput, yinput, keys;
    [SerializeField] Vector3 inputDirection, lookAt;
    

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        anim = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        sprite = transform.GetComponent<SpriteRenderer>();
        
        hpSlider = GameObject.FindWithTag("UI").transform.GetChild(0).GetComponent<Slider>();
        hpSlider.minValue = 0;
        hpSlider.maxValue = maxHp;
        hpSlider.value = Hp;

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
                if(rollTimer < 0)
                    playerState = PlayerState.Idle; 
                break;

            case PlayerState.Attacking:
                if(attackTimer < 0)
                    playerState = PlayerState.Idle; 
                break;

            case PlayerState.Hurt:
                if(attackTimer > 0 || rollTimer > 0)
                {
                    rollTimer = 0;
                    attackTimer = 0;
                }
                sprite.color = (Time.time % 0.2f < 0.1f) ? Color.red : Color.white;
                break;
                
            default:
                break;
        }
        attackTimer -= Time.deltaTime;
        rollTimer -= Time.deltaTime;


        if(Input.GetButtonDown("Fire2") && rollTimer <= 0)
        {
            rb.AddForce(lookAt*moveSpeed*15, ForceMode2D.Impulse);
            rollTimer = rollCd;
            if(lookAt.x<0)
                anim.Play("Gobbo_Roll_Right");
            else
                anim.Play("Gobbo_Roll_Left");
            playerState = PlayerState.Rolling;
        }

        
        if(Input.GetButtonDown("Fire3") && attackTimer <= 0 && playerState != PlayerState.Rolling)
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
        hpSlider.value = Hp;
    }

    void CheckLiving()
    {
        if(Hp>0)
        {   
            playerState = PlayerState.Idle;
            sprite.color = Color.white;
        }
        else
            Destroy(gameObject);
    }

    public void GetHit(float dmg, float kb, Vector3 dir)
    {
        if(playerState != PlayerState.Hurt && playerState != PlayerState.Rolling)
        {
            CancelInvoke("CheckLiving");
            Invoke("CheckLiving", 0.5f);
            Hp -= dmg;
            playerState = PlayerState.Hurt;
            rb.AddForce((transform.position-dir).normalized * kb,ForceMode2D.Impulse);
        }
    }

    public void AddHP(float add)
    {
        Hp += add;
        if(add>0)
            sprite.color = Color.green;
        else
            sprite.color = Color.yellow;
        
        Invoke("CheckLiving", 0.2f);
    }

    public void AddDMG(float add)
    {
        weapon.Damage += add;
    }
    
    void FixedUpdate()
    {
        if(playerState != PlayerState.Attacking && playerState != PlayerState.Rolling)
            rb.AddForce(inputDirection * moveSpeed,ForceMode2D.Impulse);
        
    }
}
