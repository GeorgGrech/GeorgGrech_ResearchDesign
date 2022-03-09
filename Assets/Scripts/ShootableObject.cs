using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableObject : MonoBehaviour
{
    private GameManager gameManager;

    protected int health;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Killed();
        }
    }

    protected virtual void Killed()
    {
        gameManager.DropItem();
        Destroy(gameObject);
    }
}
