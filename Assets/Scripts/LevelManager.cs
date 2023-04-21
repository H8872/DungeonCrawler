using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    Camera mainCamera;
    Transform player;
    float levelHeight, levelWidth, xpos, ypos;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player").transform;
        if(player == null)
            Debug.LogError("No player found in " + this);
        levelHeight = mainCamera.orthographicSize * 2;
        levelWidth = levelHeight * mainCamera.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(mainCamera.transform.parent.position,player.position)>0.2f)
        mainCamera.transform.parent.position = Vector3.Lerp(mainCamera.transform.parent.position,player.position,6 * Time.deltaTime);
        //MoveCamera(tstx, tsty);
    }
}
