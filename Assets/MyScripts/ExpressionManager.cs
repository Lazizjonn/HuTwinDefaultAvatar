using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System;


public class ExpressionManager : MonoBehaviour
{
    private OVRFaceExpressions faceExpressions;
    public float[] remoteExpressionWeights;
    public event Action<float[]> OnExpressionWeightsUpdated;

    void Start()
    {
        faceExpressions = GetComponent<OVRFaceExpressions>();

        if (faceExpressions == null)
        {
            Debug.LogError("TTT, OVRFaceExpressions component not found on this GameObject, Start()");
            enabled = false;
            return;
        }

        
        remoteExpressionWeights = new float[(int)OVRFaceExpressions.FaceExpression.Max];
    }

    void Update()
    {
        if (faceExpressions.ValidExpressions)
        {
            for (int i = 0; i < faceExpressions.Count; i++)
            {
                OVRFaceExpressions.FaceExpression expression = (OVRFaceExpressions.FaceExpression)i;
                remoteExpressionWeights[i] = faceExpressions[expression];
            }

            
            OnExpressionWeightsUpdated?.Invoke(remoteExpressionWeights);

            Debug.Log("TTT, Updated remote expression weights, Update()");
        }
        else
        {
            Debug.LogWarning("TTT, Face expressions are not valid, Update()");
            Array.Clear(remoteExpressionWeights, 0, remoteExpressionWeights.Length);
        }
    }
}
