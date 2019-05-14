using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRedSlime : Enemy
{

    public GameObject littleSlime;

    public override void Kill()
    {
        Destroy(gameObject);
        Instantiate(littleSlime, transform.position + new Vector3(-0.5f, 0), Quaternion.identity);
        Instantiate(littleSlime, transform.position + new Vector3(0.5f, 0), Quaternion.identity);
        Instantiate(particles, transform.position + new Vector3(0.25f, -0.5f), Quaternion.identity);
        Instantiate(particles, transform.position + new Vector3(-0.25f, -0.5f), Quaternion.identity);

    }

    public override void Damage(float damage)
    {
        audioManager.Play("BigSquelch");
        health -= damage;
        if (health < 0.05 && health < maxHealth)
        {
            Kill();
        }
        clampHealth();
    }
}


