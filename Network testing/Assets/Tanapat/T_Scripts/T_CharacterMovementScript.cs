using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_CharacterMovementScript : MonoBehaviour
{

    #region privateVariables
    [Header("PlayerComponents")]
    private Rigidbody playerRigidBody;
    private CharacterController playerCharacterController;
    [Space]

    [Header("Uncategorised")]
    private Vector3 playerMovement;
    public Vector3 playerFallingVelocity;
    [Space]

    #endregion
    #region publicVariables
    [Header("PlayerProperties")]
    [Range(1, 100)]
    public int playerSpeed = 15;
    [Range(-1, -100)]
    public float playerGravity = -10;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponentInChildren<Rigidbody>();
        playerCharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerFallingVelocity.y += playerGravity * Time.deltaTime;

        playerMovement = new Vector3
            (Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime, playerFallingVelocity.y * Time.deltaTime,
            Input.GetAxisRaw("Vertical") * playerSpeed * Time.deltaTime);


        playerCharacterController.Move(playerMovement);
        //playerCharacterController.Move();

    }
}
