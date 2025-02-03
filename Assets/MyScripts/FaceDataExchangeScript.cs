using UnityEngine;
using Unity.Netcode;
using Oculus.Movement.Tracking;

[System.Serializable]
public class FaceDataExchangeScript : NetworkBehaviour
{
    private GameObject[] onlinePlayers;

    private ClientShareScript clientFaceShareScript;
    private HostShareScript hostFaceShareScript;

    [SerializeField] public GameObject myPlayer;

    private CorrectivesFace myFaceObject;
    private CorrectivesFace hisFaceObject;
    private float[] myExpressions;

    void Start()
    {
        hostFaceShareScript = GameObject.FindGameObjectWithTag("FaceShareTransitionObj").GetComponentInChildren<HostShareScript>();
        clientFaceShareScript = GameObject.FindGameObjectWithTag("FaceShareTransitionObj").GetComponentInChildren<ClientShareScript>();

        myFaceObject = myPlayer.GetComponentInChildren<CorrectivesFace>();
        //Debug.LogError("--- Start(), this.GetInstanceID: " + this.GetInstanceID() + ", this.NetworkObjectId" + this.NetworkObjectId);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFaceExpression();
    }

    public void OnNewClientConnected(ulong clientId)
    {
        onlinePlayers = GameObject.FindGameObjectsWithTag("FidelityPlayer");
        foreach (var player in onlinePlayers)
        {
            if (player.GetComponent<NetworkObject>().IsLocalPlayer == true)
            {
                myFaceObject = player.GetComponentInChildren<CorrectivesFace>();
                //Debug.LogError("--- + OnNewClientConnected(), myFaceObject is Null: " + (myFaceObject == null) + ", this.GetInstanceID: " + this.GetInstanceID() + ", this.NetworkObjectId" + this.NetworkObjectId);
            }
            else
            {
                hisFaceObject = player.GetComponentInChildren<CorrectivesFace>();
                //Debug.LogError("--- + OnNewClientConnected(), hisFaceObject is Null: " + (hisFaceObject == null) + ", this.GetInstanceID: " + this.GetInstanceID() + ", this.NetworkObjectId" + this.NetworkObjectId);
            }
        }
    }

    private void UpdateFaceExpression()
    {
        if (NetworkManager.Singleton != null && myFaceObject != null /*&& hisFaceObject != null*/)                 // ===================================
        {
            myExpressions = myFaceObject.PrepareRemoteExpressionWeights();
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
        //Debug.LogError("--- + SetExpressionPlayerServer(), hisFaceObject is Null: " + (hisFaceObject == null) + ", this.GetInstanceID: " + this.GetInstanceID() + ", this.NetworkObjectId" + this.NetworkObjectId);
        hostFaceShareScript.SetFaceData(hostData);

        float[] data = clientFaceShareScript.GetFaceData();
        hisFaceObject.UpdateExpressionWeightFromRemote(data);
    }

    private void SetExpressionPlayerClient(float[] clientData)
    {
        if (NetworkManager.Singleton.IsHost) { return; }

        //Debug.LogError("--- + SetExpressionPlayerClient(), hisFaceObject is Null: " + (hisFaceObject == null) + ", this.GetInstanceID: " + this.GetInstanceID() + ", this.NetworkObjectId" + this.NetworkObjectId);
        clientFaceShareScript.SetFaceData(clientData);

        float[] data = hostFaceShareScript.GetFaceData();
        hisFaceObject.UpdateExpressionWeightFromRemote(data);
    }
}
