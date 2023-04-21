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

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon")
        {
            WeaponScript wep = other.GetComponent<WeaponScript>();
            rb.AddForce((transform.position-other.transform.position).normalized * wep.KnockBack,ForceMode2D.Impulse);
            //enemyState = EnemyState.Hurt;
            //hp -= wep.Damage;
            control.Invoke("CheckLiving", 0.5f);
        }
    }
}
