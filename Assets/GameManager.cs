using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum ScripType {MatrixOfAssets, EnemyNames, None}
    [SerializeField] ScripType scripType = ScripType.None;
    [SerializeField] List<string> nameList = new List<string>();
    [SerializeField] int length, width;
    [SerializeField] GameObject cube;


    // Start is called before the first frame update
    void Start()
    {
        // Excersizes (Delete references on final)
        switch (scripType)
        {
            case ScripType.MatrixOfAssets:
                for (int x = 0; x < length; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        Instantiate(cube, new Vector3(x,y,0), transform.rotation);
                    }
                }
                break;
            case ScripType.EnemyNames:
                for (int i = 0; i < length; i++)
                {
                    AddName("Name"+i);
                }
                for (int i = 0; i < width; i++)
                {
                    RemoveLastName();
                }
                foreach(string s in nameList)
                {
                    if(s[0] == 'S' || s[0] == 's')
                    {
                        Debug.Log(s);
                    }
                }
                break;
            default:
                break;
        }

        // Game Code

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void AddName(string name)
    {
        nameList.Add(name);
        Debug.Log($"Added {name}. Now there are {nameList.Count} names.");
    }

    void RemoveLastName()
    {
        if(nameList.Count>0)
        {
            Debug.Log($"Removed {nameList[nameList.Count-1]}");
            nameList.RemoveAt(nameList.Count-1);
        }
        else
            Debug.Log("No Names to remove");
    }
}
