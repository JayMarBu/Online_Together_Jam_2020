using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    // store player character prefab
    [SerializeField] private GameObject player_prefab_ = null;

    // create list of spawn points
    private static List<Transform> spawn_points_ = new List<Transform>();

    // a count of the number of players in game
    private int next_index_;

    public static void AddSpawnPoint(Transform transform)
    {
        // add point to the list of spawn points
        spawn_points_.Add(transform);

        // order them..? (tutorial didnt explain why this was necassary)
        spawn_points_ = spawn_points_.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    // remove point from the list
    public static void RemoveSpawnPoint(Transform transform) => spawn_points_.Remove(transform);

    public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawn_point = spawn_points_.ElementAtOrDefault(next_index_);

        if(spawn_point == null)
        {
            Debug.LogError($"Missing spawn point for player {next_index_}");
            return;
        }

        GameObject player_instance = Instantiate(player_prefab_, spawn_points_[next_index_].position, spawn_points_[next_index_].rotation);
        NetworkServer.Spawn(player_instance, conn);

        next_index_++;
    }

}
