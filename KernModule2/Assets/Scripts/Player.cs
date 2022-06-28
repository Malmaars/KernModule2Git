using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BehaviourTree;

public class Player : MonoBehaviour, ITargetable, IDamagable, IGrabbable
{
    float mouseSensitivity = 200f;
    float xRotation = 0f;
    float yRotation = 0f;
    float moveSpeed = 10f;
    Camera playerCamera;

    public bool isThrowing { get; set; }
    public IThrowable nearestThrowable { get; set; }
    public IThrowable currentlyHeldThrowable { get; set; }
    public CollissionDetector collissionDetector { get; set; }


    public GameObject body { get; set; }
    public Vector3 hand { 
        get { return new Vector3(0.89f, -0.65f, 1.6f); }
        set { return;  } }

    public Quaternion objectRotation
    {
        get { return Quaternion.Euler(-19, 200, -7); }
        set { return; }
    }

    public float throwStrength { get; set; } = 30;

    public float grabRange { get; set; } = 4f;
    public int Health { get; set; } = 1;
    int healthLastFrame = 100;

    Vector3 playerVelocity;
    float FOV = 1f;
    public Player(GameObject _body)
    {
        currentlyHeldThrowable = null;
        body = _body;
        collissionDetector = body.GetComponent<CollissionDetector>();
        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LogicUpdate()
    {
        if (Health <= 0)
        {
            return;
        }

        MoveCamera();
        MovePlayer();

        if (healthLastFrame != Health)
        {
            Debug.Log("Playerhealth is now" + Health);
        }
        healthLastFrame = Health;

        if (Input.GetMouseButtonDown(1))
        {
            PickUpObject();
        }

        if (Input.GetMouseButtonDown(0))
        {
            ThrowObject();
        }

        if (collissionDetector.collisionBool && collissionDetector.collisionSpeed > 5)
        {
            Health--;
        }
    }

    public void PhysicsUpdate()
    {

    }

    void MovePlayer()
    {
        Vector3 oldPos = body.transform.position;

        Vector3 forwardMoveDirection = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
        Vector3 sideMoveDirection = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z).normalized;

        Vector3 newPlayerPosition = body.transform.position + ((forwardMoveDirection * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed + sideMoveDirection * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed));
        newPlayerPosition = new Vector3(newPlayerPosition.x, body.transform.position.y, newPlayerPosition.z);

        body.transform.position = newPlayerPosition;

        playerVelocity = oldPos - body.transform.position;
    }

    void MoveCamera()
    {
        xRotation = 0;
        yRotation = 0;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        playerCamera.transform.Rotate(Vector3.up, yRotation, Space.World);
        playerCamera.transform.Rotate(playerCamera.transform.right, xRotation, Space.World);

        if(playerCamera.transform.rotation.eulerAngles.x > 60 && playerCamera.transform.rotation.eulerAngles.x < 180)
        {
            playerCamera.transform.rotation = Quaternion.Euler(60, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z);
        }

        if (playerCamera.transform.rotation.eulerAngles.x < 300 && playerCamera.transform.rotation.eulerAngles.x > 180)
        {
            playerCamera.transform.rotation = Quaternion.Euler(-60, playerCamera.transform.rotation.eulerAngles.y, playerCamera.transform.rotation.eulerAngles.z);
        }

        playerCamera.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.eulerAngles.x, playerCamera.transform.rotation.eulerAngles.y, 0);
        // playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void PickUpObject()
    {
        Debug.Log("grabbing");
        if(currentlyHeldThrowable != null)
        {
            return;
        }

        float closestRange = grabRange;
        IThrowable toPickUp = null;

        for (int i = 0; i < BlackBoard.throwables.Count; i++)
        {
            float dotProduct = Vector3.Dot(body.transform.forward, (BlackBoard.throwables[i].body.transform.position).normalized);
            float distanceFromObject = Vector3.Distance(body.transform.position, BlackBoard.throwables[i].body.transform.position);

            if (dotProduct < FOV && distanceFromObject < closestRange && !BlackBoard.throwables[i].isBeingHeld)
            {
                closestRange = distanceFromObject;
                toPickUp = BlackBoard.throwables[i];
            }
        }

        nearestThrowable = toPickUp;

        if(nearestThrowable == null)
        {
            return;
        }

        IThrowable pickedUp;

        pickedUp = nearestThrowable;

        pickedUp.body.transform.SetParent(playerCamera.transform);

        pickedUp.body.transform.localPosition = hand;
        pickedUp.body.transform.localRotation = objectRotation;
        pickedUp.rb.isKinematic = true;

        pickedUp.isBeingHeld = true;
        currentlyHeldThrowable = pickedUp;

        nearestThrowable = toPickUp;

    }

    void ThrowObject()
    {
        if(currentlyHeldThrowable == null)
        {
            return;
        }

        IThrowable toThrow;

        toThrow = currentlyHeldThrowable;
        toThrow.body.transform.SetParent(null);
        toThrow.rb.isKinematic = false;

        Vector3 direction = playerCamera.transform.forward;
        toThrow.rb.AddForce(direction * throwStrength, ForceMode.Impulse);
        currentlyHeldThrowable = null;
        toThrow.isBeingHeld = false;
        isThrowing = false;
    }

    //void GameOver()
    //{
    //    SceneManager.LoadScene(0);
    //}
}
