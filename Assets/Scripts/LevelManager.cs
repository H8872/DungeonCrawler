using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    Transform LevelsRoot;
    public Dictionary<int, Transform> stairsDict = new Dictionary<int, Transform>();
    Camera mainCamera;
    Transform player, cameraRoot;
    float cameraHeight, cameraWidth, xpos, ypos;
    public int currentFloor;
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
        player = GameObject.FindWithTag("Player").transform;
        if(player == null)
            Debug.LogWarning("No player found in " + this);
        LevelsRoot = GameObject.FindWithTag("Levels").transform;
        mainCamera = Camera.main;
        Screen.SetResolution(560,400,false);
        cameraHeight = mainCamera.orthographicSize * 2;
        cameraWidth = cameraHeight * mainCamera.aspect;
        cameraRoot = mainCamera.transform.parent;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Stairs"))
        {
            StairsScript st = obj.GetComponent<StairsScript>();
            if(!stairsDict.ContainsKey(st.id))
            {
                stairsDict.Add(st.id, obj.transform);
            }
            else
            {
                Debug.LogWarning($"Could not add stairs as stairId {st.id} already exists. Please fix, ty.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
            player = this.transform;
        if(Vector3.Distance(mainCamera.transform.parent.position,player.position)>0.2f)
            cameraRoot.position = Vector3.Lerp(mainCamera.transform.parent.position,player.position,6 * Time.deltaTime);
        //MoveCamera(tstx, tsty);
        if(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Joystick1Button7))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ChangeLevel(int level)
    {
        if(stairsDict.ContainsKey(level))
        {
            Transform target = stairsDict.GetValueOrDefault(level);
            cameraRoot.position = (cameraRoot.position - player.position) + target.position;
            player.position = target.position;
            target.GetComponent<StairsScript>().allowed = false;
            currentFloor = level;
        }
        else
        {
            Debug.LogWarning($"Could not find stairs with id {level}.");
        }

    }
}
