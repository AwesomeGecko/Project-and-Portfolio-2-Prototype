using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class EnemyLOS : MonoBehaviour
{
 
    public SphereCollider Collider;
    public float FieldOfView;
    public LayerMask LOSLayers;

    public delegate void GainSightEvent(Transform Target);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(Transform Target);
    public LoseSightEvent OnLoseSight;

    private Coroutine CheckForLOSCoroutine;

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
       if (!CheckLOS(other.transform))
        {
            CheckForLOSCoroutine = StartCoroutine(CheckForLOS(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnLoseSight?.Invoke(other.transform);

        if(CheckForLOSCoroutine != null)
        {
            StopCoroutine(CheckForLOSCoroutine);
        }
    }

    private bool CheckLOS(Transform Target)
    {
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, direction);

        if (direction == null)
        {
            Debug.Log("Transform no longer exists");
        }

        if(dotProduct >= Mathf.Cos(FieldOfView))
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Collider.radius, LOSLayers))
            {
                OnGainSight?.Invoke(Target);
                return true;
            }
        }

        return false;
    }

    private IEnumerator CheckForLOS(Transform Target)
    {
        WaitForSeconds Wait = new WaitForSeconds(.5f);

        while(!CheckLOS(Target))
        {
            yield return Wait;
        }
    }


}
