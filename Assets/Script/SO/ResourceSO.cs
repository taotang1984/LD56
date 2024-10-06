using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/ResourceSO")]
public class ResourceSO : ScriptableObject
{
    public string resourceName;
    public int resourcePoints;
    public Sprite resourceSprite;
}
