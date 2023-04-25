using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    EnemyControl control;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        control = transform.parent.GetComponent<EnemyControl>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon")
        {
            WeaponScript wep = other.GetComponent<WeaponScript>();
            control.GetHit(wep.Damage, wep.KnockBack, other.transform.position);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerScript>().GetHit(control.Damage,control.KnockBack,transform.position);
        }
    }
}
