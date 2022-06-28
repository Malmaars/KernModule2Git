using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCounterAdder : MonoBehaviour
{
    int kills;
    GameObject[] numberPrefabs;
    GameObject[] UINumbers;

    public KillCounterAdder(GameObject[] _numberPrefabs, GameObject[] _UINumbers)
    {
        numberPrefabs = _numberPrefabs;
        UINumbers = _UINumbers;
    }

    public void CounterUpdate()
    {
        if(kills != BlackBoard.killCount)
        {
            kills = BlackBoard.killCount;
            UpdateUINumbers();
        }
    }

    void UpdateUINumbers()
    {
        int digit1 = kills / 10;
        int digit2 = kills % 10;

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


}
