using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [SerializeField] private int variableIncrease;
    [SerializeField] private bool isHealth;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (isHealth) //if Medkit
                collision.gameObject.SendMessageUpwards("ChangeHealth", variableIncrease, SendMessageOptions.DontRequireReceiver); //pass variableIncrease as health to increase

            else //else if ammo
            {
                object[] parameters = new object[2] { transform.tag, variableIncrease }; //package values into array to fit into 1 parameter
                collision.gameObject.SendMessageUpwards("UpdateAmmo", parameters, SendMessageOptions.DontRequireReceiver); //pass variableIncrease as ammo to increase, alongside tag to indictate which ammo type
            }

            Destroy(gameObject); //Destroy after use
        }
    }
}
