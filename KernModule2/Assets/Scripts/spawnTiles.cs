using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTiles : MonoBehaviour
{

    //maybe generate levels if I want to
    public GameObject tile;
    public GameObject[] walls;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i <= 20; i++)
        {
            for(int j = 1; j <= 20; j++)
            {
                Instantiate(tile, new Vector3(i * 6 - 60, -1, j * 6 - 60), new Quaternion(0, 0, 0, 0), this.gameObject.transform);
            }
        }

        for (int i = 1; i <= 20; i++)
        {
            for (int j = 1; j <= 20; j++)
            {
                Instantiate(tile, new Vector3(i * 6 - 60, -1, j * 6 - 60), new Quaternion(0, 0, 0, 0), this.gameObject.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
