using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransformData", menuName = "Guns/TransformData", order = 2)]
public class TransformData : ScriptableObject
{
    public Vector3 localPosition;
    public Vector3 localRotation;
}
