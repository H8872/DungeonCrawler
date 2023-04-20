using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float hp, rollCd, attackCd, moveSpeed;
    Animator anim;
    Rigidbody2D rb;
    Transform attackPivot;
    float rollTimer, attackTimer, xinput, yinput;
    Vector3 inputDirection, lookAt;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        attackPivot = transform.GetChild(0);

        lookAt = Vector3.right;
        anim.SetFloat("xInput", Mathf.Round(lookAt.x));
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

        if(inputDirection == Vector3.zero)
        {
            anim.SetBool("Walkin", false);
        }
        else 
        {
            anim.SetBool("Walkin", true);
            lookAt = inputDirection.normalized;
            anim.SetFloat("xInput", Mathf.Round(lookAt.x));
            anim.SetFloat("yInput", Mathf.Round(lookAt.y));
        }


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

        
        if(rollTimer > 0) rollTimer -= Time.deltaTime;
        if(Input.GetButtonDown("Jump") && rollTimer <= 0)
        {
            rb.AddForce(lookAt*moveSpeed*10, ForceMode2D.Impulse);
            rollTimer = rollCd;
        }

        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            anim.SetFloat("Attack",0);
        }
        if(Input.GetButtonDown("Fire1") && attackTimer <= 0)
        {
            //anim.Play("Gobbo_Attack");
            attackTimer = attackCd;
            anim.SetFloat("Attack",attackTimer);
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

    void FixedUpdate()
    {
        rb.AddForce(inputDirection * moveSpeed,ForceMode2D.Impulse);
        
    }
}
