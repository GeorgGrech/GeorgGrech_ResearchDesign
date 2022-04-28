using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {

            GameObject.Find("GameManager").GetComponent<GameManager>().curentlyPlaying = false;

            SceneManager.LoadScene("WinScene");
        }
    }
}
