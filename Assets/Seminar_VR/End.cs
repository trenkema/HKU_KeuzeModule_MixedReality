using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField] Animator animator;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.tag == "Player")
        {
            triggered = true;

            StartCoroutine(waiter());
        }
    }

    IEnumerator waiter()
    {
        animator.Play("FadeOut");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(0);
    }
}
