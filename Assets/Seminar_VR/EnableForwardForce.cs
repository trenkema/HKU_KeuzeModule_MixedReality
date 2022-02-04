using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableForwardForce : MonoBehaviour
{
    [SerializeField] BirdMovement birdMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            birdMovement.canFlyForwards = true;

            birdMovement.hasStarted = true;

            Destroy(gameObject);
        }
    }
}
