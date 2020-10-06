using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

public class NetworkManagerLobby : NetworkManager
{
    // set the minimum amount of players required to start the game
    [SerializeField] private int min_players_ = 2;

    /// <summary>
    /// [Scene] attribute overides input type alowing developers to drag and drop
    /// scenes in the Inspector into this field rather than requiring them
    /// to type the scene name in.
    /// 
    /// A reference to the scene used as the lobby menu
    /// </summary>
    [Scene] [SerializeField] private string menu_scene_ = string.Empty;

    [Header("Room")]
    // reference to player prefab
    [SerializeField] private NetworkRoomPlayerLobby room_player_prefab_ = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby game_player_prefab_ = null;

    // custom events to be used by other classes
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    //list to store all players in room
    public List<NetworkRoomPlayerLobby> room_players_ { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> game_players_ { get; } = new List<NetworkGamePlayerLobby>();

    

    // STARTUP //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // alter startup code depending on whether or not user is the host (server)
    // or the client.

    // if host, load all prefabs to a list
    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    // if client, load all prefabs into scene 
    public override void OnStartClient()
    {
        var spawnable_prefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnable_prefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    // ON CONNECTION & DISCONNECTION  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // alter connection code depending on whether user is the host (server)
    // or the client
    public override void OnClientConnect(NetworkConnection conn)
    {
        // run base class logic for this function
        base.OnClientConnect(conn);

        Debug.Log("new Client connected");

        // raise custom event
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        // run base class logic for this function
        base.OnClientDisconnect(conn);

        Debug.Log("Client disconnected");

        // raise custom event
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        // cap the amount of players connected at any time to the
        // max number of allowed players
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            Debug.Log("max players reached, player disconnected");
            return;
        }

        // dont allow players to join after the game has started
        if (SceneManager.GetActiveScene().path != menu_scene_)
        {
            conn.Disconnect();
            Debug.Log("incorrect scene, player disconnected");
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            // remove disconnected player from list
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            room_players_.Remove(player);

            // notify other clients
            NotifyPlayersOfReadyState();
        }

        // run base class logic for this function
        base.OnServerDisconnect(conn);
    }

    // NEW PLAYER JOIN //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // called whenever AddPlayer function is called, which is in the
    // base version of OnClientConnect
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Debug.Log("instantiating new player");
        //Debug.Log(menu_scene_);

        // only allow players to join if session is still in lobby
        if (SceneManager.GetActiveScene().path == menu_scene_)
        {
            // set first player to join (the host) to the leader
            bool is_leader = room_players_.Count == 0;

            // instantiate new player
            NetworkRoomPlayerLobby room_player_instance = Instantiate(room_player_prefab_);
            Debug.Log("instantiated new player");

            // set is_leader_boolean as appropriate
            room_player_instance.is_leader_ = is_leader;

            // create link between newly instantiated game object 
            // and new connection
            NetworkServer.AddPlayerForConnection(conn, room_player_instance.gameObject);
            Debug.Log("added new player");
        }
    }

    // ON SERVER STOP ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void OnStopServer()
    {
        // clear list when server closes
        room_players_.Clear();
    }

    // GAME READY ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Functions used to determine whether game is ready to start
    public void NotifyPlayersOfReadyState()
    {
        // allow all connected players to inform all others
        // of current ready status
        foreach(var player in room_players_)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        // game is not ready to start without the minimum required players
        if (numPlayers < min_players_) { return false; }

        // if any of the connected players are not ready to start
        // the game is not ready to start
        foreach(var player in room_players_)
        {
            if (!player.is_ready_) { return false; }
        }

        // all players are ready and there are at least min_players_ players
        return true;
    }

    // START GAME ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menu_scene_)
        {
            if(!IsReadyToStart()) {return;}

            // for now takes players to a temporary testing scene
            // can be changed later
            ServerChangeScene("Scene_Testbed");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == menu_scene_)
        {
            // replace lobby player ovbjects with game player objects
            for (int i = room_players_.Count - 1; i >= 0; i--)
            {
                var conn = room_players_[i].connectionToClient;
                var gameplay_instance = Instantiate(game_player_prefab_);
                gameplay_instance.SetDisplayName(room_players_[i].display_name_);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplay_instance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }
}
