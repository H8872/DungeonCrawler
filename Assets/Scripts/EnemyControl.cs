using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] float hp, moveSpeed;
    float chaseTimer, chaseMax = 2f;
    GameObject targetObject;
    Vector3 waypoint;
    bool playerDetected = false, playerInRange = false;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        waypoint = transform.position;
        rb = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerInRange && chaseTimer > 0)
        {
            chaseTimer -= Time.deltaTime;
        }
        if(chaseTimer<0)
        {
            targetObject = gameObject;
        }
        if(targetObject != null)
            waypoint = targetObject.transform.position;
        
    }

    private void FixedUpdate()
    {
        rb.AddForce((waypoint-transform.position).normalized * moveSpeed,ForceMode2D.Impulse);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            playerDetected = true;
            playerInRange = true;
            chaseTimer = chaseMax;
            targetObject = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player")
            playerInRange = false;
    }
}
