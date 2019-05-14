using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Enemy
{
    public string type = "child";
    public bool spawns;
    public static bool fightStarted = false;
    public static int count = 0;
    public GameObject littleSlime;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        player = GameObject.Find("Player");
        count++;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.gameObject.transform.position, gameObject.transform.position) < attackRange)
        {
            if (!fightStarted)
            {
                fightStarted = true;
                audioManager.Stop("Music");
                audioManager.Play("BossMusic");
            }

            if (player.transform.position.x < gameObject.transform.position.x)
            {
                while (horizontalMove > -runspeed)
                {
                    horizontalMove -= 0.02f * runspeed;
                }
            }
            if (player.transform.position.x > gameObject.transform.position.x)
            {
                while (horizontalMove < runspeed)
                {
                    horizontalMove += 0.02f * runspeed;
                }
            }
        }
        else
        {
            // move in a random direction
            if (thinktime < 0)
            {
                thinktime = 1;
                float direction = Random.Range(-1, 2);
                horizontalMove = runspeed * direction;

            }
            thinktime -= Time.deltaTime * 0.1f;
        }
    }

    public override void Kill()
    {
        count--;
        Destroy(gameObject);

        Instantiate(particles, transform.position + new Vector3(0.25f, 0.5f), Quaternion.identity);
        Instantiate(particles, transform.position + new Vector3(-0.25f, 0.5f), Quaternion.identity);
        if (spawns) {
            
            Instantiate(littleSlime, transform.position + new Vector3(-0.5f, 0), Quaternion.identity);
            Instantiate(littleSlime, transform.position + new Vector3(0.5f, 0), Quaternion.identity);
            
        }
        Debug.Log(count);
    }

    public override void Damage(float damage)
    {
        
        health -= damage;

        if (health < 0.05 && health < maxHealth)
        {
            Kill();
        }
        clampHealth();
        audioManager.Play("BigSquelch");
    }
}


