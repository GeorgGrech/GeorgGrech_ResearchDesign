using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : ShootableObject
{
    [SerializeField] private int objectHealth = 15;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.health = objectHealth;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
