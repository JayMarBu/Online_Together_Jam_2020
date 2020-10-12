using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{

    public Transform playerHandLocation;
    public bool playerHandEmpty;
    public bool inRange;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            playerHandLocation = GameObject.Find("PlayerHand").transform;

            if (Input.GetKeyDown(KeyCode.E) && FindObjectOfType<T_CharacterMovementScript>().playerHandEmpty == true)
            {

                GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                //Can't figure out how to stop rotation, however it could be a feature, so i've always added rotation
                GetComponent<Rigidbody>().AddTorque(10, 10, 0);

                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<BoxCollider>().enabled = false;

                this.transform.position = playerHandLocation.position;
                this.transform.parent = GameObject.Find("PlayerHand").transform;

                Debug.Log("Pickup");
                FindObjectOfType<T_CharacterMovementScript>().playerHandEmpty = false;

                
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<BoxCollider>().enabled = true;

                this.transform.parent = null;
                FindObjectOfType<T_CharacterMovementScript>().playerHandEmpty = true;

                Debug.Log("Drop");
            }
        }
    }


    #region isPlayerInRange
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter");
            inRange = true;
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exit");
            inRange = false;
        }
    }
    #endregion
}
