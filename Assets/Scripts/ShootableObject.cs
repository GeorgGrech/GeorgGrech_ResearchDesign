using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableObject : MonoBehaviour
{
    public GameManager gameManager;

    protected int health;
    private bool isDying = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    protected virtual void ChangeHealth(int amount)
    {
        // Change the health by the amount specified in the amount variable
        health += amount;

        // If the health runs out, then Die.
        if (health <= 0 && !isDying)
            Killed();

        /*
        // Make sure that the health never exceeds the maximum health
        else if (currentHealth > maxHealth)
            currentHealth = maxHealth;*/
    }

    /*
    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Killed();
        }
    }*/

    protected virtual void UpdatePlayerAccuracy()
    {
        //gameManager.successfulShots++;
        gameManager.UpdateAccuracy(true); //Update accuracy with successful shot
    }

    protected virtual void Killed()
    {

        isDying = true;

        if (this.transform.CompareTag("Enemy"))
        {
            gameManager.CheckInCombat();
        }

        GameObject itemDrop = gameManager.DropItem();
        Instantiate(itemDrop,transform.position,transform.rotation);

        Destroy(gameObject);
    }

    protected virtual void SetInCombat()
    {
        gameManager.inCombat = true;
    }
}
