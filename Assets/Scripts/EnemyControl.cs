using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    enum EnemyState {Hurt, Chasing, Idle, Moving}
    [SerializeField] EnemyState enemyState;
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
        enemyState = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                targetObject = gameObject;
                if(playerDetected)
                    enemyState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:
                chaseTimer -= Time.deltaTime;
                if(playerInRange)
                {
                    chaseTimer = chaseMax;
                }
                else if(chaseTimer<0)
                {
                    targetObject = gameObject;
                    playerDetected = false;
                    enemyState = EnemyState.Idle;
                }
                else
                {
                    waypoint = targetObject.transform.position;
                    targetObject = null;
                    enemyState = EnemyState.Moving;
                }
                break;

            case EnemyState.Moving:
                
                if(playerInRange)
                {
                    enemyState = EnemyState.Chasing;
                    break;
                }
                targetObject = null;
                if((waypoint-transform.position).magnitude < 0.1f)
                {
                    playerDetected = false;
                    enemyState = EnemyState.Idle;
                }
                break;

            default:
                break;
        }
        if(targetObject != null)
        {
            waypoint = targetObject.transform.position;
        }
        
    }

    private void FixedUpdate()
    {
        rb.AddForce((waypoint-transform.position).normalized * moveSpeed,ForceMode2D.Impulse);
    }

    public void SetWaypoint(Vector3 target)
    {
        targetObject = null;
        waypoint = target;
        enemyState = EnemyState.Moving;
    }

    public void HandleDetection(GameObject other, bool inRange, bool LoS)
    {
        targetObject = other;
        if(other.tag == "Player")
        {
            Debug.Log(LoS);
            // If Line of Sight
            if(LoS)
            {
                playerDetected = true;
                if(inRange)
                {
                    playerInRange = true;
                }
                else
                {
                    playerInRange = false;
                    chaseTimer = chaseMax;
                }
            }
            else
            {
                playerInRange = false;
            }
        }
    }
}
