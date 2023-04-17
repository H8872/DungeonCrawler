using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("Horizontal")<0)
            transform.GetComponent<SpriteRenderer>().flipX = true;
        else if(Input.GetAxisRaw("Horizontal")>0)
            transform.GetComponent<SpriteRenderer>().flipX = false;
        transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(Input.GetAxisRaw("Horizontal")*Time.deltaTime,Input.GetAxisRaw("Vertical")*Time.deltaTime));
    }
}
