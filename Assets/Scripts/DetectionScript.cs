using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionScript : MonoBehaviour
{
    EnemyControl control;
    float dist;

    Vector3 temp;
    // Start is called before the first frame update
    void Start()
    {
        control = transform.parent.GetComponent<EnemyControl>();
        dist = transform.GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            bool LoS = true;

            temp = other.transform.position;

            //Checking Line of Sight
            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position,(other.transform.position-transform.position).normalized,dist);
            foreach(RaycastHit2D h in hitList)
            {
                if(h.transform.tag == "WallCollider")
                    LoS = false;
            }

            if(LoS)
                control.HandleDetection(other.gameObject, true, true);
            else
                control.HandleDetection(other.gameObject, true, false);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player")
        {
            control.HandleDetection(other.gameObject, false, false);
        }
    }
    
    /*private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,temp);
    }*/
}
