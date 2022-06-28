using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyHead : IMergeable, IThrowable, IAudible
{
    Player player;

    public Rigidbody rb { get; set; }
    public bool isBeingHeld { get; set; } = false;

    public CollissionDetector collissionDetector { get; set; }

    public GameObject body { get; set; }

    public GameObject resultPrefab { get; set; }
    public IMergeable[] Requirements { get; set; }
    public ISplittable MergeResult { get; set; }

    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }

    public float soundRange {get;set; }
    public bool makingSound { get; set; }

    ParticleSystem screamParticles;

    Node tree;

    public EnemyHead(GameObject _body)
    {
        body = _body;
        rb = body.GetComponent<Rigidbody>();
        collissionDetector = body.GetComponent<CollissionDetector>();
        player = BlackBoard.player;

        resultPrefab = Resources.Load("EnemyModels/RiggedSkeleton") as GameObject;
        BlackBoard.AddSpawnable(this);
        BlackBoard.AddThrowable(this);
        screamParticles = body.GetComponentInChildren<ParticleSystem>();
        Debug.Log(screamParticles);
        //screamParticles.Play();

        tree = new Selector(
                    new Interruptor(new CheckIfHeld(this, true),
                        new Sequence(
                            new WaitNode(6f),
                            new MakeASound(this),
                            new PlayParticleSystem(screamParticles),
                            new WaitNode(6f),
                            new StopParticleSystem(screamParticles),
                            new StopMakingASound(this)
                            ),
                        new StopParticleSystem(screamParticles),
                        new StopMakingASound(this)
                        )
                    );
    }

    public bool LogicUpdate()
    {
        if (body == null)
        {
            if (BlackBoard.throwables.Contains(this))
            {
                BlackBoard.RemoveThrowable(this);
            }

            if (BlackBoard.currentSounds.Contains(this))
            {
                BlackBoard.RemoveSound(this);
            }
            return false;
        }

        position = body.transform.position;
        rotation = body.transform.rotation;

        tree.Run();

        return true;
    }
}
