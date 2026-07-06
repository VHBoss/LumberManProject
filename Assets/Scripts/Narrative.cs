using System.Collections;
using UnityEngine;

public class Narrative : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float timeToShow = 3;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        animator.SetBool("Show", true);
        yield return new WaitForSeconds(timeToShow);
        animator.SetBool("Show", false);
    }
}
