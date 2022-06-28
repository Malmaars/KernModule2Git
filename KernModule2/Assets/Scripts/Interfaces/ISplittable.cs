using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface for things that can split apart
public interface ISplittable : ISpawnable
{
    bool ShouldSplit { get; set; }
    GameObject[] resultPrefabs { get; set; }


}
