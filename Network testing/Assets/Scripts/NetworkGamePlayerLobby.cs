using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: change dumb ass tutorial nameing scheme
public class NetworkGamePlayerLobby : NetworkBehaviour
{
    /// <summary>
    /// [SyncVar] are variables that can only be edited by the server.
    /// the hook is a reference to a method that is called when the server changes 
    /// the values of the SyncVar
    /// </summary>

    // name displayed for each player
    [SyncVar]
    private string display_name_ = "Loading...";

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

    // activates for all clients regardless of authority
    public override void OnStartClient()
    {
        // depending on game structure, this might not be necassary
        DontDestroyOnLoad(gameObject);

        // add client to list of players
        Room.game_players_.Add(this);
    }

    public override void OnNetworkDestroy()
    {
        // remove client from list
        Room.game_players_.Remove(this);
    }

    // [Server] attribute means that code will only be run on the server

    [Server]
    public void SetDisplayName(string display_name)
    {
        this.display_name_ = display_name;
    }
}
