using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : ShootableObject
{
    public Transform player;

    [SerializeField] private int objectHealth = 100;

    //A* variables
    private Seeker seeker;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;

    //Weapon
    [SerializeField] private GameObject weaponObject;
    private Weapon weapon;

    //Player detection variables
    [SerializeField] private float detectDistance = 10;
    [SerializeField] private float shootAngle = 30;
    public LayerMask obstacleMask; //Obstacles that block enemy vision


    #region Inherited methods
    // Start is called before the first frame update
    protected override void Start()
    {
        base.health = objectHealth;
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").transform; //Find player
        weapon = weaponObject.GetComponent<Weapon>();
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        destinationSetter.target = player;
    }

    protected override void ChangeHealth(int amount)
    {
        base.ChangeHealth(amount);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (DetectPlayer()/*&& !stunActive && !isDying*/)
        {
            //StopAllCoroutines(); //Stop everything to follow player
            AStarEnable(true);
            //animator.SetBool("isFollowing", true);


            if (PlayerInSight(shootAngle))
            {
                weapon.RemoteFire();
            }
            //else weapon.isFiring = false;
        }
    }

    bool DetectPlayer() //Spot player in field of view
    {
        if (player) //if player hasn't been destroyed
        {
            if (Vector3.Distance(transform.position, player.position) < detectDistance) //if within view distance
            {

                return true;
            }
            return false;
        }
        return false;
    }

    private bool PlayerInSight(float angle)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleBetweenPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleBetweenPlayer < angle / 2f) //Within the viewing angle
        {
            if (!Physics.Linecast(transform.position, player.position, obstacleMask))//if view to player is not being obstructed by obstacle
            {
                Debug.Log("Player in sight");
                return true;
            }
        }
        return false;
    }

    public void AStarEnable(bool enable) //Enables tracking
    {
        //isFollowingPlayer = enable;
        seeker.enabled = enable;
        aiPath.enabled = enable;
        destinationSetter.enabled = enable;
    }
}
