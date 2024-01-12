using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransformData", menuName = "Custom/TransformData")]
public class TransformData : ScriptableObject
{
    public Vector3 localPosition;
    public Vector3 localRotation;
}
