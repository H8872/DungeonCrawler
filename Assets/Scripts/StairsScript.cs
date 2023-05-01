using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsScript : MonoBehaviour
{
    public int id, goToFloor;
    LevelManager lvlManager;
    SpriteRenderer sprite;
    BoxCollider2D hitBox;
    public bool allowed = true;

    // Start is called before the first frame update
    void Start()
    {
        lvlManager = LevelManager.instance;
        sprite = gameObject.GetComponent<SpriteRenderer>();
        hitBox = gameObject.GetComponent<BoxCollider2D>();
        if(goToFloor>lvlManager.currentFloor)
        {
            sprite.flipX = true;
            transform.rotation = Quaternion.Euler(0,0,90);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.tag == "Player")
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            
            if(player.playerState != PlayerScript.PlayerState.Rolling)
            {
                if(allowed)
                {
                    lvlManager.ChangeLevel(goToFloor);
                    allowed = false;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player")
        {
            allowed = true;
        }
    }
}
