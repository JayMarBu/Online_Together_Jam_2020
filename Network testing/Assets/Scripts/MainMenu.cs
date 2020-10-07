using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby network_manager_ = null;

    [Header("UI")]
    [SerializeField] private GameObject landing_page_panel_ = null;

    // function run when user decides to become host
    // a.k.a. presses host button
    public void HostLobby()
    {
        //network_manager_.networkAddress = "192.168.0.1";
        network_manager_.StartHost();

        landing_page_panel_.SetActive(false);
    }
}
// this is a comment
