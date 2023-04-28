using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DestructibleSO")]
public class DestructibleSO : ScriptableObject
{
    public string Name;

    public int ID;

    public Mesh Mesh;

    public DestructibleObject Prefab;
}
