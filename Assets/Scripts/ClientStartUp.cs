using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;

public class ClientStartUp : MonoBehaviour
{
    public string buildId;
    public string ipAddress;
    public ushort port;

    private void Start()
    {
        OnLoginUserButtonClick();
    }

    public void OnLoginUserButtonClick()
    {
        if (string.IsNullOrEmpty(buildId))
        {
            throw new Exception("A remote client build must have a buildId. Add it to the Configuration. Get this from your Multiplayer Game Manager in the PlayFab web console.");
        }
        else
        {
            LoginRemoteUser();
        }
    }

    public void LoginRemoteUser()
    {
        Debug.Log("[ClientStartUp].LoginRemoteUser");

        //We need to login a user to get at PlayFab API's. 
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = GUIDUtility.getUniqueID()
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
    }    

    private void OnPlayFabLoginSuccess(LoginResult response)
    {
        Debug.Log("OnPlayFabLoginSuccess: " + response.ToString());
        if (ipAddress == string.Empty)
        {   //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer();
        }
        else
        {
            ConnectRemoteClient();
        }
    }

    private void OnLoginError(PlayFabError response)
    {
        Debug.Log(response.ToString());
    }

    private void RequestMultiplayerServer()
    {
        Debug.Log("[ClientStartUp].RequestMultiplayerServer");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
        requestData.BuildId = buildId;
        requestData.SessionId = System.Guid.NewGuid().ToString();
        requestData.PreferredRegions = new List<string>() { AzureRegion.EastUs.ToString() };
        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        Debug.Log(response.ToString());
        ConnectRemoteClient(response);
    }

    private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
    {
        if (response == null)
        {
            //networkManager.networkAddress = ipAddress;
            //telepathyTransport.port = port;
            //apathyTransport.port = port;
        }
        else
        {
            Debug.Log("**** ADD THIS TO YOUR CONFIGURATION **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
            //networkManager.networkAddress = response.IPV4Address;
            //telepathyTransport.port = (ushort)response.Ports[0].Num;
            //apathyTransport.port = (ushort)response.Ports[0].Num;
        }
        
        NetworkManager.Singleton.StartClient();
    }

    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        Debug.Log(error.ErrorDetails);
    }
}
