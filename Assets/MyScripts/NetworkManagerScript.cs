using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using AddressFamily = System.Net.Sockets.AddressFamily;
using TMPro;

public class NetworkManagerScript : MonoBehaviour
{
    private string sharedLocalIpAddress = "127.0.0.1";
    [SerializeField] private TMP_Text deviceIpAddress;
    [SerializeField] private TMP_InputField ipTextField;
    [SerializeField] private TMP_InputField maxPayload;
    [SerializeField] private TMP_InputField maxPacketQueue;


    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 20;

        sharedLocalIpAddress = FindIpAddress();
        Debug.Log("host ip: " + sharedLocalIpAddress);
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = sharedLocalIpAddress;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)7777;

        deviceIpAddress.text = sharedLocalIpAddress;
        ipTextField.text = "";

        NetworkManager.Singleton.OnClientConnectedCallback += OnNewClientConnected;
    }

    public void OnNewClientConnected(ulong clientId)
    {
        GameObject[] onlinePlayers = GameObject.FindGameObjectsWithTag("FidelityPlayer");
        foreach (var player in onlinePlayers)
        {
            player.GetComponentInChildren<FaceDataExchangeScript>().OnNewClientConnected(clientId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public void StartHost()
    {
        Start();
        SetMaxPacketQueueSize();
        SetMaxPayloadSize();
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        string ipAddress = ipTextField.text;
        Debug.Log("client ip: " + ipAddress);
        SetMaxPacketQueueSize();
        SetMaxPayloadSize();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 20;


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

    private void SetMaxPayloadSize()
    {
        string input = maxPayload.text.Trim();

        if (string.IsNullOrEmpty(input))
        {
            Debug.LogWarning("Input is empty. Defaulting to 1400.");
            NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPayloadSize = 1400;
            return;
        }


        if (int.TryParse(input, out int payloadSize))
        {
            if (payloadSize > 0)
            {
                Debug.Log("TTT, SetMaxPayloadSize(), maxPayload = " + maxPayload.text);
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPayloadSize = payloadSize;
            }
            else
            {
                Debug.LogWarning("Invalid number. Payload size must be greater than 0. Defaulting to 1400.");
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPayloadSize = 1400;
            }
        }
        else
        {
            Debug.LogWarning("Invalid Max Payload Size. Please enter a valid number. Defaulting to 1400.");
            NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPayloadSize = 1400;
        }
    }


    private void SetMaxPacketQueueSize()
    {
        string input = maxPacketQueue.text.Trim();

        if (string.IsNullOrEmpty(input))
        {
            Debug.LogWarning("TTT, Input is empty. Defaulting to 128.");
            NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize = 128;
            return;
        }


        if (int.TryParse(input, out int packetQueueSize))
        {
            if (packetQueueSize > 0)
            {
                Debug.Log("TTT, SetMaxPacketQueueSize(), maxPacketQueue = " + maxPacketQueue.text);
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize = packetQueueSize;
            }
            else
            {
                Debug.LogWarning("TTT, Invalid number. Packet queue size must be greater than 0. Defaulting to 128.");
                NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize = 128;
            }
        }
        else
        {
            Debug.LogWarning("TTT, Invalid Max Packet Queue Size. Please enter a valid number. Defaulting to 128.");
            NetworkManager.Singleton.GetComponent<UnityTransport>().MaxPacketQueueSize = 128;
        }
    }

}