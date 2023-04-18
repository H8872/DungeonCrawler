using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float hp, rollCd, attackCd, moveSpeed;
    Animator anim;
    Rigidbody2D rb;
    Transform attackPivot;
    float moveForce, rollTimer, attackTimer, xinput, yinput;
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        attackPivot = transform.GetChild(1);
        direction = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        moveForce = moveSpeed * 1000;

        xinput = Input.GetAxisRaw("Horizontal");
        yinput = Input.GetAxisRaw("Vertical");

        attackPivot.position = transform.position + direction/2;
        attackPivot.rotation = Quaternion.FromToRotation(Vector3.up, direction);

        if(xinput<0)
            transform.GetComponent<SpriteRenderer>().flipX = true;
        else if(xinput>0)
            transform.GetComponent<SpriteRenderer>().flipX = false;

        if(xinput == 0 && yinput == 0)
        {
            anim.SetBool("Walkin", false);
        }
        else 
        {
            anim.SetBool("Walkin", true);
            direction = new Vector3(xinput,yinput,0);
        }
        
        if(rollTimer > 0) rollTimer -= Time.deltaTime;
        if(Input.GetButtonDown("Jump") && rollTimer <= 0)
        {
            rb.AddForce(direction*(moveForce/2));
            rollTimer = rollCd;
        }

        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if(Input.GetButtonDown("Fire1") && attackTimer <= 0)
        {
            anim.Play("Gobbo_Attack");
            attackTimer = attackCd;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(xinput*Time.deltaTime*moveForce,yinput*Time.deltaTime*moveForce));
        
    }
}
