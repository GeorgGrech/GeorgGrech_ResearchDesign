using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void ChangeHealth(int amount)
    {
        gameManager.PlayerHealth += amount;
        Debug.Log("Player health now: "+gameManager.PlayerHealth);
    }

    private void UpdateAmmo(string weapon, int amount)
    {
        gameManager.UpdateAmmo(weapon, amount);
    }
}
