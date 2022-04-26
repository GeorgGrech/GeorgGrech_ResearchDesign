using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : ShootableObject
{
    public Transform player;
    public Transform playerCamera;

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
    [SerializeField] private float viewDistance = 20;
    [SerializeField] private float shootAngle = 30;
    [SerializeField] private float viewAngle = 100;
    public LayerMask obstacleMask; //Obstacles that block enemy vision

    [SerializeField] private Transform viewSpot;

    [SerializeField] private float baseRotationSpeed = 200; //player targeting rotation speed when static
    [SerializeField] private float baseMovementSpeed = 7; //movement speed when moving to player
    [SerializeField] private float modifiedMovementSpeed; //seperate modified variable to compare with aiPath.maxSpeed


    public bool isFollowing = false; //enabled when player detected 
    public bool isTracking = false; //enabled only when enemy pauses for stationary aim

    public bool canFire = false; //dictates when enemy is firing or cooling down if player in sight
    //public Coroutine fireCoroutine;

    #region Inherited methods
    // Start is called before the first frame update
    protected override void Start()
    {
        base.health = objectHealth;
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").transform; //Find player
        playerCamera = player.transform.GetChild(0).Find("PlayerCamera"); //Find player camera for use in hiding
        weapon = weaponObject.GetComponent<Weapon>();
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        destinationSetter.target = player;

        gameManager.enemies.Add(gameObject);
    }

    protected override void ChangeHealth(int amount)
    {
        /*if (!isFollowing)
        {
            StartCoroutine(MovePause());//Follow the player when damaged even if not within regular distance
        }*/

        base.ChangeHealth(amount);
    }

    protected override void UpdatePlayerAccuracy()
    {
        base.UpdatePlayerAccuracy();
    }

    protected override void SetInCombat()
    {
        base.SetInCombat();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (DetectPlayer() /*&& !stunActive && !isDying*/) //If player detected and not already followoing 
        {
            //StopAllCoroutines(); //Stop everything to follow player
            if (!isFollowing)
            {
                StartCoroutine(FollowPlayer());
            }
            //animator.SetBool("isFollowing", true);


            if (PlayerInSight(shootAngle) && canFire)
            {
                weapon.RemoteFire();
            }
            /*else if(fireCoroutine!=null)
            {
                StopCoroutine(fireCoroutine);
            }*/

            if (aiPath.reachedDestination || isTracking)
            {
                Debug.Log("Target reached"); //To be replaced with code that updates to look at player
                RotateToPlayer();
            }
            else if(aiPath.maxSpeed!=modifiedMovementSpeed)
            {
                modifiedMovementSpeed = baseMovementSpeed * gameManager.difficultyModifier;
                aiPath.maxSpeed = modifiedMovementSpeed;
            }
            //else weapon.isFiring = false;
        }
    }

    bool DetectPlayer() //Spot player in field of view
    {
        if (player) //if player hasn't been destroyed
        {
            if (Vector3.Distance(transform.position, player.position) < viewDistance) //if within view distance
            {

                if (PlayerInSight(viewAngle) //if within viewing angle
                    || (Vector3.Distance(transform.position, player.position) < detectDistance) //if within detection distance
                    || (health < objectHealth)) //if damaged
                {
                    //Debug.Log("Enemy Visual Detection");
                    return true;
                }

                /*
                if (Vector3.Distance(transform.position, player.position) < detectDistance) //if within detection distance
                {
                    Debug.Log("Enemy Proximity Detection");
                    return true;
                }

                if (health < objectHealth) //if damaged
                {
                    return true;
                }*/

            return false;
            }
        }
        return false;
    }

    private bool PlayerInSight(float angle)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleBetweenPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleBetweenPlayer < angle / 2f) //Within the viewing angle
        {
            if (!Physics.Linecast(viewSpot.position, playerCamera.position, obstacleMask))//if view to player is not being obstructed by obstacle (Camera used to dictate by player POV)
            {
                Debug.Log("Player in sight");
                return true;
            }
        }
        return false;
    }

    public void AStarEnable(bool enable) //Enables tracking
    {
        //if (!isFollowing)
        //{
            SetInCombat(); //Set GameManager in combat mode

            //isFollowingPlayer = enable;
            seeker.enabled = enable;
            aiPath.enabled = enable;
            destinationSetter.enabled = enable;
            //StartCoroutine(MovePause());
        //}
    }

    void RotateToPlayer()
    {
        float rotationSpeed = baseRotationSpeed * gameManager.difficultyModifier; //multiply base rotation speed by the difficulty ratio

        Vector3 TargetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        Quaternion.LookRotation(TargetPosition - transform.position),
        Time.deltaTime * rotationSpeed);
         
    }

    private IEnumerator FollowPlayer()
    {
        isFollowing = true;
        StartCoroutine(BurstFiring());

        /*while (true)
        {
            //yield return null;
            aiPath.maxSpeed = 0;
            isTracking = true;
            Debug.Log("Enemy state: Tracking");
            yield return new WaitForSeconds(Random.Range(2, 5));
            aiPath.maxSpeed = modifiedMovementSpeed;
            isTracking = false;
            Debug.Log("Enemy state: Following");
            yield return new WaitForSeconds(Random.Range(2, 5));
        }*/

        while (true)
        {
            //yield return null;
            AStarEnable(false);
            isTracking = true;
            Debug.Log("Enemy state: Tracking");
            yield return new WaitForSeconds(Random.Range(1, 3));
            AStarEnable(true);
            isTracking = false;
            Debug.Log("Enemy state: Following");
            yield return new WaitForSeconds(Random.Range(3, 5)); //Longer follow time
        }
    }

    private IEnumerator BurstFiring()
    {
        while (true)
        {
            canFire = true;
            Debug.Log("Enemy firing state: Firing");
            yield return new WaitForSeconds(1);
            canFire = false;
            Debug.Log("Enemy firing state: Cooling down");
            yield return new WaitForSeconds(1);
        }
    }
}
