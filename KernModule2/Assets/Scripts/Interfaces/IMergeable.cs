using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface for things that can merge together
public interface IMergeable : ISpawnable
{
    IMergeable[] Requirements { get; set; }
    ISplittable MergeResult { get; set; }
    GameObject resultPrefab { get; set; }
}
