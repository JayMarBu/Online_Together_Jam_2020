using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: change these god awful nameing conventions used by the tutorial series
public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    // initialize UI objects
    [Header("UI")]
    [SerializeField] private GameObject lobby_ui_ = null;
    [SerializeField] private TMP_Text[] player_name_texts_ = new TMP_Text[4];
    [SerializeField] private TMP_Text[] player_ready_texts_ = new TMP_Text[4];
    [SerializeField] private Button start_game_button_ = null;

    /// <summary>
    /// [SyncVar] are variables that can only be edited by the server.
    /// the hook is a reference to a method that is called when the server changes 
    /// the values of the SyncVar
    /// </summary>

    // name displayed for each player
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string display_name_ = "Loading...";
    
    // boolean to indicate whether a player is ready to start or not
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool is_ready_ = false;

    public int player_number_ = -1;

    // boolean to indicate whether a player is the host or not
    // this variable can only be set publically, otherwise it is private.
    // TODO: change this dumb-ass nameing scheme 
    private bool is_leader_priv_;
    public bool is_leader_
    {
        set
        {
            is_leader_priv_ = value;

            // only show start game button if player is the host
            start_game_button_.gameObject.SetActive(value);
        }
    }

    // reference to the current lobby
    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            {
                // cast network manager singelton to our custom network manager.
                // this will only ever happen once
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }
    }

    // function is run on the initialisation of an object that this client has authority over
    public override void OnStartAuthority()
    {
        // send a command to the server to set the display name
        // TODO: validate names e.g. min max chars, profanity filter ect.
        CmdSetDisplayName(PlayerNameInput.display_name_);

        // only allow this clients ui to display
        lobby_ui_.SetActive(true);
    }

    // activates for all clients regardless of authority
    public override void OnStartClient()
    {
        // add client to list of players
        Room.room_players_.Add(this);

        // update UI to reflect this
        UpdateDisplay();
    }

    public override void OnNetworkDestroy()
    {
        // remove client from list
        Room.room_players_.Remove(this);

        // update UI to reflect this
        UpdateDisplay();
    }

    // methods to be run any time a clients name or reday status changes 
    // which update the UI accordingly
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        // update the other clients version of this clients UI elements
        if(!hasAuthority)
        {
            foreach(var player in Room.room_players_)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        // TODO: this could definetly be made more efficient, 
        // however it doenst actually matter all that much

        // set all players to empty temporarily
        for (int i = 0; i < player_name_texts_.Length; i++)
        {
            player_name_texts_[i].text = "Waiting For Player...";
            player_ready_texts_[i].text = string.Empty;
        }

        // update all players visuals with accurate information
        for (int i = 0; i < Room.room_players_.Count; i++)
        {
            player_name_texts_[i].text = Room.room_players_[i].display_name_;

            if (Room.room_players_[i].is_ready_)
            {
                player_ready_texts_[i].text = "<color=green>Ready</color>";
            }
            else
            {
                player_ready_texts_[i].text = "<color=red>Not Ready</color>";
            }
            //player_ready_texts_[i].text = Room.room_players_[i].is_ready_ ?
            //    "<color=green>Ready</color>" :
            //    "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool ready_to_start)
    {
        // check if user is the host
        if (!is_leader_priv_) { return; }

        // update start game button when appropriate
        start_game_button_.interactable = ready_to_start;
    }

    public void PressReadyButton()
    {
        CmdReadyUp(!is_ready_);
    }

    /// <summary>
    /// [Command] attribute designates that a function should be run only on the server
    /// 
    /// </summary>

    [Command]
    void CmdSetDisplayName(string display_name)
    {
        // TODO: validate name
        display_name_ = display_name;
    }

    [Command]
    void CmdReadyUp(bool ready)
    {
        // toggle is ready variable
        is_ready_ = ready;

        // inform other clients of state change
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        // if this client is not the host, return
        if (Room.room_players_[0].connectionToClient != connectionToClient) { return; }

        // start game
        Room.StartGame();
    }
}
