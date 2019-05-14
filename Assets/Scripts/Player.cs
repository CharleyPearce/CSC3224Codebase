using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    enum Direction
    {
        left, right
    }

    float horizontalMove = 0f;
    float verticalMove = 0f;
    bool jump = false;
    bool dead = false;
    bool won = false;
    bool cheatMode;
    bool invulnrable;
    float invulnrableTimer = 0;
    float forceTimer = 0;
    bool downAtt = false;
    bool upAtt = false;
    Direction facing = Direction.right;

    float flightSpeed = 20f;
    bool grounded = true;

    public float runspeed;
    public HealthBarContoller hp;
    public Animator animator;
    public CharacterController2D controller;
    public TileGenerator tileGenerator;

    public void applyForce(Vector3 force)
    {
        if (forceTimer <= 0)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(force);
            forceTimer = 0.5f;
        } else
        {
            forceTimer -= Time.deltaTime;
        }
        
    }

    public void makeInvulrable(float time)
    {
        invulnrableTimer = time;
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        // find the health bar controller
        hp = GameObject.Find("PlayerHealthBar").GetComponent<HealthBarContoller>();
        hp.UpdateHealth();
    }

    // override the clamp method to update the healthbar whenever called
    public override void clampHealth()
    {
        if (health < 0)
        {
            health = 0;
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
        hp.UpdateHealth();
    }

    public bool Alive()
    {
        return !dead;
    }

    public bool Won()
    {
        return won;
    }

    public override void Kill()
    {
        if (!dead)
        {
            dead = true;
            animator.SetBool("Death", true);
            makeInvulrable(10000);
        } 
        
        
        // reset to a smaller stage
        PlayerPrefs.SetInt("noRooms", 5);
        PlayerPrefs.SetInt("floorNo", 0);

        /*

        // reset the scene
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        */
    }

    public bool GetUpAttack()
    {
        return upAtt;
    }

    public bool GetDownAttack()
    {
        return downAtt;
    }

    // Update is called once per frame
    void Update()
    {
        if (KingSlime.fightStarted && KingSlime.count <= 0)
        {
            won = true;
            Debug.Log("You win!");
        }

        if (invulnrableTimer > 0)
        {
            invulnrable = true;
            invulnrableTimer -= Time.deltaTime;
        } else
        {
            invulnrable = false;
        }

        grounded = controller.isGrounded();
        if (OptionsScript.debugMode)
        {
            cheatMode = Input.GetButton("Cheat");
        }
        else
        {
            cheatMode = false;
        }

        if (Input.GetButtonDown("Reset"))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        // disable the rigidbody so the player can pass through walls when chatmode is on
        if (cheatMode == true)
        {
            makeInvulrable(0.5f);
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        }

        verticalMove = Input.GetAxisRaw("Vertical") * runspeed;
        horizontalMove = Input.GetAxisRaw("Horizontal") * runspeed;

        if (!grounded && verticalMove < 0)
        {
            makeInvulrable(0.1f);
            downAtt = true;
        }
        else
        {
            downAtt = false;
        }

        if (verticalMove > 0)
        {
            upAtt = true;
        }
        else
        {
            upAtt = false;
        }

        if (Input.GetAxisRaw("Horizontal") < 0 )
        {
            facing = Direction.left;
            animator.SetBool("Moving", true);
        } else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            facing = Direction.right;
            animator.SetBool("Moving", true);
        } else
        {
            animator.SetBool("Moving", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Attack"))
        {

            // attack(facing == Direction.left ? new Vector2(-1, 0) : new Vector2(1, 0));

            animator.SetBool("Attack", true);
        }

        animator.SetBool("Grounded", grounded);
        animator.SetBool("DownAttack", downAtt);
        animator.SetBool("UpAttack", upAtt);
    }

    // override the damage method to make the player immune when in cheatmode
    public override void Damage(float damage)
    {
        if (!invulnrable) {
            audioManager.Play("Hurt");

            animator.SetBool("Hurt", true);
            health -= damage;
            makeInvulrable(0.5f);
            if (health < 0.05 && health < maxHealth)
            {
                Kill();
            }
            clampHealth();
        }
    }

    public void attack(Vector2 direction)
    {
        // send three rays to hit the enemy
        direction = facing == Direction.left ? new Vector2(-1, 0) : new Vector2(1, 0);
        direction = direction * 2;
        for (float i = -1.0f; i < 2f; i += 1.0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, (i / 3.0f) - 0.4f, 0), direction, 1.0f, LayerMask.GetMask("Enemies"));

            Debug.DrawRay(transform.position + new Vector3(0, (i / 3) - 0.4f, 0), direction, Color.red, 10);

            if (hit.collider != null)
            {

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemies"))
                {
                    hit.collider.gameObject.GetComponent<Enemy>().Damage(1);
                    // knockback
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce((direction + new Vector2(0, 0.25f)) * 1000);
                }
            }
        }
        // check if the player hit a wall and make a clink if so
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, 1.0f, LayerMask.GetMask("Walls"));
        if (wallHit.collider != null)
        {
            Debug.Log("Hit a wall ");
            audioManager.Play("Clink");
        }
    }

    void FixedUpdate()
    {
        if (!dead) {
            if (!cheatMode)
            {
                // move the player with the player controller
                controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
                if (jump == true)
                {
                    jump = false;
                }
            }
            else
            {
                // move the player in whichever direction
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(
                    horizontalMove * Time.fixedDeltaTime * flightSpeed,
                    verticalMove * Time.fixedDeltaTime * flightSpeed,
                    0
                );
            }
        }
    }
}
