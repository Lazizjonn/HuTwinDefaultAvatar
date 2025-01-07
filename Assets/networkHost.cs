using System.Net;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

public class networkHost : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = "192.168.57.191";
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)7777;
        NetworkManager.Singleton.StartHost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
