using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ShootableObject
{
    [SerializeField] private int objectHealth = 100;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.health = objectHealth;
        base.Start();
    }

    protected override void ChangeHealth(int amount)
    {
        base.ChangeHealth(amount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
