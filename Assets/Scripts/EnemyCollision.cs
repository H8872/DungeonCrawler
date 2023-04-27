using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    EnemyControl control;
    AIPath ai;
    Rigidbody2D rb;
    bool touchWall;
    float stuckTimer, stuckMax = 3f;
    // Start is called before the first frame update
    void Start()
    {
        control = transform.parent.GetComponent<EnemyControl>();
        ai = transform.parent.GetComponent<AIPath>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }
    private void Update() {
        if(touchWall)
            stuckTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Weapon")
        {
            WeaponScript wep = other.GetComponent<WeaponScript>();
            control.GetHit(wep.Damage, wep.KnockBack, other.transform.position);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag == "Player")
        {
            touchWall = false;
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            if(player == null)
                player = other.transform.parent.GetComponent<PlayerScript>();
            
            player.GetHit(control.Damage,control.KnockBack,transform.position);
        }
        else if(other.gameObject.tag == "WallCollider")
        {
            if(!touchWall && ai.velocity.magnitude < 0.1f)
            {
                touchWall = true;
                stuckTimer = stuckMax;
            }
            if(stuckTimer<0 && touchWall)
            {
                control.GetHit(0f, 15f, control.Home+(transform.position-control.Home)*2);
                touchWall = false;
            }
        }
        else
            touchWall = false;
    }
}
