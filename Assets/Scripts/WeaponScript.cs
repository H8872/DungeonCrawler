using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public float Damage, KnockBack, AttackSpeed;
    void Awake() {
        gameObject.tag = "Weapon";
    }
}
