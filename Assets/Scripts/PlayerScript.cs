using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public enum PlayerState {Idle, Hurt, Moving, Rolling, Attacking}
    public PlayerState playerState;
    [SerializeField] float hp, rollCd, moveSpeed, maxHp = 4;
    Slider hpSlider;
    TextMeshProUGUI weaponDmgText;
    Animator anim;
    AudioSource aSource;
    [SerializeField] AudioClip attackClip, hpUpClip, rollClip, hitClip, keyClip, invulnClip, dmgUpClip;
    SpriteRenderer sprite;
    Rigidbody2D rb;
    Transform attackPivot;
    WeaponScript weapon;
    public float AttackCd{get{return attackCd;}set{if(value < 0.1f) attackCd = 0.1f; else attackCd = value;}}
    public float Keys{get{return keys;}set{if(value>0) keys = value; else keys = 0;}}
    public float Hp {get{return hp;} set{if(value>maxHp) hp = maxHp; else { if(!isInvuln) hp = value;}}}
    float rollTimer, invultTimer, attackTimer, attackCd, xinput, yinput, keys;
    bool isInvuln = false;
    [SerializeField] bool cheats = false;
    public Vector3 inputDirection, lookAt;
    

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        anim = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        sprite = transform.GetComponent<SpriteRenderer>();
        aSource = transform.GetComponent<AudioSource>();
        
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
        weaponDmgText = GameObject.FindWithTag("UI").transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        weaponDmgText.text = "Lvl:"+weapon.Damage.ToString("00");

        lookAt = Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        if(cheats)
            hp = 4;

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
                    sprite.flipX = true;
                    attackPivot.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;

                }
                else if(xinput>0)
                {
                    sprite.flipX = false;
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
                rollTimer = 0;
                attackTimer = 0;
                sprite.color = (Time.time % 0.2f < 0.1f) ? Color.red : Color.white;
                break;
                
            default:
                break;
        }

        if(attackTimer>0)
            attackTimer -= Time.deltaTime;
        if(rollTimer>0)
            rollTimer -= Time.deltaTime;
        if(invultTimer>0)
        {
            invultTimer -= Time.deltaTime;
            if(invultTimer<3)
                sprite.color = (Time.time % 0.5f < 0.25f) ? Color.black : Color.white;
            else
                sprite.color = Color.black;
        }
        else
            isInvuln = false;



        if(Input.GetButtonDown("Fire2") && rollTimer <= 0)
        {
            aSource.clip = rollClip;
            aSource.Play();
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
            aSource.clip = attackClip;
            aSource.Play();
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
        if(Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Q))
            cheats = (cheats) ? cheats = false : cheats = true;
        if(Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.W))
            AddDMG(1);
        hpSlider.value = Hp;
    }

    void CheckLiving()
    {
        if(Hp>0)
        {   
            invultTimer = 0;
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
            aSource.clip = hitClip;
            aSource.Play();
            CancelInvoke("CheckLiving");
            Invoke("CheckLiving", 0.5f);
            Hp -= dmg;
            playerState = PlayerState.Hurt;
            rb.AddForce((transform.position-dir).normalized * kb,ForceMode2D.Impulse);
        }
    }

    public void AddHP(float add)
    {
        aSource.clip = hpUpClip;
        aSource.Play();
        if(invultTimer>0)
        {
            isInvuln = false;
            CheckLiving();
        }
        Hp += add;
        if(add>0)
            sprite.color = Color.green;
        else
            sprite.color = Color.yellow;
        
        Invoke("CheckLiving", 0.3f);
    }

    public void AddKey(float add)
    {
        aSource.clip = keyClip;
        aSource.Play();
        keys += add;
    }

    public void GiveInvuln(float time)
    {
        aSource.clip = invulnClip;
        aSource.Play();
        invultTimer = time;
        isInvuln = true;
        Invoke("CheckLiving", time);
    }

    public void AddDMG(float add)
    {
        aSource.clip = dmgUpClip;
        aSource.Play();
        weapon.Damage += add;
        weaponDmgText.text = "Lvl:"+weapon.Damage.ToString("00");
    }
    
    void FixedUpdate()
    {
        if(playerState != PlayerState.Attacking && playerState != PlayerState.Rolling)
            rb.AddForce(inputDirection * moveSpeed,ForceMode2D.Impulse);
        
    }
}
