using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;
using TMPro;

public class EnemyWithHeadAI : IDamagable, IAttackable, ISplittable, IAgent
{
    public bool ShouldSplit { get; set; }
    Player player;
    public GameObject body { get; set; }
    public int Health { get; set; } = 1;
    public CollissionDetector collissionDetector { get; set; }

    public int Damage { get; set; } = 1;

    public GameObject[] resultPrefabs { get; set; }

    public Vector3[] patrolPath { get; set; } = new Vector3[] { 
        new Vector3(Random.Range(-55f, 60f), 0, Random.Range(-55f, 60f)), 
        new Vector3(Random.Range(-55f, 60f), 0, Random.Range(-55f, 60f)),
        new Vector3(Random.Range(-55f, 60f), 0, Random.Range(-55f, 60f)),
        new Vector3(Random.Range(-55f, 60f), 0, Random.Range(-55f, 60f)) };
    public Vector3 currentTarget { get; set; }
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public NavMeshAgent agent { get; set; }
    public TextMeshPro NodeText { get; set; }

    GameObject deathParticlesGameObject;

    Node tree;

    public EnemyWithHeadAI(GameObject _body)
    {
        body = _body;
        agent = body.GetComponent<NavMeshAgent>();
        collissionDetector = body.GetComponent<CollissionDetector>();
        //NodeText = body.GetComponentInChildren<TextMeshPro>();


        resultPrefabs = new GameObject[]
        {
            Resources.Load("EnemyModels/SkeletonHead") as GameObject,
            Resources.Load("EnemyModels/RiggedSkeletonHeadless") as GameObject
        };
        deathParticlesGameObject = Resources.Load("DeathParticles") as GameObject;

        player = BlackBoard.player;

        tree = new SuccessParallel(
                    new LookForTarget(0, 50, 0.8f, body, player,
                        //new Selector(
                        //    new LookForTarget(0, 10, 0.8f, body, player,
                        //        new Sequence(
                        //            new MoveToTarget(this, player),
                        //            new Attack(this, player),
                        //            new WaitNode(2f)
                        //            )
                        //        ),
                            new LookForTarget(0, 50, 0.8f, body, player,
                                new Sequence(
                                    //new DisplayText(NodeText, "Splitting"),
                                    new Split(this, resultPrefabs)
                                    )
                                )
                           //)
                        ),
                    new Interruptor(new LookForTarget(0, 50, 0.8f, body, player),
                            new Sequence(
                               // new DisplayText(NodeText, "Patrolling"),
                                new MoveToTarget(this, patrolPath)
                            )
                        ),
                    new CheckCollission(this,
                        new Sequence(
                           // new DisplayText(NodeText, "I've Been hit"),
                            new BasicSpawnPrefab(deathParticlesGameObject, body, new Quaternion(0, 0, 0, 0)),
                            new KillSpawnable(this)
                            )
                        )
                    );

        BlackBoard.AddSpawnable(this);
    }

    // Update is called once per frame
    public bool LogicUpdate()
    {
        if (agent == null && body != null)
        {
            agent = body.GetComponent<NavMeshAgent>();
        }

        if (body == null)
            return false;

        position = body.transform.position;
        rotation = body.transform.rotation;

        tree.Run();

        return true;
    }

}
