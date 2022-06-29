using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject playerBody;

    [SerializeField]
    GameObject[] torches;
    
    [SerializeField]
    GameObject[] numberPrefabs;
    
    [SerializeField]
    GameObject[] UINumbers;

    [SerializeField]
    GameObject[] UIModes;

    ParticleSystem[] torchParticles;
    ParticleSystemRenderer[] torchParticlerenderers;

    Player player;
    EnemySpawner enemySpawner;
    KillCounterAdder killAdder;
    int currentUIMode = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = new Player(playerBody);
        BlackBoard.SetPlayer(player);
        killAdder = new KillCounterAdder(numberPrefabs, UINumbers);

        AssertThrowables();

        enemySpawner = new EnemySpawner();

        for (int i = 0; i < 8; i++)
        {
            enemySpawner.spawnAgent();
        }


        //torchParticles = new ParticleSystem[torches.Length];
        //torchParticlerenderers = new ParticleSystemRenderer[torches.Length];
        //for(int i = 0; i < torches.Length; i++)
        //{
        //    torchParticles[i] = torches[i].transform.GetChild(0).GetComponent<ParticleSystem>();
        //    torchParticlerenderers[i] = torches[i].transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < BlackBoard.spawned.Count; i++)
        {
            bool checkIfDead = BlackBoard.spawned[i].LogicUpdate();

            if (!checkIfDead)
            {
                BlackBoard.spawned.Remove(BlackBoard.spawned[i]);
                i--;
            }
        }

        if(BlackBoard.spawned.Count < 5 + BlackBoard.killCount)
        {
            Debug.Log("spawning new enemy");
            enemySpawner.spawnAgent();
        }

        SwitchUIModes();
        player.LogicUpdate();
        killAdder.CounterUpdate();
        //RegulateParticles();
    }

    private void FixedUpdate()
    {
        player.PhysicsUpdate();
    }

    void AssertThrowables()
    {
        GameObject[] throwablesInScene = GameObject.FindGameObjectsWithTag("Throwable");

        foreach(GameObject throwable in throwablesInScene)
        {
            new SimpleThrowable(throwable);
        }
    }

    void SwitchUIModes()
    {
        if(player.Health <= 0)
        {
            currentUIMode = 3;
            UIModes[0].SetActive(false);
            UIModes[1].SetActive(false);
            UIModes[2].SetActive(false);
            UIModes[3].SetActive(true);
        }
        if(currentUIMode == 0 && Input.GetMouseButtonDown(1))
        {
            currentUIMode++;
            UIModes[0].SetActive(false);
            UIModes[1].SetActive(true);
        }

        if (currentUIMode == 1 && Input.GetMouseButtonDown(0))
        {
            currentUIMode++;
            UIModes[1].SetActive(false);
            UIModes[2].SetActive(true);
        }

        if(currentUIMode == 2 && player.Health <= 0)
        {
            currentUIMode++;
            UIModes[3].SetActive(false);
            UIModes[4].SetActive(true);
        }

        if(currentUIMode == 3 && Input.GetMouseButtonDown(0))
        {
            BlackBoard.ClearEverything();
            SceneManager.LoadScene(0);
        }
    }
}
