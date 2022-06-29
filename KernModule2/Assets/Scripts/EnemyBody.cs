using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;
using TMPro;

public class EnemyBody : IDamagable, IAttackable, IMergeable, IAgent, IGrabbable, IListener
{
    Player player;

    public bool isThrowing { get; set; } = true;
    public GameObject body { get; set; }
    public int Health { get; set; } = 1;
    public CollissionDetector collissionDetector { get; set; }
    public int Damage { get; set; } = 1;

    public Vector3 hand
    {
        get { return new Vector3(0.23f, 0.14f, 0.8f); }
        set { return; }
    }

    public Quaternion objectRotation
    {
        get { return Quaternion.Euler(-2.5f, 353, -3); }
        set { return; }
    }

    public float throwStrength { get; set; } = 30;
    public float grabRange { get; set; } = 2;
    public IThrowable nearestThrowable { get; set;}
    public IThrowable currentlyHeldThrowable { get; set; }

    public IAudible audibleClosest { get; set; }
    public float hearingRange { get; set; }


    public GameObject resultPrefab { get; set; }
    public IMergeable[] Requirements { get; set; }
    public ISplittable MergeResult { get; set; }


    public Vector3 currentTarget { get; set; }
    public NavMeshAgent agent { get; set; }
    public Vector3[] patrolPath { get; set; } = new Vector3[] { new Vector3(40, 0, -40), new Vector3(40, 0, 40), new Vector3(40, 0, -40), new Vector3(-40, 0, -40) };

    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }

    public TextMeshPro NodeText { get; set; }

    GameObject deathParticlesGameObject;

    Node tree;

    public EnemyBody(GameObject _body)
    {
        body = _body;
        collissionDetector = body.GetComponent<CollissionDetector>();
        agent = body.GetComponent<NavMeshAgent>();
        NodeText = body.GetComponentInChildren<TextMeshPro>();

        player = BlackBoard.player;

        resultPrefab = Resources.Load("EnemyModels/RiggedSkeleton") as GameObject;
        deathParticlesGameObject = Resources.Load("DeathParticles") as GameObject;

        tree =  new SuccessParallel(
                    new Selector(
                        new ListenForSound(this,
                            new Sequence(
                                new DisplayText(NodeText, "Hearing Sound"),
                                new MoveToNearestThrowable(this, this),
                                new Pickup(this),
                                new CheckThrowableType(this, typeof(EnemyHead),
                                    new Merge(this, this, typeof(EnemyHead)))
                                )
                            ),
                        new LookForThrowable(this, grabRange, 0.8f,
                            new Sequence(
                                new DisplayText(NodeText,"Throwing"),
                                new MoveToNearestThrowable(this, this),
                                new Pickup(this),
                                new Aim(this, player),
                                new WaitNode(1f),
                                new Throw(this, player),
                                new WaitNode(1f)
                                )
                            ),
                        new Interruptor(new ListenForSound(this), 
                            //new Selector(
                            //    new LookForTarget(0, 50, 0.8f, body, player,
                            //        new Sequence(
                            //            new MoveToTarget(this, player),
                            //            new Attack(this, player)
                            //            )
                            //        ),
                                new Interruptor(new LookForTarget(0, 50, 0.8f, body, player),
                                    new Sequence(
                                        new DisplayText(NodeText, "Random Walking"),
                                        new RandomlyWalk(this)
                                        )
                                    )
                            ),
                        new LookForTarget(0, 50, 0.8f, body, player,
                            new Sequence(
                                new Aim(this, player),
                                new WaitNode(1f),
                                new Throw(this, player),
                                new WaitNode(1f)))
                        ),
                    new CheckCollission(this,
                        new Sequence(
                            new DisplayText(NodeText, "I've Been hit"),
                            new BasicSpawnPrefab(deathParticlesGameObject, body, new Quaternion(0,0,0,0)),
                            new Drop(this),
                            new KillSpawnable(this)
                            )
                        )
                    );
        BlackBoard.AddSpawnable(this);
    }


    public bool LogicUpdate()
    {
        if (body == null)
            return false;

        position = body.transform.position;
        rotation = body.transform.rotation;

        tree.Run();
        return true;
    }

}
