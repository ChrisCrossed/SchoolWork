using UnityEngine;
using System.Collections;

public enum EnemySpawnLocation
{
    North,
    South,
    East,
    West
}

public class Cs_EnemySpawnLogic : MonoBehaviour
{
    public EnemySpawnLocation enemySpawnLocation;

    public void SpawnEnemy()
    {
        Vector3 pos = gameObject.transform.position;
        Quaternion quat = gameObject.transform.rotation;

        Instantiate(Resources.Load("Enemy", typeof(GameObject)), pos, quat);

        Vector3 newPos = gameObject.transform.position;
        if (enemySpawnLocation == EnemySpawnLocation.North || enemySpawnLocation == EnemySpawnLocation.South)
        {
            newPos.x = Random.Range(-10, 11);
        }
        else if(enemySpawnLocation == EnemySpawnLocation.East || enemySpawnLocation == EnemySpawnLocation.West)
        {
            newPos.z = Random.Range(-10, 11);
        }

        gameObject.transform.position = newPos;
    }
}
