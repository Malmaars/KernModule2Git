using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SkullPrefab;
    public GameObject[] UINumbers, numberPrefabs;
    Node tree;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowHighScore();


        tree = new Sequence(
                    new BasicSpawnPrefab(SkullPrefab, new Vector3(-63.8f, 12.71f, 48), new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) ),
                    new WaitNode(2f)
                    );
    }

    void ShowHighScore()
    {
        int digit1 = BlackBoard.highScore / 10;
        int digit2 = BlackBoard.highScore % 10;


        GameObject temp = Instantiate(numberPrefabs[digit1], UINumbers[0].transform.position, UINumbers[0].transform.rotation, UINumbers[0].transform.parent);
        Destroy(UINumbers[0]);
        UINumbers[0] = temp;
        MeshRenderer[] meshrenderersTemp = temp.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in meshrenderersTemp)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        temp = Instantiate(numberPrefabs[digit2], UINumbers[1].transform.position, UINumbers[1].transform.rotation, UINumbers[1].transform.parent);
        Destroy(UINumbers[1]);
        UINumbers[1] = temp;
        meshrenderersTemp = temp.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in meshrenderersTemp)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
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
