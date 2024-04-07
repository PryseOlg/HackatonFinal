using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PathData", menuName = "ScriptableObjects/PathData", order = 1)]
public class PathData : ScriptableObject
{
    public List<Vector2> points = new();
}