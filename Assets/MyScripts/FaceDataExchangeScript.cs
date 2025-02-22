using UnityEngine;
using Unity.Netcode;
using Oculus.Movement.Tracking;
using System;

[System.Serializable]
public class FaceDataExchangeScript : NetworkBehaviour
{
    private GameObject[] onlinePlayers;

    private ClientShareScript clientFaceShareScript;
    private HostShareScript hostFaceShareScript;

    [SerializeField] public GameObject myPlayer;

    private CorrectivesFace[] myFaceObject;
    private CorrectivesFace[] hisFaceObject;
    private float[] myExpressions;

    void Start()
    {
        hostFaceShareScript = GameObject.FindGameObjectWithTag("FaceShareTransitionObj").GetComponentInChildren<HostShareScript>();
        clientFaceShareScript = GameObject.FindGameObjectWithTag("FaceShareTransitionObj").GetComponentInChildren<ClientShareScript>();

        myFaceObject = myPlayer.GetComponentsInChildren<CorrectivesFace>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            UpdateFaceExpression();
        } 
        catch (Exception e)
        {
            Debug.Log("TTT, FaceDataExchangeScript::Update() crashed");
        }

    }

    public void OnNewClientConnected(ulong clientId)
    {
        onlinePlayers = GameObject.FindGameObjectsWithTag("FidelityPlayer");
        foreach (var player in onlinePlayers)
        {
            if (player.GetComponent<NetworkObject>().IsLocalPlayer == true)
            {
                myFaceObject = player.GetComponentsInChildren<CorrectivesFace>();
            }
            else
            {
                hisFaceObject = player.GetComponentsInChildren<CorrectivesFace>();
            }
        }
    }

    private void UpdateFaceExpression()
    {
        if (NetworkManager.Singleton != null && myFaceObject != null && hisFaceObject != null)
        {
            myExpressions = myFaceObject[0].PrepareRemoteExpressionWeights();
            Debug.LogError("--- UpdateFaceExpression(), myExpressions.array.Length: " + myExpressions.Length);
            
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
        hisFaceObject[0]?.UpdateExpressionWeightFromRemote(data);
        hisFaceObject[1]?.UpdateExpressionWeightFromRemote(data);
    }

    private void SetExpressionPlayerClient(float[] clientData)
    {
        if (NetworkManager.Singleton.IsHost) { return; }

        clientFaceShareScript.SetFaceData(clientData);

        float[] data = hostFaceShareScript.GetFaceData();
        hisFaceObject[0]?.UpdateExpressionWeightFromRemote(data);
        hisFaceObject[1]?.UpdateExpressionWeightFromRemote(data);
    }
}
