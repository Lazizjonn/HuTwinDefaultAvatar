using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using AddressFamily = System.Net.Sockets.AddressFamily;
using TMPro;
using System;


public class TestNetworkScript : MonoBehaviour
{
    string sharedLocalIpAddress = "127.0.0.1";
    public TMP_Text deviceIpAddress;
    public TMP_InputField ipTextField;

    // Start is called once before the first execution of Update after the MonoBehavior is created
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

        NetworkManager.Singleton.OnServerStarted += OnHostStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        //NetworkManager.Singleton.StartHost();

        //NetworkManager.Singleton.StartClient();
    }

    private void InstantiateCameraRig()
    {
        //
    }

    private void OnHostStarted()
    {
        //
    }

    private void OnClientConnected(ulong clientId)
    {

        /*MySpawnerScript script = GameObject.Find("SpawnerStaticObject").GetComponent<MySpawnerScript>();
        if(script != null)
        {
            script.CreateClientPrefab(clientId);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //
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

}
