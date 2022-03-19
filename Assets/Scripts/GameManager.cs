using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    //current values
    [SerializeField] private int playerHealth = 100;
    [SerializeField] private int rifleAmmo = 60; //Weapons start at half amount
    [SerializeField] private int shotgunAmmo = 20;

    //Max values 
    public int maxPlayerHealth = 100;
    public int maxRifleAmmo = 90;
    public int maxShotgunAmmo = 32;

    //Both current and max values ignoring ammo in mag

    public int PlayerHealth { get => playerHealth; set => playerHealth = value; }
    public int RifleAmmo { get => rifleAmmo; set => rifleAmmo = value; }
    public int ShotgunAmmo { get => shotgunAmmo; set => shotgunAmmo = value; }

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

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Ammo updating to be accessed from weapon or item drops - Could be cleaned up
    public void UpdateAmmo(string weapon, int amount)
    {
        if (weapon.Equals("Rifle"))
        {
            rifleAmmo += amount;

            if (rifleAmmo > maxRifleAmmo)
                rifleAmmo = maxRifleAmmo;
            else if (rifleAmmo < 0)
                rifleAmmo = 0;
        }
        else if (weapon.Equals("Shotgun"))
        {
            shotgunAmmo += amount;

            if (shotgunAmmo > maxShotgunAmmo)
                ShotgunAmmo = maxShotgunAmmo;
            else if (shotgunAmmo < 0)
                shotgunAmmo = 0;
        }
        Debug.Log("Rifle ammo: " + rifleAmmo + "\t Shotgun ammo: " + shotgunAmmo);
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

    //total Chance for items
    int totalChance;

    //Calculates chance as int from 1-baseChance by comparing current variable to the max amount
    private int CalculateChance(int playerVariable, int variableMax)
    {

        int baseChance = 100;
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
        healthChance = CalculateChance(playerHealth, maxPlayerHealth);
        rifleAmmoChance = CalculateChance(rifleAmmo, maxRifleAmmo);
        shotgunAmmoChance = CalculateChance(shotgunAmmo, maxShotgunAmmo);

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
