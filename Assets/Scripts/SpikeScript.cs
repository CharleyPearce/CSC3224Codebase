using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Character>().Damage(0.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Spike Trigger: " + other.name);
    }
}
