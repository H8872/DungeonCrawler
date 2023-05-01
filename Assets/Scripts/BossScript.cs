using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField] GameObject stairsPrefab, lootDrop, spawnObject;
    EnemyControl control;
    public int stairId, stairGoTo, nAdds;
    public Vector3 lootPos;
    private void Start() {
        control = transform.GetComponent<EnemyControl>();
    }
    public void SpawnAdds()
    {
        for (int i = 0; i < nAdds; i++)
        {
            Vector2 pos = new Vector2(transform.position.x + Random.Range(-1f,1f),transform.position.y + Random.Range(-1f,1f));
            if(control.enemyType == EnemyControl.EnemyType.KnightBoss)
                pos = new Vector2(transform.position.x + Random.Range(-3f,3f),transform.position.y + Random.Range(-3f,3f));
            GameObject tmp = Instantiate(spawnObject, pos, transform.rotation);
            if(control.enemyType == EnemyControl.EnemyType.BossSlime)
                tmp.tag = "SlimeBossMinion";
            else if(control.enemyType == EnemyControl.EnemyType.KnightBoss)
                tmp.tag = "KnightBossMinion";
        }
    }
    public void SpawnLoot()
    {
        lootPos = control.Home;
        Instantiate(lootDrop, lootPos + Vector3.right, transform.rotation);
        StairsScript s = Instantiate(stairsPrefab, lootPos, transform.rotation).GetComponent<StairsScript>();
        s.id = stairId;
        s.goToFloor = stairGoTo;
        LevelManager.instance.stairsDict.Add(s.id, s.transform);
    }
}
