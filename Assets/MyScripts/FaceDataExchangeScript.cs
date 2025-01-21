using UnityEngine;
using Unity.Netcode;
using Oculus.Movement.Tracking;

[System.Serializable]
public class FaceDataExchangeScript : MonoBehaviour
{
    private GameObject[] onlinePlayers;

    [SerializeField] public ClientShareScript clientFaceShareScript;
    [SerializeField] public HostShareScript hostFaceShareScript;

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
        foreach (var player in onlinePlayers)
        {
            if (player.GetComponent<NetworkObject>().IsLocalPlayer == true)
            {
                myFaceObject = player.GetComponentInChildren<CorrectivesFace>();
            } else
            {
                hisFaceObject = player.GetComponentInChildren<CorrectivesFace>();
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
                SetExpressionPlayerServerRpc(myExpressions);
            }
            else
            {
                SetExpressionPlayerClientRpc(myExpressions);
            }
        }
    }

    private void SetExpressionPlayerServerRpc(float[] hostData)
    {
        hostFaceShareScript.SetFaceData(hostData);

        float[] data = clientFaceShareScript.GetFaceData();
        hisFaceObject.UpdateExpressionWeightFromRemote(data);


        // Local test
        /*hostFaceShareScript.SetFaceData(hostData);

        float[] data = hostFaceShareScript.GetFaceData();
        myFaceObject.UpdateExpressionWeightFromRemote(data);*/
    }

    private void SetExpressionPlayerClientRpc(float[] clientData)
    {
        if (NetworkManager.Singleton.IsHost) { return; }

        clientFaceShareScript.SetFaceData(clientData);

        float[] data = hostFaceShareScript.GetFaceData();
        hisFaceObject.UpdateExpressionWeightFromRemote(data);
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
