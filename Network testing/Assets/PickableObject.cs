using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    [Header("PlayerHand")]
    public Transform playerHandLocation;

    [Header("PlayerInteractableDistance")]
    public Transform objectOrigin;
    public int interactDistance;
    public LayerMask playerMask;
    public bool canBePickedUp;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        canBePickedUp = Physics.CheckSphere(objectOrigin.position, interactDistance , playerMask);
    }

    void Pickup()
    {
        if (canBePickedUp)
        {
            gameObject.transform.position = playerHandLocation.position;
        }
    }
}
