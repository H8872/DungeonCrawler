using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    Camera mainCamera;
    Transform player;
    float cameraHeight, cameraWidth, xpos, ypos;
    int currentFloor;
    // Start is called before the first frame update
    void Awake() {
        
        if(instance != null)
        {
            Debug.LogWarning($"More than 1 {instance} active. Deleting this {instance}.");
            Destroy(this);
        }
        instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
        Screen.SetResolution(560,400,false);
        player = GameObject.FindWithTag("Player").transform;
        if(player == null)
            Debug.LogError("No player found in " + this);
        cameraHeight = mainCamera.orthographicSize * 2;
        cameraWidth = cameraHeight * mainCamera.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCamera == null)
        {
            Start();
        }
        if(player == null)
            player = this.transform;
        if(Vector3.Distance(mainCamera.transform.parent.position,player.position)>0.2f)
        mainCamera.transform.parent.position = Vector3.Lerp(mainCamera.transform.parent.position,player.position,6 * Time.deltaTime);
        //MoveCamera(tstx, tsty);
    }

    public void ChangeLevel(int level)
    {

    }
}
