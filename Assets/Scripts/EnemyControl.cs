using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] bool drawGizmos;
    enum EnemyState {Idle, Chasing, Hurt, Moving}
    public enum EnemyType {Bat, Slime, BossBat, BossSlime, KnightBoss}
    [SerializeField] EnemyState enemyState;
    public EnemyType enemyType;
    public float Damage {get{return damage;} set{damage = value;}}
    public float KnockBack {get{return knockBack;} set{knockBack = value;}}
    [SerializeField] float hp, moveSpeed, damage, knockBack;
    SpriteRenderer sprite;
    CircleCollider2D hitCollider;
    [SerializeField] AudioClip hitClip;
    AudioSource aSource;
    float chaseTimer, chaseMax = 2f, dist;
    [SerializeField] float bossCounter = 10;
    GameObject targetObject;
    Vector3 waypoint;
    public Vector3 Home;
    bool playerDetected = false, playerInRange = false;
    Rigidbody2D rb;
    AIPath ai;
    AIDestinationSetter dSetter;
    Transform moveTo;
    BossScript boss;

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        aSource = transform.GetComponent<AudioSource>();
        if(aSource == null)
            aSource = gameObject.AddComponent<AudioSource>();
        hitCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
        moveTo = new GameObject().transform;
        moveTo.name = gameObject.name + "MoveTo";
        moveTo.position = transform.position;
        waypoint = transform.position;
        Home = transform.position;
        rb = transform.GetComponent<Rigidbody2D>();
        if(transform.GetComponent<CircleCollider2D>() != null)
            dist = transform.GetComponent<CircleCollider2D>().radius;
        else
            dist = 8;
        ai = transform.GetComponent<AIPath>();
        ai.maxSpeed = moveSpeed * 4;
        
        dSetter = transform.GetComponent<AIDestinationSetter>();
        enemyState = EnemyState.Idle;
        if(enemyType == EnemyType.BossBat || enemyType == EnemyType.BossSlime)
        {
            bossCounter = 5;
            boss = GetComponent<BossScript>();
            transform.localScale = new Vector3(2, 2, 2);
            moveSpeed /= 2;
            knockBack *= 2;
            hp *= 4;
        }
        if(enemyType == EnemyType.KnightBoss)
        {
            boss = GetComponent<BossScript>();
            bossCounter = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
            switch (enemyState)
            {

                case EnemyState.Idle:
                    ai.canMove = true;
                    rb.velocity = Vector2.zero;
                    targetObject = gameObject;
                    chaseTimer -= Time.deltaTime;
                    if(playerDetected && playerInRange)
                        enemyState = EnemyState.Chasing;
                    if(chaseTimer<=0 && playerDetected)
                    {
                        playerDetected = false;
                        moveTo.position = Home;
                        enemyState = EnemyState.Moving;
                    }
                    else if(chaseTimer<=0)
                    {
                        chaseTimer = chaseMax*2;
                        if(moveTo.position == Home)
                            moveTo.position = moveTo.position + new Vector3(Random.Range(-1f,1f),Random.Range(-1,1),0);
                        else
                            moveTo.position = Home;
                        enemyState = EnemyState.Moving;
                    }
                    break;

                case EnemyState.Chasing:
                    if(targetObject == null)
                    {
                        enemyState = EnemyState.Moving;
                        break;
                    }
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
                    if(playerInRange)
                    {
                        enemyState = EnemyState.Chasing;
                        break;
                    }
                    if(ai.velocity.magnitude < 0.1f)
                    {
                        chaseTimer -= Time.deltaTime;
                        if(chaseTimer <= 0)
                            enemyState = EnemyState.Idle;
                    }
                    ai.canMove = true;
                    rb.velocity = Vector2.zero;
                    dSetter.target = moveTo;
                    if(ai.reachedEndOfPath)
                    {
                        enemyState = EnemyState.Idle;
                        waypoint = transform.position;
                    }
                    break;

                case EnemyState.Hurt:
                    ai.canMove = false;
                    sprite.color = (Time.time % 0.2f < 0.1f) ? Color.red : Color.white;
                    hitCollider.enabled = false;
                    break;

                default:
                    break;
            }
            if(targetObject != null)
            {
                waypoint = targetObject.transform.position;
            }
            if(ai.velocity.x < 0 || rb.velocity.x < 0)
                sprite.flipX = true;
            else
                sprite.flipX = false;
            if(enemyType == EnemyType.KnightBoss && playerDetected)
            {
                if(bossCounter>0)
                    bossCounter -= Time.deltaTime;
                else
                {
                    boss.SpawnAdds();
                    bossCounter = 3;
                }
            }
    }

    private void FixedUpdate()
    {
        ai.maxSpeed = moveSpeed * 4;
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
        hitCollider.enabled = true;
        if(hp>0)
        {   
            if(boss != null)
            {
                if(enemyType != EnemyType.KnightBoss)
                    moveSpeed += 0.1f;
                if(enemyType == EnemyType.BossSlime && hp < 40f)
                {
                    boss.SpawnAdds();
                    hitCollider.enabled = false;
                    sprite.enabled = false;
                }
            }
            enemyState = EnemyState.Idle;
            sprite.color = Color.white;
        }
        else
        {
            if(boss != null)
            {
                boss.SpawnLoot();
            }
            else if(gameObject.tag == "SlimeBossMinion")
            {
                GameObject.FindWithTag("SlimeBoss").GetComponent<EnemyControl>().DamageSlimeBoss();
            }
            else if(gameObject.tag == "KnightBossMinion")
            {
                if(GameObject.FindWithTag("KnightBoss") != null)
                    GameObject.FindWithTag("KnightBoss").GetComponent<EnemyControl>().GetHit(5, 0, Vector3.zero);
            }
            Destroy(gameObject);
        }
    }
    //quick n dirty :)
    public void DamageSlimeBoss()
    {
        bossCounter--;
        if(bossCounter<=0 && enemyType == EnemyType.BossSlime)
        {
            hp = 0;
            CheckLiving();
        }
    }

    public void GetHit(float dmg, float kb, Vector3 dir)
    {
        if(enemyState != EnemyState.Hurt)
        {
            aSource.clip = hitClip;
            aSource.Play();
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
                else if(h.transform.tag == "WallCollider" && enemyType != EnemyType.KnightBoss)
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
        if(drawGizmos)
        {
            // To waypoint
            Gizmos.color = Color.red;
            Gizmos.DrawCube(waypoint,new Vector3(0.2f,0.2f,0.2f));
            Gizmos.DrawLine(transform.position,waypoint);
        }
    }
}
