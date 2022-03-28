using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    
    //default values
    [SerializeField] private int defaultPlayerHealth = 100;
    [SerializeField] private int defaultRifleAmmo = 60; //Weapons start at half amount
    [SerializeField] private int defaultShotgunAmmo = 20;

    //Max values 
    public int maxPlayerHealth = 100;
    public int maxRifleAmmo = 90;
    public int maxShotgunAmmo = 32;

    //Both current and max values ignoring ammo in mag

    //current values
    public int PlayerHealth;
    public int RifleAmmo;
    public int ShotgunAmmo;

    [SerializeField] private GameObject[] items;


    void Awake()
    {
        //Check if instance already exists
        if (_instance == null)
            //if not, set instance to this
            _instance = this;
        //If instance already exists and it's not this:
        else if (_instance != this)
            //Then destroy this. This enforces our singleton pattern,
            //meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        SetToDefault();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetToDefault() //Set or Reset health and ammo to default
    {
        PlayerHealth = defaultPlayerHealth;
        RifleAmmo = defaultRifleAmmo;
        ShotgunAmmo = defaultShotgunAmmo;
    }

    //Ammo updating to be accessed from weapon or item drops - Could be cleaned up
    public void UpdateAmmo(string weapon, int amount)
    {
        if (weapon.Equals("Rifle"))
        {
            RifleAmmo += amount;

            if (RifleAmmo > maxRifleAmmo)
                RifleAmmo = maxRifleAmmo;
            else if (RifleAmmo < 0)
                RifleAmmo = 0;
        }
        else if (weapon.Equals("Shotgun"))
        {
            ShotgunAmmo += amount;

            if (ShotgunAmmo > maxShotgunAmmo)
                ShotgunAmmo = maxShotgunAmmo;
            else if (ShotgunAmmo < 0)
                ShotgunAmmo = 0;
        }
        Debug.Log("Rifle ammo: " + RifleAmmo + "\t Shotgun ammo: " + ShotgunAmmo);
    }

    public void UpdateHealth(int amount)
    {
        PlayerHealth += amount;
        if (PlayerHealth > maxPlayerHealth)
        {
            PlayerHealth = maxPlayerHealth; //cap health at max
        }

        else if (PlayerHealth <= 0)
        {
            SceneManager.LoadScene("GameOver");
            SetToDefault();
        }


        Debug.Log("Player health now: " + PlayerHealth);
    }

    //Used by Pickup.cs to check if value already at Max before attempting to increase and destroying pickup
    public bool NotAtMax(string item)
    {
        int valueToCheck = 0;
        int maxValue = 0;

        switch (item)
        {
            case "Health":
                valueToCheck = PlayerHealth;
                maxValue = maxPlayerHealth;
                break;
            case "Rifle":
                valueToCheck = RifleAmmo;
                maxValue = maxRifleAmmo;
                break;
            case "Shotgun":
                valueToCheck = ShotgunAmmo;
                maxValue = maxShotgunAmmo;
                break;
        }

        if (valueToCheck < maxValue)
            return true;
        else return false;
    }


    #region Item Drop Selection

    /// <summary>
    /// 
    /// Custom developed algorithm for smart item drops. 
    /// By comparing current variables to the max variables, a percentage is calculated, then inverted.
    /// This percentage is applied to a base number (e.g 100) for every player variable, then they are all added together in a totalChance variable.
    /// The algorithm picks a number from 0 - totalChance, then the drop is generated depending on what item range the generated number falls in.
    /// 
    /// </summary>

    //Chance counters for items
    int healthChance = 1;
    int rifleAmmoChance = 1;
    int shotgunAmmoChance = 1;

    //Base chance for all items, to balance accordingly
    [SerializeField] int baseHealthChance = 200;
    [SerializeField] int baseRifleAmmoChance = 100;
    [SerializeField] int baseShotgunAmmoChance = 100;

    //total Chance for items
    int totalChance;

    //Calculates chance as int from 1-baseChance by comparing current variable to the max amount
    private int CalculateChance(int playerVariable, int variableMax, int baseChance)
    {
        //int baseChance = 100;

        float chanceRatio = 1 - ((float)playerVariable / variableMax); //Subtract from 1 for inverse chance

        int calculatedChance = (int)(baseChance * chanceRatio);

        if (calculatedChance == 0)
            calculatedChance = 1; //Negates chance of all values being 0, potentially causing errors

        //Console.WriteLine("Chance after int cast and 0 check: "+calculatedChance);
        return calculatedChance;
    }

    //Set item chances
    public void SetChances()
    {
        healthChance = CalculateChance(PlayerHealth, maxPlayerHealth, baseHealthChance);
        rifleAmmoChance = CalculateChance(RifleAmmo, maxRifleAmmo, baseRifleAmmoChance);
        shotgunAmmoChance = CalculateChance(ShotgunAmmo, maxShotgunAmmo, baseShotgunAmmoChance);

        //Subject to cleanup
        totalChance = healthChance + rifleAmmoChance + shotgunAmmoChance;
    }

    //Single drop method
    public GameObject DropItem() //Change later to GameObject return to drop item from items[]
    {
        SetChances();

        int ranNum = Random.Range(0, totalChance); //rnd.Next to be replaced with random.range


        //Needs efficiency cleanup
        int checkRange = healthChance;
        int selectedItem;
        if (ranNum < checkRange)
        {
            selectedItem = 0;
            Debug.Log("Health Kit dropped");
        }
        else
        {
            checkRange += rifleAmmoChance;
            if (ranNum < checkRange)
            {
                selectedItem = 1;
                Debug.Log("Ammo 1 dropped");
            }
            else
            {
                selectedItem = 2;
                Debug.Log("Ammo 2 dropped");
            }
        }
        return items[selectedItem];
    }

    #endregion
}   
