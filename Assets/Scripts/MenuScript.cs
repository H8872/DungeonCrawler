using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    GameManager GM;
    LevelManager LM;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        LM = LevelManager.instance;
        Screen.SetResolution(1120,800,false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            SceneManager.LoadScene("DungeonScene");
        }
    }
}
