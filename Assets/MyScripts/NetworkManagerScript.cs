using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using AddressFamily = System.Net.Sockets.AddressFamily;
using TMPro;
using Oculus.Movement.Tracking;

public class NetworkManagerScript : MonoBehaviour
{
    private string sharedLocalIpAddress = "127.0.0.1";
    [SerializeField] private TMP_Text deviceIpAddress;
    [SerializeField] private TMP_InputField ipTextField;


    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 10;

        sharedLocalIpAddress = FindIpAddress();
        Debug.Log("host ip: " + sharedLocalIpAddress);
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = sharedLocalIpAddress;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)7777;

        deviceIpAddress.text = sharedLocalIpAddress;
        ipTextField.text = "";

        NetworkManager.Singleton.OnClientConnectedCallback += OnNewClientConnected;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFaceExpression();
    }

    public void StartHost() 
    {
        Start();
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        string ipAddress = ipTextField.text;
        Debug.Log("client ip: " + ipAddress);
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipAddress;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)7777;
        NetworkManager.Singleton.StartClient();
    }

    public void Stop()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private string FindIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Debug.Log("FindIpAddress(): ip.ToString() = " + ip.ToString());
                return ip.ToString(); ;
            }
        }

        Debug.LogError("FindIpAddress(): No network adapters with an IPv4 address in the system!");
        return sharedLocalIpAddress;
    }


    // ===================================================================================================== //
    GameObject[] onlinePlayers;

    private MyCorrectivesFace myFaceObject;
    private MyCorrectivesFace hisFaceObject;

    private float[] myExpressions;
    private void OnNewClientConnected(ulong clientId)
    {
        onlinePlayers = GameObject.FindGameObjectsWithTag("FidelityPlayer");
        Debug.LogError("--- OnNewClientConnected(), onlinePlayers.Length: " + onlinePlayers.Length);
        foreach (var player in onlinePlayers)
        {
            Debug.LogError("--- OnNewClientConnected(),  player.IsLocalPlayer: " + player.GetComponent<NetworkObject>().IsLocalPlayer);
            if (player.GetComponent<NetworkObject>().IsLocalPlayer == true)
            {
                myFaceObject = player.GetComponentInChildren<MyCorrectivesFace>();
            }
            else
            {
                hisFaceObject = player.GetComponentInChildren<MyCorrectivesFace>();
            }
        }
    }

    private void UpdateFaceExpression()
    {
        if (onlinePlayers != null && onlinePlayers.Length >= 1)
        {
            Debug.LogError("--- UpdateFaceExpression(), onlinePlayers.Length: " + onlinePlayers.Length);

            myExpressions = myFaceObject.PrepareRemoteExpressionWeights();
            Debug.LogError("--- UpdateFaceExpression(), myExpressions.array.Length: " + myExpressions.Length);
            
            Debug.LogError("--- UpdateFaceExpression(), NetworkManager.IsHost: " + NetworkManager.Singleton.IsHost);
            
            SetExpressionPlayerServerRpc(myExpressions);
           /* if (NetworkManager.Singleton.IsHost)
               
                //SetExpressionPlayerClientRpc(myExpressions);
            else
                SetExpressionPlayerServerRpc(myExpressions);*/
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetExpressionPlayerServerRpc(float[] incomingData)
    {
        //hisFaceObject.UpdateExpressionWeightFromRemote(incomingData);
        myFaceObject.UpdateExpressionWeightFromRemote(incomingData);
    }

    [ClientRpc]
    private void SetExpressionPlayerClientRpc(float[] incomingData)
    {
        if (NetworkManager.Singleton.IsHost)
            return;
        
        hisFaceObject.UpdateExpressionWeightFromRemote(incomingData);
    }
}
