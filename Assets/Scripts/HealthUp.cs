using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D trigger)
    {

        if (trigger.gameObject.name == "Player")
        {
            trigger.gameObject.GetComponent<Player>().increaseHealth(1.0f);
            Destroy(gameObject);
        }
    }
}
