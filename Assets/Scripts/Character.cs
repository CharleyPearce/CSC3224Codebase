using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public AudioManager audioManager;
    public float health;
    public float maxHealth;

    public void SetMaxHealth(float newHealth)
    {
        if (maxHealth >= 0)
        {
            maxHealth = newHealth;
        } else
        {
            maxHealth = 0;
        }

        clampHealth();
    }

    public bool isFullHealth () {
        if (Mathf.Approximately(health, maxHealth))
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void setHealth(float newHealth)
    {
        health = newHealth;
        clampHealth();
    }

    public virtual void Damage(float damage)
    {
        health -= damage;
        if (health < 0.05 && health < maxHealth)
        {
            Kill();
        }
        clampHealth();
    }

    public void heal(float newHealth)
    {
        health += newHealth;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        clampHealth();
    }

    public void increaseHealth(float newHealth)
    {
        maxHealth += newHealth;
        health += newHealth;
        clampHealth();
    }

    public void decreaseHealth(float newHealth)
    {
        maxHealth -= newHealth;
        // player can still survive on 0 health
        if (maxHealth < 0)
        {
            maxHealth = 0;
        }
        // decreasing health doesn't harm the player
        clampHealth();
    }

    public virtual void clampHealth()
    {
        if (health < 0)
        {
            health = 0;
        } else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public virtual void Kill()
    {
        // set health to 0 incase kill is called from somewhere else

        health = 0;
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
