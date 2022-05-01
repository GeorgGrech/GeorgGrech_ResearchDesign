using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif


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

    private bool isDying = false;
    public int PlayerDeaths = 0;

    public bool curentlyPlaying; //To enable or disable CSV tracking

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
        //SetupLevel(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Current difficulty ratio: " + DifficultyCalculate());
    }

    public void SetupLevel(bool resetEntirely) //Setting appropriate variables when replaying level
    {
        curentlyPlaying = true;
        isDying = false;

        if(!isTrackingData) //If not already tracking data
        {
            StartCoroutine(AddKeyframes()); //Start tracking data for CSV
        }

        PlayerHealth = defaultPlayerHealth;
        RifleAmmo = defaultRifleAmmo;
        ShotgunAmmo = defaultShotgunAmmo;

        enemies.Clear(); //Clear list before use in level
        //enemies = GameObject.FindGameObjectsWithTag("Enemy");
        inCombat = false;

        if (resetEntirely)
        {
            accuracyRatio = 1; //Reset Accuracy variables
            shotsFired = 0;
            successfulShots = 0;

            PlayerDeaths = 0; //Reset deaths
        }


        difficultyUpateCoroutine = StartCoroutine(UpdateDifficulty());
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

        else if (PlayerHealth <= 0 && !isDying) //Check if functions at death already running
        {
            PlayerKill();
            /*
           
            //SetToDefault();*/
        }
        Debug.Log("Player health now: " + PlayerHealth);
    }

    void PlayerKill()
    {
        isDying = true;

        PlayerDeaths++;
        StopCoroutine(difficultyUpateCoroutine); //Disable difficulty corutine when not needed
        SceneManager.LoadScene("GameOver");
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

    [Tooltip("SMART ITEM DROP \nWhen disabled, all items will have equal chance of spawning from drops at all times. When enabled, chances for items to spawn is calculated whenever an item needs to drop from a crate or an enemy by comparing current player variables (Health and all ammo types) to the variable max.")]
    [SerializeField] private bool DropAlgorithmEnable = true;

    //Chance counters for items
    int healthChance;
    int rifleAmmoChance;
    int shotgunAmmoChance;

    //Base chance for all items, to balance accordingly
    [SerializeField] int baseHealthChance = 200;
    [SerializeField] int baseRifleAmmoChance = 100;
    [SerializeField] int baseShotgunAmmoChance = 100;

    //total Chance for items
    int totalChance;

    //Calculates chance as int from 1-baseChance by comparing current variable to the max amount
    private int CalculateChance(int playerVariable, int variableMax, int baseChance)
    {
        if (DropAlgorithmEnable)
        {
            //int baseChance = 100;

            float chanceRatio = 1 - ((float)playerVariable / variableMax); //Subtract from 1 for inverse chance

            int calculatedChance = (int)(baseChance * chanceRatio);

            if (calculatedChance == 0)
                calculatedChance = 1; //Negates chance of all values being 0, potentially causing errors

            //Console.WriteLine("Chance after int cast and 0 check: "+calculatedChance);
            return calculatedChance;
        }
        else
            return 1; //Return 1 for equal chance for all
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
    public GameObject DropItem()
    {
        SetChances();

        int ranNum = Random.Range(0, totalChance);


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
        return items[selectedItem]; //Drop from items list
    }

    #endregion

    #region Dynamic Difficulty Adjustement
    public bool inCombat = true; // True if in combat (followed by enemies)

    public int shotsFired = 0;
    public int successfulShots = 0;
    //private int failedShots;
    public float accuracyRatio = 1;

    public List<GameObject> enemies;

    public bool enableDDA = true;
    [SerializeField] private bool ddaUseHealth = true;
    [SerializeField] private bool ddaUseAmmo = true;
    [SerializeField] private bool ddaUseAccuracy = true;

    public float difficultyModifier; //Modifier to manipulate enemy values, changing game difficulty 
    [SerializeField] float constantDifficultyModifier = .7f; //Constant difficulty modifier to be used when DDA disabled
    [SerializeField] private float difficultyUpdateInterval = .5f;
    private Coroutine difficultyUpateCoroutine;

    public void UpdateAccuracy(bool shotSuccess)
    {
        if (inCombat) //Only take note of shots and accuracy if in combat 
        {
            if (!shotSuccess)
            {
                shotsFired++;
            }
            else
            {
                successfulShots++;
            }

            accuracyRatio = (float)successfulShots / shotsFired;
            Debug.Log("Succesful shots: " + successfulShots + " | Shots fired: " + shotsFired + " | Accuracy: " + accuracyRatio);
        }
    }

    public void CheckInCombat(string triggerEnemyName)
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                if(enemy.name == triggerEnemyName) //If enemy that triggered method 
                {
                    continue; //ignore isFollowingStatus
                }

                if (enemy.GetComponent<Enemy>().isFollowing == true)
                {
                    inCombat = true;
                    break;
                }
            }
        inCombat = false;
        }   
    }

    private IEnumerator UpdateDifficulty()
    {
        while (true)
        {
            if (enableDDA)
            {
                difficultyModifier = DifficultyCalculate();
            }
            else
            {
                difficultyModifier = constantDifficultyModifier;
            }
            Debug.Log("Difficulty Modifier: " + difficultyModifier);
            yield return new WaitForSeconds(difficultyUpdateInterval);
        }
    }

    public float DifficultyCalculate()
    {
        float healthRatio = (float)PlayerHealth / maxPlayerHealth;


        float rifleAmmoRatio = (float)RifleAmmo / maxRifleAmmo;
        float shotgunAmmoRatio = (float)ShotgunAmmo / maxShotgunAmmo;

        float ammoRatio = (rifleAmmoRatio + shotgunAmmoRatio) / 2; //Group ammo together to not majorly overshadow health

        List<float> ratioVariables = new List<float>();

        if(ddaUseHealth)
            ratioVariables.Add(healthRatio);

        if (ddaUseAmmo)
            ratioVariables.Add(ammoRatio);

        if (ddaUseAccuracy)
            ratioVariables.Add(accuracyRatio);

        float ratioAdd = 0; //To be later divided for average

        foreach(float variable in ratioVariables)
        {
            ratioAdd += variable;
        }

        float averageRatio = ratioAdd / ratioVariables.Count; //Average of all ratios, to be used as a difficulty ratio, 1 being the maximum difficulty

        return averageRatio;
    }
    #endregion

    #region CSV Data tracking
    public bool isTrackingData;

    [System.Serializable]
    public class KeyFrame
    {
        public float Time;
        public int PlayerHealth;
        public float AccuracyRatio;
        public float DifficultyModifier;
        public int PlayerDeaths;

        public KeyFrame() { }

        public KeyFrame(float time, int playerhealth, float accuracyRatio, float difficultyModifer,int playerDeaths)
        {
            PlayerHealth = playerhealth;
            Time = time;
            AccuracyRatio = accuracyRatio * 100; //Multiplied by 100 for percentage
            DifficultyModifier = difficultyModifer * 100;
            PlayerDeaths = playerDeaths;
        }
    }

    private List<KeyFrame> keyFrames = new List<KeyFrame>(10000);

    private IEnumerator AddKeyframes()
    {
        isTrackingData = true;
        Debug.Log("Data tracking: Started");
        while (curentlyPlaying)
        {
            Debug.Log("Data tracking: Keyframe added");
            keyFrames.Add(new KeyFrame(Time.time, PlayerHealth, accuracyRatio, difficultyModifier, PlayerDeaths));
            yield return new WaitForSeconds(1);
        }
        isTrackingData = false;

        SaveToFile();
        Debug.Log("Data tracking: Saving Data");
    }

    public string ToCSV()
    {
        var sb = new StringBuilder("Time,PlayerHealth,Accuracy,Difficulty,PlayerDeaths");
        foreach (var frame in keyFrames)
        {
            sb.Append('\n')
                .Append(frame.Time.ToString()).Append(',')
                .Append(frame.PlayerHealth.ToString()).Append(',')
                .Append(frame.AccuracyRatio.ToString()).Append(',')
                .Append(frame.DifficultyModifier.ToString()).Append(',')
                .Append(frame.PlayerDeaths.ToString()).Append(',');
        }

        return sb.ToString();
    }

public void SaveToFile()
{
    // Use the CSV generation from before
    var content = ToCSV();

    // The target file path e.g.
#if UNITY_EDITOR
    var folder = Application.streamingAssetsPath;

    if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
#else
    var folder = Application.persistentDataPath;
#endif

        string fileName;
        if (enableDDA)
            fileName = "dda_enabled";
        else
            fileName = "dda_disabled";

    var filePath = Path.Combine(folder, fileName+".csv");

    using (var writer = new StreamWriter(filePath, false))
    {
        writer.Write(content);
    }

    // Or just
    //File.WriteAllText(content);

    Debug.Log($"CSV file written to \"{filePath}\"");

#if UNITY_EDITOR
    AssetDatabase.Refresh();
#endif
}

    #endregion
}
