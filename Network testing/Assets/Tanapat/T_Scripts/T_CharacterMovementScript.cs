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

    [Space]

    [Header("Uncategorised")]

    private Vector3 playerMovement;
    public Vector3 playerFallingVelocity;
    [Space]

    #endregion
    #region publicVariables
    [Header("PlayerReferenced")]
    public Transform playerHand;
    [Space]

    [Header("PlayerProperties")]
    [Range(1, 100)]
    public int playerSpeed = 15;
    [Range(-1, -100)]
    public float playerGravity = -10;
    [Range(1, 100)]
    public float playerJumpHeight = 2;
    [Space]

    [Header("PlayerHand")]
    public bool playerHandEmpty = true;
    [Space]

    [Header("PlayerGroundChecks")]
    public Transform playerGroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;


    #endregion

    // Grab components
    void Start()
    {
        playerRigidBody = GetComponentInChildren<Rigidbody>();
        playerCharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        #region FallingJump
        //Projects a sphere underneath player to check ground layer
        isGrounded = Physics.CheckSphere(playerGroundCheck.position, groundDistance, groundMask);
        
        //Player recieves a constant y velocity from gravity
        playerFallingVelocity.y += playerGravity * Time.deltaTime;

        if(isGrounded && playerFallingVelocity.y < 0)
        {
            playerFallingVelocity.y = -1f;
        }

        //Player can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerFallingVelocity.y = Mathf.Sqrt(playerJumpHeight * -2 * playerGravity);
        }
        #endregion

        #region PlayerMovement
        //Complete player movement
        playerMovement = new Vector3
            (Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime, playerFallingVelocity.y * Time.deltaTime,
            Input.GetAxisRaw("Vertical") * playerSpeed * Time.deltaTime);
        

        playerCharacterController.Move(playerMovement);
        #endregion

    }
}
