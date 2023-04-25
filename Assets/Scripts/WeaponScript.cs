using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    PlayerScript player;
    public float Damage, KnockBack, AttackSpeed;
    void Start() {
        player = transform.parent.parent.GetComponent<PlayerScript>();
        player.AttackCd = AttackSpeed;
        gameObject.tag = "Weapon";
    }
}
