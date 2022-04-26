using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager for buttons in Game Over and WinScene menu
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField] string playSceneName; //Name of scene to be played if 'Play Again' pressed
    [SerializeField] bool resetAccuracy; //Dictates whether player accuracy will be kept from previous session or reset

    void Start()
    {
        //Re-enable cursor on menu load
        Cursor.visible = true;
        Screen.lockCursor = false;
    }

    public void PlayAgain() //Play level again
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().SetupLevel(resetAccuracy);

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
}
