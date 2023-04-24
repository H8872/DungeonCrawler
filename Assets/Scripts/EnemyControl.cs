using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyControl : MonoBehaviour
{
    enum EnemyState {Idle, Chasing, Hurt, Moving}
    [SerializeField] EnemyState enemyState;
    [SerializeField] float hp, moveSpeed;
    SpriteRenderer sprite;
    float chaseTimer, chaseMax = 2f, dist;
    GameObject targetObject;
    Vector3 waypoint, home;
    bool playerDetected = false, playerInRange = false;
    Rigidbody2D rb;
    AIPath ai;
    AIDestinationSetter dSetter;
    Transform moveTo;

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        moveTo = new GameObject().transform;
        moveTo.name = gameObject.name + "MoveTo";
        moveTo.position = transform.position;
        waypoint = transform.position;
        home = transform.position;
        rb = transform.GetComponent<Rigidbody2D>();
        dist = transform.GetComponent<CircleCollider2D>().radius;
        ai = transform.GetComponent<AIPath>();
        ai.maxSpeed = moveSpeed * 4;
        dSetter = transform.GetComponent<AIDestinationSetter>();
        enemyState = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                ai.canMove = true;
                targetObject = gameObject;
                chaseTimer -= Time.deltaTime;
                if(playerDetected)
                    enemyState = EnemyState.Chasing;
                if(chaseTimer<0)
                {
                    playerDetected = false;
                    moveTo.position = home;
                }
                break;

            case EnemyState.Chasing:
                ai.canMove = false;
                moveTo.position = targetObject.transform.position;
                if(playerInRange)
                {
                    chaseTimer = chaseMax;
                }
                else
                {
                    waypoint = targetObject.transform.position;
                    moveTo.position = targetObject.transform.position;
                    targetObject = null;
                    enemyState = EnemyState.Moving;
                }
                break;

            case EnemyState.Moving:
                ai.canMove = true;
                dSetter.target = moveTo;
                if(ai.reachedEndOfPath)
                    enemyState = EnemyState.Idle;
                if(playerInRange)
                {
                    enemyState = EnemyState.Chasing;
                    break;
                }
                break;

            case EnemyState.Hurt:
                ai.canMove = false;
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
        if(enemyState != EnemyState.Hurt && enemyState != EnemyState.Moving)
            rb.AddForce((waypoint-transform.position).normalized * moveSpeed,ForceMode2D.Impulse);

    }

    public void HandleDetection(GameObject other, bool inRange, bool LoS)
    {
        if(other.tag == "Player")
        {
            // If Line of Sight
            if(LoS)
            {
                targetObject = other;
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
        if(hp>0)
        {   
            enemyState = EnemyState.Idle;
            sprite.color = Color.white;
        }
        else
            Destroy(gameObject);
    }

    public void GetHit(float dmg, float kb, Vector3 dir)
    {
        if(enemyState != EnemyState.Hurt)
        {
            CancelInvoke("CheckLiving");
            Invoke("CheckLiving", 0.5f);
            hp -= dmg;
            enemyState = EnemyState.Hurt;
            rb.AddForce((transform.position-dir).normalized * kb,ForceMode2D.Impulse);
        }
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
