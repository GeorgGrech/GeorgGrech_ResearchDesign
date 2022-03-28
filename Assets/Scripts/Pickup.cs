using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [SerializeField] private int variableIncrease;
    [SerializeField] private bool isHealth;

    GameManager gameManager;
    // Start is called before the first frame update
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            string objectTag = transform.tag;
            if (gameManager.NotAtMax(objectTag)) //if value isn't at max alrady
            {
                if (objectTag.Equals("Health")) //if Medkit
                    gameManager.UpdateHealth(variableIncrease);
                    //collision.gameObject.SendMessageUpwards("ChangeHealth", variableIncrease, SendMessageOptions.DontRequireReceiver); //pass variableIncrease as health to increase

                else //else if ammo
                {
                    gameManager.UpdateAmmo(objectTag, variableIncrease);
                    //object[] parameters = new object[2] { transform.tag, variableIncrease }; //package values into array to fit into 1 parameter
                    //collision.gameObject.SendMessageUpwards("UpdateAmmo", parameters, SendMessageOptions.DontRequireReceiver); //pass variableIncrease as ammo to increase, alongside tag to indictate which ammo type
                }

                Destroy(transform.parent.gameObject); //Destroy after use
            }
        }
    }
}
