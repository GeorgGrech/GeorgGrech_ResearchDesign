using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Currently Unused, implemented in GameManager
/// </summary>
public class ItemDropSelector : MonoBehaviour
{

    int[] playerVariables, maxVariables;

    public ItemDropSelector(int[] playerVariables, int[] maxVariables)
    {
        this.playerVariables = playerVariables;
        this.maxVariables = maxVariables;
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
