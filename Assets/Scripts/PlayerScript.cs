using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float hp, moveSpeed;
    Animator anim;
    Rigidbody2D rb;
    float moveForce;
    Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        moveForce = moveSpeed * 1000;

        if(Input.GetAxisRaw("Horizontal")<0)
            transform.GetComponent<SpriteRenderer>().flipX = true;
        else if(Input.GetAxisRaw("Horizontal")>0)
            transform.GetComponent<SpriteRenderer>().flipX = false;
        anim.SetFloat("Speed", rb.velocity.magnitude);
        if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            anim.Play("Gobbo_Idle");
            anim.SetFloat("Speed", 0);
        }
        else 
        {
            anim.SetFloat("Speed", rb.velocity.magnitude);
            direction = rb.velocity.normalized;
        }
        if(Input.GetButtonDown("Jump"))
        {
            rb.AddForce(direction*(moveForce/2));
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal")*Time.deltaTime*moveForce,Input.GetAxisRaw("Vertical")*Time.deltaTime*moveForce));
        
    }
}
