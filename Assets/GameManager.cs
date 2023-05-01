using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    LevelManager lvlManager;
    [SerializeField] GameObject playerPrefab;
    string fire1 = "Fire1", fire2 = "Fire2", fire3 = "Fire3", jump = "Jump";

    // Start is called before the first frame update
    void Awake() 
    {
        if(instance != null)
        {
            Debug.LogWarning($"More than 1 {instance} active. Deleting this {instance}.");
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        lvlManager = LevelManager.instance;
    }


    // Update is called once per frame
    void Update()
    {

    }

    /*
    ToDo:
    death handling
    boss fights
    secret hittable wall colliders
    ?life bars for enemies
    ?button test on main menu
    ?speech/intent bubbles
    ??slime fren tutorie
    ??use button
    ??seperate wall colliders for rolling to roll through enemies
    */
}
