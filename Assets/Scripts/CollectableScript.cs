using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScript : MonoBehaviour
{
    enum CollectType {Potion, Key, Door}
    enum CollectColor {White, Red, Green}
    [SerializeField] CollectType cType;
    [SerializeField] CollectColor cColor;
    [SerializeField] float value;
    Animator anim;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        if(cType == CollectType.Door)
        {
            anim = transform.GetComponent<Animator>();
            source = transform.GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player" && cType != CollectType.Door)
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            if(player != null)
            {
                if(cType == CollectType.Key)
                {
                    if(cColor == CollectColor.White)
                        player.AddKey(value); 
                    Destroy(gameObject);
                }
                else if(cType == CollectType.Potion)
                {
                    switch (cColor)
                    {
                        case CollectColor.White:
                            player.GiveInvuln(value);
                            Destroy(gameObject);
                            break;
                        case CollectColor.Red:
                            if(player.Hp < 4)
                            {
                                player.AddHP(value);
                                Destroy(gameObject);
                            }
                            break;
                        case CollectColor.Green:
                            player.AddDMG(value);
                            Destroy(gameObject);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.transform.tag == "Player" && !source.isPlaying)
        {
            PlayerScript player = other.transform.GetComponent<PlayerScript>();
            if(player == null)
                player = other.transform.parent.GetComponent<PlayerScript>();

            if(player.Keys >= value)
            {
                anim.Play("DoorAnim");
                source.Play();
            }
        }
    }
}
