using UnityEngine;
using Unity.Netcode;
using Oculus.Movement.Tracking;

public class FaceDataExchangeScript : NetworkBehaviour
{
    private GameObject[] onlinePlayers;

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
        if (NetworkManager.Singleton != null && onlinePlayers != null && onlinePlayers.Length >= 1)
        {
            Debug.LogError("--- UpdateFaceExpression(), onlinePlayers.Length: " + onlinePlayers.Length);

            myExpressions = myFaceObject.PrepareRemoteExpressionWeights();
            Debug.LogError("--- UpdateFaceExpression(), myExpressions.array.Length: " + myExpressions.Length);

            if (NetworkManager.Singleton.IsHost)
            {
                SetExpressionPlayerClientRpc(myExpressions, NetworkManager.Singleton.IsHost);
            } else
            {
                SetExpressionPlayerServerRpc(myExpressions, NetworkManager.Singleton.IsHost);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SetExpressionPlayerServerRpc(float[] incomingData, bool isHost)
    {
        hisFaceObject.UpdateExpressionWeightFromRemote(incomingData);
    }

    [Rpc(SendTo.NotServer)]
    private void SetExpressionPlayerClientRpc(float[] incomingData, bool isHost)
    {
        hisFaceObject.UpdateExpressionWeightFromRemote(incomingData);
    }
}
