using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

namespace BehaviourTree
{
    //Base class for the Nodes
    public abstract class Node
    {
        //Every Node needs to be able to relay a result, of which there are three options
        public enum Result { success, failed, running }

        //Every Node also needs to be able to run
        public abstract Result Run();
    }



    //Selector
    public class Selector : Node
    {
        private Node[] children;
        private int currentIndex = 0;
        public Selector(params Node[] _children)
        {
            children = _children;
        }
        public override Result Run()
        {
            //The selector goes through its children one by one until a child returns success or running
            for (; currentIndex < children.Length; currentIndex++)
            {
                Result result = children[currentIndex].Run();

                switch (result)
                {
                    case Result.failed:
                        break;
                    case Result.success:
                        return Result.success;
                    case Result.running:
                        return Result.running;
                }
            }

            currentIndex = 0;
            return Result.failed;
        }
    }

    //Sequencer
    public class Sequence : Node
    {
        private Node[] children;
        private int currentIndex = 0;
        public Sequence(params Node[] _children)
        {
            children = _children;
        }
        public override Result Run()
        {

            //The sequence goes through all its children and stops when one returns failed
            for (; currentIndex < children.Length; currentIndex++)
            {
                Result result = children[currentIndex].Run();

                switch (result)
                {
                    case Result.failed:
                        currentIndex = 0;
                        return Result.failed;
                    case Result.success:
                        break;
                    case Result.running:
                        return Result.running;
                }
            }

            currentIndex = 0;
            return Result.success;
        }
    }

        //Parallel Nodes

    //Success version
    public class SuccessParallel : Node
    {
        private Node[] children;
        private int currentIndex = 0;
        public SuccessParallel(params Node[] _children)
        {
            children = _children;
        }
        public override Result Run()
        {

            //Success Parallel runs all its children simultaneously until one of them returns success, or all of them return failed
            int amountOfFailures = 0;
            for (; currentIndex < children.Length; currentIndex++)
            {
                Result result = children[currentIndex].Run();

                switch (result)
                {
                    case Result.failed:
                        amountOfFailures++;
                        break;
                    case Result.success:
                        return Result.success;
                    case Result.running:
                        break;
                }
            }

            if (amountOfFailures == children.Length)
                return Result.failed;

            currentIndex = 0;
            return Result.running;
        }
    }

    //Fail version
    public class FailParallel : Node
    {
        private Node[] children;
        private int currentIndex = 0;
        public FailParallel(params Node[] _children)
        {
            children = _children;
        }
        public override Result Run()
        {
            //Success Parallel runs all its children simultaneously until one of them returns failed, or all of them return success
            int amountOfSuccesses = 0;
            for (; currentIndex < children.Length; currentIndex++)
            {
                Result result = children[currentIndex].Run();

                switch (result)
                {
                    case Result.failed:
                        return Result.failed;
                    case Result.success:
                        amountOfSuccesses++;
                        break;
                    case Result.running:
                        break;
                }
            }

            if (amountOfSuccesses == children.Length)
                return Result.success;

            currentIndex = 0;
            return Result.running;
        }
    }

        //Conditional 

    //Base for conditional nodes
    public abstract class ConditionalNode : Node
    {
        //every conditional node needs a child it can run, and it can only be one child
        //if you want more children make the child a sequence or selector etc.
        private Node child;

        public override Result Run()
        {

            Result result = child.Run();

            return result;

        }
    }

    //Interruptor Node which checks conditional Nodes. These nodes will have two constructors,
    //one for Conditional node usage, and one for interruptor node usage
    public class Interruptor : Node
    {
        //The interruptor stops its child node when the condition of the given conditionalnode has been met

        ConditionalNode condition;
        Node child;

        public Interruptor(ConditionalNode _condition, Node _child)
        {
            condition = _condition;
            child = _child;
        }

        Node actionsUponExit;

        //Interruptor version that performs specific action(s) when interrupting
        public Interruptor(ConditionalNode _condition, Node _child, Node _actionsUponExit)
        {
            condition = _condition;
            child = _child;
            actionsUponExit = _actionsUponExit;
        }

        public override Result Run()
        {
            Result result = condition.Run();

            if (result == Result.success)
            {
                if (actionsUponExit != null)
                {
                    actionsUponExit.Run();
                    
                }
                return Result.failed;
            }

            result = child.Run();

            return result;
        }
    }
    
    //Conditional Node for checking if ISplittable wants or has to split
    //! replaced by checkBoolean for modularity
    public class CheckForSplit : ConditionalNode
    {        
        private Node child;
        ISplittable splittable;

        public CheckForSplit(Node _child, ISplittable _splittable)
        {
            child = _child;
            splittable = _splittable;
        }
        public override Result Run()
        {
            //Every Isplittable has a boolean that checks if it should split
            if (!splittable.ShouldSplit)
            {
                return Result.failed;
            }

            else
            {
                Result result = child.Run();

                return result;
            }
        }
    }

    //ConditionalNode which looks for specified Itarget
    public class LookForTarget : ConditionalNode
    {
        //child we want to run if the condition is met
        Node child;

        //The actor that wants to look for a target
        GameObject actor;

        //The target we're looking for
        ITargetable target;

        //How far can we see
        float minRange, maxRange;

        //what is the field of view? in radians
        float FOV;


        //First constructor for when you want to use the conditionalNode as a Node, running a child Node
        public LookForTarget(float _minRange, float _maxRange, float _FOV, GameObject _actor, ITargetable _target, Node _child)
        {
            child = _child;
            target = _target;
            minRange = _minRange;
            maxRange = _maxRange;
            actor = _actor;
            FOV = _FOV;
        }

        //Second constructor for when you want to use the conditionalNode as a condition for (as an example) an interruptor Node
        public LookForTarget(float _minRange, float _maxRange, float _FOV, GameObject _actor, ITargetable _target)
        {
            target = _target;


            minRange = _minRange;
            maxRange = _maxRange;
            actor = _actor;
            FOV = _FOV;
        }

        public override Result Run()
        {
            bool fovCheck = CheckFieldOfView();

            //If the fovCheck didn't see the target, the condition isn't met
            if (!fovCheck)
                return Result.failed;

            //if there is a child, that means you used the first constructor
            if (child != null)
            {
                Result result = child.Run();

                return result;
            }

            //if there is no child, that means you used the second constructor
            else
            {
                return Result.success;
            }
        }

        bool CheckFieldOfView()
        {
            float distanceFromTarget = Vector3.Distance(actor.transform.position, target.body.transform.position);

            if (distanceFromTarget < minRange || distanceFromTarget > maxRange)
                return false;

            RaycastHit hit;

            Vector3 RayPosition = actor.transform.position + actor.transform.forward + new Vector3(0,target.body.transform.position.y,0);
            Physics.Raycast(RayPosition, (target.body.transform.position - RayPosition).normalized, out hit);

            float dotProduct = Vector3.Dot(actor.transform.forward, (target.body.transform.position - RayPosition).normalized);

            //if the view of the target is obstructed, or it is not within the field of view
            if (hit.collider.gameObject != target.body || dotProduct < FOV)
                return false;

            return true;
        }
    }

    //Conditional node that checks for a boolean to be equal to a given value
    public class CheckBoolean : ConditionalNode
    {
        Node child;
        bool boolToCheck;
        bool boolValueWeWant;

        //Conditional Version
        public CheckBoolean(bool _boolToCheck, bool _boolValueWeWant,Node _child)
        {
            child = _child;
            boolToCheck = _boolToCheck;
            boolValueWeWant = _boolValueWeWant;
        }

        //Interruptor version
        public CheckBoolean(bool _boolToCheck, bool _boolValueWeWant)
        {
            boolToCheck = _boolToCheck;
            boolValueWeWant = _boolValueWeWant;
        }

        public override Result Run()
        {
            Debug.Log("running boolean: " + boolToCheck + ", " + boolValueWeWant);

            if (boolToCheck != boolValueWeWant)
            {
                return Result.failed;
            }


            if (child != null)
            {
                Result result = child.Run();

                return result;
            }

            return Result.success;
        }
    }

    public class CheckForThrow : ConditionalNode
    {
        Node child;
        IGrabbable grab;
        bool boolToCheck;
        bool boolValueWeWant;

        //Conditional Version
        public CheckForThrow(IGrabbable _grab, bool _boolValueWeWant, Node _child)
        {
            child = _child;
            grab = _grab;
            boolValueWeWant = _boolValueWeWant;
        }

        //Interruptor version
        public CheckForThrow(IGrabbable _grab, bool _boolValueWeWant)
        {
            grab = _grab;
            boolValueWeWant = _boolValueWeWant;
        }

        public override Result Run()
        {
            boolToCheck = grab.isThrowing;
            //Debug.Log("running boolean: " + boolToCheck + ", " + boolValueWeWant);

            if (boolToCheck != boolValueWeWant)
            {
                return Result.failed;
            }


            if (child != null)
            {
                Result result = child.Run();

                return result;
            }

            return Result.success;
        }
    }

    public class CheckIfHeld : ConditionalNode
    {
        Node child;
        IThrowable throwable;
        bool boolValueWeWant;
        bool boolToCheck;
        public CheckIfHeld(IThrowable _throwable, bool _boolValueWeWant, Node _child)
        {
            child = _child;
            throwable = _throwable;
            boolValueWeWant = _boolValueWeWant;
        } 
        
        public CheckIfHeld(IThrowable _throwable, bool _boolValueWeWant)
        {
            throwable = _throwable;
            boolValueWeWant = _boolValueWeWant;
        }

        public override Result Run()
        {
            boolToCheck = throwable.isBeingHeld;
            //Debug.Log("running boolean: " + boolToCheck + ", " + boolValueWeWant);

            if (boolToCheck != boolValueWeWant)
            {
                return Result.failed;
            }


            if (child != null)
            {
                Result result = child.Run();

                return result;
            }

            return Result.success;
        }
    }

    public class LookForThrowable : ConditionalNode
    {
        Node child;
        IGrabbable toGrab;
        float FOV;
        float grabRange;

        public LookForThrowable(IGrabbable _toGrab, float _grabRange, float _FOV, Node _child)
        {
            toGrab = _toGrab;
            grabRange = _grabRange; 
            FOV = _FOV; 
            child = _child;
        }

        public override Result Run()
        {
            float closestRange = grabRange;
            IThrowable toPickUp = null;

            for(int i = 0; i < BlackBoard.throwables.Count; i++)
            {
                float dotProduct = Vector3.Dot(toGrab.body.transform.forward, (BlackBoard.throwables[i].body.transform.position).normalized);
                float distanceFromObject = Vector3.Distance(toGrab.body.transform.position, BlackBoard.throwables[i].body.transform.position);

                if(dotProduct < FOV && distanceFromObject < closestRange)
                {
                    closestRange = distanceFromObject;
                    toPickUp = BlackBoard.throwables[i];
                }
            }

            if (toPickUp == null)
            {
                return Result.failed;
            }

            toGrab.nearestThrowable = toPickUp;

            Result result = child.Run();

            return result;
        }

    }

    public class ListenForSound : ConditionalNode
    {
        Node child;
        IListener listener;

        public ListenForSound(IListener _listener,Node _child)
        {
            child = _child;
            listener = _listener;
        }

        public ListenForSound(IListener _listener)
        {
            listener = _listener;
        }

        public override Result Run()
        {
            float closest = Mathf.Infinity;
            IAudible closestAudible = null;
            for(int i = 0; i < BlackBoard.currentSounds.Count; i++)
            {
                if(BlackBoard.currentSounds[i] == null || listener.body == null)
                {
                    break;
                }
                if(Vector3.Distance(listener.body.transform.position, BlackBoard.currentSounds[i].body.transform.position) < listener.hearingRange + BlackBoard.currentSounds[i].soundRange ||
                    Vector3.Distance(listener.body.transform.position, BlackBoard.currentSounds[i].body.transform.position) < closest)
                {
                    closestAudible = BlackBoard.currentSounds[i];
                }
            }

            listener.audibleClosest = closestAudible;

            if(closestAudible == null)
            {
                return Result.failed;
            }

            if (child != null)
            { 
                Result result = child.Run();

                return result;
            }

            return Result.success;
        }
    }

    public class CheckThrowableType : ConditionalNode
    {
        IGrabbable grabbable;
        IThrowable throwable;
        System.Type type;
        Node child;
        public CheckThrowableType(IGrabbable _grabbable, System.Type _type, Node _child)
        {
            grabbable = _grabbable;
            type = _type;
            child = _child;
        }

        public override Result Run()
        {
            throwable = grabbable.currentlyHeldThrowable;

            if (throwable.GetType() != type)
            {
                Debug.Log(throwable.GetType() + " is not the same as " + type);
                return Result.failed;
            }

            if (child != null)
            {
                Result result = child.Run();

                return result;
            }

            return Result.success;
        }
    }

    public class CheckCollission : ConditionalNode
    {
        IDamagable damagable;
        Node child;
        public CheckCollission(IDamagable _damagable)
        {
            damagable = _damagable;
        }

        public CheckCollission(IDamagable _damagable, Node _child)
        {
            damagable = _damagable;
            child = _child;
        }

        public override Result Run()
        {
            if(!damagable.collissionDetector.collisionBool || damagable.collissionDetector.collisionSpeed < 5)
            {
                //if (damagable.collissionDetector.collisionBool)
                  //  Debug.Log(damagable.collissionDetector.collisionSpeed);

                return Result.failed;
            }

            if (child != null)
            {
                Result result = child.Run();

                return result;
            }

            return Result.success;
        }
    }

        //Leafs


    public class MakeASound : Node
    {
        IAudible audible;
        public MakeASound(IAudible _audible)
        {
            audible = _audible;
        }
        public override Result Run()
        {
            audible.makingSound = true;
            BlackBoard.AddSound(audible);

            return Result.success;
        }
    }

    public class StopMakingASound : Node
    {
        IAudible audible;
        public StopMakingASound(IAudible _audible)
        {
            audible = _audible;
        }
        public override Result Run()
        {
            audible.makingSound = false;
            BlackBoard.RemoveSound(audible);

            return Result.success;
        }
    }
    //Node that sets a target for an IAgent
    public class SetTarget : Node
    {
        Vector3 target;
        IAgent agent;
        public SetTarget(IAgent _agent, ITargetable _target)
        {
            agent = _agent;
            target = _target.body.transform.position;
        }
        public override Result Run()
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas))
            {
                Debug.Log("Setting new path");
                agent.currentTarget = target;
                return Result.success;
            }

            Debug.Log("I can't walk to that target");
            return Result.failed;
        }
    }
    
    //Node that moves an IAgent to a specified Vector3 target
    public class MoveToTarget : Node
    {
        NavMeshAgent actor;
        ITargetable target;
        Vector3 vectorTarget;
        
        Vector3[] points;
        int currentIndex = 0;


        public MoveToTarget(IAgent _agent, ITargetable _target)
        {
            actor = _agent.agent;
            target = _target;
        }

        public MoveToTarget(IAgent _agent, params Vector3[] _points)
        {
            actor = _agent.agent;

            points = _points;
            currentIndex = 0;
        }

        public void SetPath()
        {
            if (target != null)
            {
                vectorTarget = target.body.transform.position;
            }

            else if(points.Length > 0)
            {
                vectorTarget = points[currentIndex];
            }
            actor.SetDestination(vectorTarget);
        }

        public override Result Run()
        {
            SetPath();
            if (actor == null || actor.path.status == NavMeshPathStatus.PathInvalid)
            {
                return Result.failed;
            }

            if (Vector3.Distance(actor.transform.position, vectorTarget) < 1f)
            {
                currentIndex++;
                if(points != null && currentIndex >= points.Length)
                {
                    currentIndex = 0;
                }

                return Result.success;
            }

            return Result.running;
        }
    }

    public class MoveToNearestThrowable : Node
    {
        NavMeshAgent actor;
        ITargetable target;
        IGrabbable grabbable;
        Vector3 vectorTarget;


        public MoveToNearestThrowable(IAgent _agent, IGrabbable _grabbable)
        {
            actor = _agent.agent;
            grabbable = _grabbable;
        }

        public void SetPath()
        {
            target = grabbable.nearestThrowable;
            vectorTarget = target.body.transform.position;
            actor.SetDestination(vectorTarget);
        }

        public override Result Run()
        {
            if (grabbable.nearestThrowable == null)
            {
                return Result.failed;
            }
            SetPath();

            if (actor == null || actor.path.status == NavMeshPathStatus.PathInvalid || target == null)
            {
                return Result.failed;
            }

            if (Vector3.Distance(actor.transform.position, vectorTarget) < grabbable.grabRange)
            {
                return Result.success;
            }

            return Result.running;
        }
    }

    public class MoveForward : Node
    {
        Vector3 targetLocation;
        bool destinationSet;
        IAgent agent;
        public MoveForward(IAgent _agent)
        {
            agent = _agent;
            destinationSet = false;
        }

        public override Result Run()
        {
            if (!destinationSet)
            {
                Debug.Log("setting destination");
                RaycastHit hit;

                Physics.Raycast(agent.body.transform.position, agent.body.transform.forward, out hit);

                targetLocation = hit.point;

                agent.agent.SetDestination(targetLocation);
                destinationSet = true;

                Debug.Log(hit.collider.gameObject.name);

            }

            if(agent.agent.path.status == NavMeshPathStatus.PathInvalid)
            {
                destinationSet = false;
                return Result.failed;
            }


            if(agent.agent.remainingDistance < 2f)
            {
                destinationSet = false;
                return Result.success;
            }

            Debug.Log(targetLocation + ", " + agent.body.transform.position);
            return Result.running;
        }
    }

    public class RandomlyWalk : Node
    {
        Vector3 targetLocation;
        bool destinationSet;
        IAgent agent;
        public RandomlyWalk(IAgent _agent)
        {
            agent = _agent;
            destinationSet = false;
        }
        public override Result Run()
        {
            if (!destinationSet)
            {
                RaycastHit hit;

                Physics.Raycast(agent.body.transform.position, new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), out hit);

                targetLocation = hit.point;

                agent.agent.SetDestination(targetLocation);
                destinationSet = true;
            }

            if (agent.agent.path.status == NavMeshPathStatus.PathInvalid)
            {
                destinationSet = false;
                return Result.failed;
            }

            if (agent.agent.remainingDistance < 1f)
            {
                destinationSet = false;
                return Result.success;
            }

            return Result.running;
        }
    }

    public class Rotate : Node
    {
        GameObject actor;       
        bool running;
        float rotationAmount = 0;
        float speed = 1f;

        public Rotate(GameObject _actor)
        {
            actor = _actor;
        }

        public override Result Run()
        {
            if(running == false)
            {
                rotationAmount = Random.Range(120f, 210f);
                rotationAmount = actor.transform.rotation.eulerAngles.y + rotationAmount;
                running = true;
            }
            if (running == true)
            {
                Vector3 newRotation = new Vector3(actor.transform.rotation.eulerAngles.x, rotationAmount, actor.transform.rotation.eulerAngles.z);
                actor.transform.rotation = Quaternion.Euler(Vector3.Lerp(actor.transform.rotation.eulerAngles, newRotation, speed));

                if (Vector3.Distance(actor.transform.rotation.eulerAngles, newRotation) < 0.1f)
                {
                    rotationAmount = 0;
                    running = false;
                    return Result.success;
                }
            }
            return Result.running;
        }
    }


    //Node for an IAttackable to attack an IDamagable
    public class Attack : Node
    {
        IDamagable damageTarget;
        IAttackable attacker;
        public Attack(IAttackable _attacker ,IDamagable _damageTarget)
        {
            damageTarget = _damageTarget;
            attacker = _attacker;
        }

        public override Result Run()
        {
            if (damageTarget == null || attacker == null)
                return Result.failed;

            damageTarget.Health -= attacker.Damage;
            return Result.success;
        }
    }

    //Node for an IGrabbable to pick up an IThrowable
    public class Pickup : Node
    {
        IThrowable pickedUp;
        IGrabbable grabbing;
        public Pickup(IGrabbable _grabbing)
        {
            grabbing = _grabbing;
        }

        public override Result Run()
        {

            pickedUp = grabbing.nearestThrowable;

            if (pickedUp.isBeingHeld)
            {
                return Result.failed;
            }

            pickedUp.body.transform.SetParent(grabbing.body.transform);

            pickedUp.body.transform.localPosition = grabbing.hand;
            pickedUp.body.transform.localRotation = grabbing.objectRotation;
            pickedUp.rb.isKinematic = true;

            pickedUp.isBeingHeld = true;
            grabbing.currentlyHeldThrowable = pickedUp;

            return Result.success;
        }
    }

    public class Aim : Node
    {
        IGrabbable grabbable;
        ITargetable target;

        public Aim(IGrabbable _grabbable, ITargetable _target)
        {
            grabbable = _grabbable;
            target = _target;
        }

        public override Result Run()
        {
            Vector3 direction = target.body.transform.position - grabbable.body.transform.position;
            grabbable.body.transform.forward = direction.normalized;
            return Result.success;
        }
    }

    //Node for an IGrabbable to drop the IThrowable they're holding
    public class Drop : Node
    {
        IGrabbable dropping;
        public Drop(IGrabbable _dropping)
        {
            dropping = _dropping;
        }
        public override Result Run()
        {
            if (dropping.currentlyHeldThrowable != null)
            {
                IThrowable throwable = dropping.currentlyHeldThrowable;
                throwable.isBeingHeld = false;
                throwable.body.transform.SetParent(null);
                throwable.rb.isKinematic = false;
            }

            return Result.success;
        }
    }

    //Node for an IGrabbable to throw the IThrowable they're holding
    public class Throw : Node
    {
        IThrowable toThrow;
        ITargetable target;
        IGrabbable grabbed;

        public Throw(IGrabbable _grabbed, ITargetable _target)
        {
            grabbed = _grabbed;
            target = _target;
        }

        public override Result Run()
        {
            toThrow = grabbed.currentlyHeldThrowable;

            if(toThrow == null)
            {
                return Result.failed;
            }

            toThrow.body.transform.SetParent(null);
            toThrow.rb.isKinematic = false;

            Vector3 direction =  (target.body.transform.position - grabbed.body.transform.position).normalized;
            toThrow.rb.AddForce(direction * grabbed.throwStrength, ForceMode.Impulse);
            toThrow.isBeingHeld = false;
            grabbed.isThrowing = false;
            grabbed.currentlyHeldThrowable = null;

            return Result.success;
        }
    }
    
    //Node for an ISplittable to split into multiple IMergables
    public class Split : Node
    {
        //If I theoretically want to expand this even more, I could incorporate ISplittable into the possibilities, like a matryoshka
        GameObject[] splitPartsPrefabs;
        ISplittable original;
        public Split(ISplittable _original, GameObject[] _splitPartsPrefabs)
        {
            original = _original;
            splitPartsPrefabs = _splitPartsPrefabs;
        }

        public override Result Run()
        { 

            if(original == null)
            {
                return Result.failed;
            }

            //split the ISplittable into multiple parts

            for(int i = 0; i < splitPartsPrefabs.Length; i++)
            {
                GameObject newBody = Object.Instantiate(splitPartsPrefabs[i], original.body.transform.position, original.body.transform.rotation);
                switch (splitPartsPrefabs[i].name)
                {
                    case "SkeletonHead":
                        EnemyHead temp = new EnemyHead(newBody);
                        newBody.transform.position = original.body.transform.position + Vector3.up * 0.72f + original.body.transform.forward;
                        break;

                    case "RiggedSkeletonHeadless":
                        new EnemyBody(newBody);
                        break;
                }
            }

            BlackBoard.RemoveSpawnable(original);

            Object.Destroy(original.body);

            //disable or delete the original

            return Result.success;
        }
    }

    public class Merge : Node
    {
        IMergeable[] mergeables;

        IMergeable firstMergeable;
        IGrabbable grabbable;
        System.Type mergeType;

        //skeleton variant, should make other constructors if I make other entities that can merge
        public Merge(IMergeable _firstMergeable, IGrabbable _grabbable, System.Type _mergeType)
        {
            firstMergeable = _firstMergeable;
            grabbable = _grabbable;
            mergeType = _mergeType;
        }
        public override Result Run()
        {
            if (grabbable.currentlyHeldThrowable != null && grabbable.currentlyHeldThrowable.GetType() == mergeType)
            {
                mergeables = new IMergeable[] { firstMergeable, grabbable as IMergeable };
            }

            if(mergeables == null || mergeables.Length <= 1)
            {
                return Result.failed;
            }

            for(int i = 0; i < mergeables.Length - 1; i++)
            {
                if(mergeables[i].resultPrefab.name != mergeables[i + 1].resultPrefab.name)
                {
                    return Result.failed;
                }
            }

            GameObject newBody = Object.Instantiate(mergeables[0].resultPrefab, mergeables[0].body.transform.position, mergeables[0].body.transform.rotation);

            switch (mergeables[0].resultPrefab.name)
            {
                case "RiggedSkeleton":
                    EnemyWithHeadAI temp = new EnemyWithHeadAI(newBody);
                    newBody.transform.position = mergeables[0].body.transform.position + Vector3.up * 0.72f + mergeables[0].body.transform.forward;
                    break;
            }

            foreach(IMergeable mergeable in mergeables)
            {
                BlackBoard.RemoveSpawnable(mergeable);

                Object.Destroy(mergeable.body);
            }

            return Result.success;
        }
    }

    public class KillSpawnable : Node
    {
        ISpawnable spawnable;
        public KillSpawnable(ISpawnable _spawnable)
        {
            spawnable = _spawnable;
        }

        public override Result Run()
        {
            BlackBoard.AddKill();
            BlackBoard.RemoveSpawnable(spawnable);
            Object.Destroy(spawnable.body);
            return Result.success;
        }
    }

    //Node to send a debug message to the console
    public class DebugMessage : Node
    {
        string message;
        public DebugMessage(string _message)
        {
            message = _message;
        }
        public override Result Run()
        {
            Debug.Log(message);
            return Result.success;
        }
    }

    //Node to wait for a given amount of seconds
    public class WaitNode : Node
    {
        private float waitTime;
        private float currentTime;

        public WaitNode(float _waitTime)
        {
            waitTime = _waitTime;
        }
        public override Result Run()
        {
            currentTime += Time.deltaTime;

            if(currentTime >= waitTime)
            {
                currentTime = 0;
                return Result.success;
            }

            //Debug.Log(currentTime + ", " + waitTime);
            return Result.running;
        }
    }

    public class DisplayText : Node
    {
        TextMeshPro editText;
        string textToDisplay;
        public DisplayText(TextMeshPro _editText, string _textToDisplay)
        {
            editText = _editText;
            textToDisplay = _textToDisplay;
        }

        public override Result Run()
        {
            editText.text = textToDisplay;
            return Result.success;
        }
    }

    public class PlaySoundFile : Node
    {
        public PlaySoundFile()
        {

        }
        public override Result Run()
        {
            return Result.failed;

        }
    }

    public class PlayParticleSystem : Node
    {
        ParticleSystem particles;
        public PlayParticleSystem(ParticleSystem _particles)
        {
            particles = _particles;
        }
        public override Result Run()
        {
            particles.Play();
            return Result.success;
        }
    }

    public class StopParticleSystem : Node
    {
        ParticleSystem particles;
        public StopParticleSystem(ParticleSystem _particles)
        {
            particles = _particles;
        }
        public override Result Run()
        {
            Debug.Log("Stop particles");
            particles.Stop();
            return Result.success;
        }
    }

    public class PlayAnimation : Node
    {
        Animator animator;
        string animation;
        public PlayAnimation(Animator _animator, string _animation)
        {
            animator = _animator;
            animation = _animation;
        }
        public override Result Run()
        {
            animator.Play(animation);
            return Result.success;

        }
    }

    public class BasicSpawnPrefab : Node
    {
        GameObject prefab;
        Vector3 location;
        GameObject activeLocation;
        Quaternion rotation;
        public BasicSpawnPrefab(GameObject _prefab, Vector3 _location, Quaternion _rotation)
        {
            prefab = _prefab;
            location = _location;
            rotation = _rotation;
        }

        public BasicSpawnPrefab(GameObject _prefab, GameObject _activeLocation, Quaternion _rotation)
        {
            prefab = _prefab;
            activeLocation = _activeLocation;
            rotation = _rotation;
        }

        public override Result Run()
        {
            if(activeLocation != null)
            {
                location = activeLocation.transform.position;
            }

            Object.Instantiate(prefab, location, rotation);
            return Result.success;
        }
    }

}
