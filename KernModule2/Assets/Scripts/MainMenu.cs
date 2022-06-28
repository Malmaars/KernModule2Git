using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SkullPrefab;
    Node tree;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        tree = new Sequence(
                    new BasicSpawnPrefab(SkullPrefab, new Vector3(-63.8f, 12.71f, 48), new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) ),
                    new WaitNode(2f)
                    );
    }

    // Update is called once per frame
    void Update()
    {
        tree.Run();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
