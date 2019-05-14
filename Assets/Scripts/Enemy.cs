using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public GameObject consumableHeart;
    public GameObject particles;

    public CharacterController2D controller;
    protected GameObject player;
    protected float horizontalMove = 0f;
    protected float thinktime = 0;
    public float runspeed;
    public Animator animator;
    public float attackRange = 3;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.gameObject.transform.position, gameObject.transform.position) < attackRange)
        {
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
        } else
        {
            // move in a random direction
            if (thinktime < 0)
            {
                thinktime = 1;
                float direction = Random.Range(-1, 2);
                horizontalMove =  runspeed * direction;

            }
            thinktime -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.GetComponent<Player>().Damage(0.5f);
            Vector3 difference = Vector3.Normalize(collision.gameObject.transform.position - gameObject.transform.position);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(difference*500);
        }
    }

    public override void Kill()
    {
        audioManager.Play("Crunch");
        Destroy(gameObject);
        if (Random.Range(0, 2) < 0.5f)
        {
            Instantiate(consumableHeart, transform.position, Quaternion.identity);
        }
        
        Instantiate(particles, transform.position, Quaternion.identity);
    }

    public override void Damage(float damage)
    {
        audioManager.Play("Squelch");
        health -= damage;
        if (health < 0.05 && health < maxHealth)
        {
            Kill();
        }
        clampHealth();
    }
}
