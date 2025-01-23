using UnityEngine;
using Unity.Netcode;
using Oculus.Movement.Tracking;
using System.Collections;

[System.Serializable]
public class FaceDataExchangeScript : NetworkBehaviour
{
    private GameObject[] onlinePlayers;

    private ClientShareScript clientFaceShareScript;
    private HostShareScript hostFaceShareScript;

    private CorrectivesFace myFaceObject;
    private CorrectivesFace hisFaceObject;
    private float[] myExpressions;

    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFaceExpression();
    }

    public void OnNewClientConnected(ulong clientId)
    {
        onlinePlayers = GameObject.FindGameObjectsWithTag("FidelityPlayer");
        Debug.LogError("--- OnNewClientConnected(), onlinePlayers.Length: " + onlinePlayers.Length);

        // disable unneded scripts first
        foreach (var player in onlinePlayers)
        {
            if (player.GetComponent<NetworkObject>().IsLocalPlayer == true)     // local player
            {
                myFaceObject = player.GetComponentInChildren<CorrectivesFace>();

                if (NetworkManager.Singleton.IsHost)                            // if true means - this is my local player and I am host
                {
                    // keep needed script reference
                    hostFaceShareScript = player.GetComponentInChildren<HostShareScript>();

                    // disable unnecessary empty object
                    player.GetComponentInChildren<ClientShareScript>().enabled = false;
                }
                else                                                            // means - this is my local player and I am client
                {
                    // keep needed script reference
                    clientFaceShareScript = player.GetComponentInChildren<ClientShareScript>();

                    // disable unnecessary empty object
                    player.GetComponentInChildren<HostShareScript>().enabled = false;
                }
            }
        }

        // Wait a bit
        //StartCoroutine(DelayAction(0.1f));

        foreach (var player in onlinePlayers)
        {
            if (player.GetComponent<NetworkObject>().IsLocalPlayer == false)    // not local player
            {
                hisFaceObject = player.GetComponentInChildren<CorrectivesFace>();

                if (clientFaceShareScript == null)                              //if true means - this is remote player and client, not host.
                {
                    // keep needed script reference
                    clientFaceShareScript = player.GetComponentInChildren<ClientShareScript>();

                    // disable unnecessary empty object
                    player.GetComponentInChildren<HostShareScript>().enabled = false;
                }
                else                                                            // means - this is remote player and host, not client.
                {
                    // keep needed script reference
                    hostFaceShareScript = player.GetComponentInChildren<HostShareScript>();

                    // disable unnecessary empty object
                    player.GetComponentInChildren<ClientShareScript>().enabled = false;
                }
            }
        }
    }

    private void UpdateFaceExpression()
    {
        if (NetworkManager.Singleton != null && myFaceObject != null /*&& hisFaceObject != null*/)                 // ===================================
        {
            myExpressions = myFaceObject.PrepareRemoteExpressionWeights();
            //Debug.LogError("--- UpdateFaceExpression(), myExpressions.array.Length: " + myExpressions.Length);
            //clientFaceShareScript.SetFaceData(myExpressions);

            if (NetworkManager.Singleton.IsHost)
            {
                SetExpressionPlayerServer(myExpressions);
            }
            else
            {
                SetExpressionPlayerClient(myExpressions);
            }
        }
    }

    private void SetExpressionPlayerServer(float[] hostData)
    {
        hostFaceShareScript.SetFaceData(hostData);

        float[] data = clientFaceShareScript.GetFaceData();
        hisFaceObject.UpdateExpressionWeightFromRemote(data);


        // Local test
        /*hostFaceShareScript.SetFaceData(hostData);

        float[] data = hostFaceShareScript.GetFaceData();
        myFaceObject.UpdateExpressionWeightFromRemote(data);*/
    }

    private void SetExpressionPlayerClient(float[] clientData)
    {
        if (NetworkManager.Singleton.IsHost) { return; }

        clientFaceShareScript.SetFaceData(clientData);

        float[] data = hostFaceShareScript.GetFaceData();
        hisFaceObject.UpdateExpressionWeightFromRemote(data);
    }

    IEnumerator DelayAction(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.
    }


    /*//[Rpc(SendTo.Server)]
    [ServerRpc]
    private void SetExpressionPlayerServerRpc(float[] incomingData)
    {
        hisFaceObject.UpdateExpressionWeightFromRemote(incomingData);
    }

    //[Rpc(SendTo.NotServer)]
    [ClientRpc]
    private void SetExpressionPlayerClientRpc(float[] incomingData)
    {
        if (NetworkManager.Singleton.IsHost) { return; }
        
        hisFaceObject.UpdateExpressionWeightFromRemote(incomingData);
    }*/
}
