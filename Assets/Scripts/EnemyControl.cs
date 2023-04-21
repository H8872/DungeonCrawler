using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    enum EnemyState {Idle, Chasing, Hurt, Moving}
    [SerializeField] EnemyState enemyState;
    [SerializeField] float hp, moveSpeed;
    SpriteRenderer sprite;
    float chaseTimer, chaseMax = 2f, dist;
    GameObject targetObject;
    Vector3 waypoint;
    bool playerDetected = false, playerInRange = false;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        waypoint = transform.position;
        rb = transform.GetComponent<Rigidbody2D>();
        dist = transform.GetComponent<CircleCollider2D>().radius;
        enemyState = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        

        chaseTimer -= Time.deltaTime;
        switch (enemyState)
        {
            case EnemyState.Idle:
                targetObject = gameObject;
                if(playerDetected)
                    enemyState = EnemyState.Chasing;
                break;

            case EnemyState.Chasing:
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
                if(rb.velocity == Vector2.zero)
                {
                    enemyState = EnemyState.Idle;
                    break;
                }
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

            case EnemyState.Hurt:
                sprite.color = Color.red;
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
        if(enemyState != EnemyState.Hurt)
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

    void CheckLiving()
    {
        Debug.Log("checkd");
        if(hp>0)
        {   
            enemyState = EnemyState.Idle;
            sprite.color = Color.white;
        }
        else
            Destroy(gameObject);
    }

    
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            bool LoS = true;

            //Checking Line of Sight on player
            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position,(other.transform.position-transform.position).normalized,dist);
            foreach(RaycastHit2D h in hitList)
            {
                if(h.transform.tag == "Player")
                    break;
                else if(h.transform.tag == "WallCollider")
                    LoS = false;
            }

            if(LoS)
                HandleDetection(other.gameObject, true, true);
            else
                HandleDetection(other.gameObject, true, false);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            HandleDetection(other.gameObject, false, false);
        }
    }

    private void OnDrawGizmos() {

        // To waypoint
        Gizmos.color = Color.red;
        Gizmos.DrawCube(waypoint,new Vector3(0.2f,0.2f,0.2f));
        Gizmos.DrawLine(transform.position,waypoint);
    }
}
