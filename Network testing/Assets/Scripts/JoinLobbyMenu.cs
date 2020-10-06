using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby network_manager_ = null;

    [Header("UI")]
    [SerializeField] private GameObject landing_page_panel_ = null;
    [SerializeField] private TMP_InputField ip_address_input_field_ = null;
    [SerializeField] private Button join_button_ = null;

    private void OnEnable()
    {
        // subscribe to custom events when enabled
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        // unsubscribe from custom events when disabled
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        // allow users to enter desired ip address
        string ip_address = ip_address_input_field_.text;

        // connect client to network
        network_manager_.networkAddress = ip_address;
        network_manager_.StartClient();

        // dis-allow users from spamming the button
        join_button_.interactable = false;
    }

    private void HandleClientConnected()
        // function runs if client has successfully 
        // connected to host
    {
        // re-enable join button incase user goes back
        // to main menu
        join_button_.interactable = true;

        // disable this object and landing page
        gameObject.SetActive(false);
        landing_page_panel_.SetActive(false);
    }

    private void HandleClientDisconnected()
        // funtion runs if client failed
        // to connect to a host
    {
        Debug.Log("client failed to connect");
        join_button_.interactable = true;
    }
}
