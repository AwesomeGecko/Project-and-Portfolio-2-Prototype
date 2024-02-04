using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LaserTrap : MonoBehaviour
{
    // Notes: Set the Start Point to the original position of the laser. Adjust the End Point
    // x or y values. The laser will now bounce back and forth between two points.
    // IMPORTANT! Make sure Point to Point flag is set to TRUE(checked).

    [Header("Damage")]
    [SerializeField] int dmgAmount;
    [SerializeField] int burnAmt;
    [SerializeField] int burnOverTime;

    [Header("Position")]
    [SerializeField] bool pointToPoint;
    [SerializeField] Vector3 startPoint;
    [SerializeField] Vector3 endPoint;
    [SerializeField] float movingSpeed;

    private bool playerInside = false;
    Vignette vig;

    // Start is called before the first frame update
    void Start()
    {
        if(pointToPoint)
        {
            StartCoroutine(Move());
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController HP = other.GetComponent<PlayerController>();
            if (HP != null)
            {
                HP.takeDamage(dmgAmount);
                StartCoroutine(BurnOverTime(HP));
                playerInside = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator BurnOverTime(PlayerController HP) // Burn Damage Method
    {
        float timer = 0f;
        HP.BurnStart();

        /*
        PostProcessVolume ppVol = HP.GetComponentInChildren<PostProcessVolume>();
        if(ppVol != null && ppVol.profile.TryGetSettings(out vig))
        {
            vig.intensity.value = 0.5f;
        }
        */

        while (timer < burnOverTime && HP.HP > 0)
        {
            HP.takeDamage(burnAmt);
            yield return new WaitForSeconds(1f);
            timer += 1f;
        }
        HP.BurnStop();
        /*
        if (ppVol != null && ppVol.profile.TryGetSettings(out vig))
        {
            vig.intensity.value = 0.5f;
        }
        */
        if (playerInside && HP.HP > 0)
        {
            StartCoroutine(BurnOverTime(HP));
        }
    }

    private IEnumerator Move()
    {
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint, movingSpeed * Time.deltaTime);
            if (transform.position == endPoint)
            {
                yield return new WaitForSeconds(1f);
                ReverseCourse();
            }
            yield return null;
        }while (true);
    }

    private void ReverseCourse() // Use a swap method
    {
        Vector3 temp = startPoint;
        startPoint = endPoint;
        endPoint = temp;
    }
}
