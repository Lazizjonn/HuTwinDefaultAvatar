using UnityEngine;
using Unity.Netcode;


public class AttachToCamera : MonoBehaviour
{
    private GameObject cameraRig;

    private void Start()
    {
        GameObject XRrig = GameObject.FindGameObjectWithTag("XRrig");
        if (XRrig == null)
        {
            Debug.LogError("--- Static camera controller (XRrig) NOT found2");
            return;
        }
        
        cameraRig = XRrig;
    }


    void Update()
    {
        this.transform.position = cameraRig.transform.position + new Vector3(0, 1.1f, 0.0f);
        this.transform.rotation = cameraRig.transform.rotation;
    }
}