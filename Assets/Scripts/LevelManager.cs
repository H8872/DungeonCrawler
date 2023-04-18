using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    Camera mainCamera;
    Transform player;
    float levelHeight, levelWidth;
    [SerializeField] float tstx, tsty;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player").transform;
        if(player == null)
            Debug.LogError("No player found in " + this);
        levelHeight = mainCamera.orthographicSize * 2;
        levelWidth = levelHeight * 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        tstx = Mathf.Floor((player.position.x + levelWidth/2)/levelWidth);
        tsty = Mathf.Floor((player.position.y + levelHeight/2)/levelHeight);

        //MoveCamera(tstx, tsty);
    }
    void MoveCamera(float x, float y)
    {
        mainCamera.transform.position = new Vector3(x*levelWidth, y*levelHeight, -10f);
    }
}
