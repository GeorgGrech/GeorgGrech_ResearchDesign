using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manager for buttons in Game Over and WinScene menu
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField] string playSceneName; //Name of scene to be played if 'Play Again' pressed
    [SerializeField] bool resetEntirely; //Dictates whether player accuracy and deaths will be kept from previous session or reset

    GameManager gameManager;
    Toggle DDAToggle;

    void Start()
    {
        //Re-enable cursor on menu load
        Cursor.visible = true;
        Screen.lockCursor = false;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        DDAToggle = GameObject.Find("DDAToggle").GetComponent<Toggle>();

        DDAToggle.isOn = gameManager.enableDDA; //Set initial toggle correctly
    }

    public void Play() //Play level
    {
        gameManager.SetupLevel(resetEntirely);

        SceneManager.LoadScene(playSceneName);
    }

    public void Quit() //Quit game, works for both editor and application
    {
        //Standalone build
#if UNITY_STANDALONE
        //Quit application
        Application.Quit();
#endif

        //Playing in editor
#if UNITY_EDITOR
        //Quit play mode
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void EnableDDA()
    {
        gameManager.enableDDA = DDAToggle.isOn;
    }
}
