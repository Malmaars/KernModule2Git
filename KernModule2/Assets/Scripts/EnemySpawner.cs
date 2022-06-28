using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public ISpawnable spawnAgent()
    {

        Vector3 spawnLocation = Vector3.zero;
        NavMeshHit hit;
        while (spawnLocation == Vector3.zero || !NavMesh.SamplePosition(spawnLocation, out hit, 1f, NavMesh.AllAreas))
        {
            spawnLocation = new Vector3(Random.Range(-55f, 60f), 0, Random.Range(-55f, 60f));
        }

        GameObject whatToSpawn = Instantiate(Resources.Load("EnemyModels/RiggedSkeleton"), spawnLocation, new Quaternion(0, 0, 0, 0)) as GameObject;
        ISpawnable thisSpawnable = new EnemyWithHeadAI(whatToSpawn);
        
        //GameObject temp = Instantiate(whatToSpawn, spawnLocation, new Quaternion(0, 0, 0, 0));

        return thisSpawnable;
    }
}
