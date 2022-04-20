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
        if (gameManager.shotsFired > 0) //Prevents shotsFired=0 & succesfulShots=1 that causes divide by 0
        {
            gameManager.UpdateAccuracy(true); //Update accuracy with successful shot
        }
    }

    protected virtual void Killed()
    {

        isDying = true;

        if (this.transform.CompareTag("Enemy"))
        {
            gameManager.CheckInCombat(this.name);

            int ranNum = Random.Range(0, 3);
            if (ranNum == 0) //if Enemy, drop is 1/3 chance
            {
                DropItem();
            }
        }
        else //if Crate, drop awlays
        {
            DropItem();
        }

        

        Destroy(gameObject);
    }

    protected virtual void SetInCombat()
    {
        gameManager.inCombat = true;
    }

    private void DropItem()
    {
        GameObject itemDrop = gameManager.DropItem();
        Instantiate(itemDrop, transform.position, transform.rotation);
    }
}
