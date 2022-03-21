using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager for buttons in Game Over menu
/// </summary>
public class LoseMenuManager : MonoBehaviour
{
    public void PlayAgain(string sceneName) //Set parameter in inspector, may be changed according to what scene is being tested
    {
        SceneManager.LoadScene(sceneName);
    }
}
