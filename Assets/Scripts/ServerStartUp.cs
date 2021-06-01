using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using PlayFab;

public class ServerStartUp : MonoBehaviour
{
    void Start()
    {
        StartRemoteServer();
    }

    private void StartRemoteServer()
    {
        Debug.Log("[ServerStartUp].StartRemoteServer");

        PlayFabMultiplayerAgentAPI.Start();
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;

        StartCoroutine(ReadyForPlayers());
    }

    private IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(0.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }

    private void OnServerActive()
    {
        Debug.Log("Server Started From Agent Activation");
        // players can now connect to the server
        NetworkManager.Singleton.StartServer();
    }
}