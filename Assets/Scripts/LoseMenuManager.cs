using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager for buttons in Game Over menu
/// </summary>
public class LoseMenuManager : MonoBehaviour
{
    void Start()
    {
        //Re-enable cursor on menu load
        Cursor.visible = true;
        Screen.lockCursor = false;
    }

    public void PlayAgain(string sceneName) //Set parameter in inspector, may be changed according to what scene is being tested
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().SetToDefault();

        SceneManager.LoadScene(sceneName);
    }
}
