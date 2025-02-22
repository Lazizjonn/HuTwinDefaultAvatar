using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using System.IO;
using System;
using Unity.Netcode;

public class ExpressionLogger : MonoBehaviour
{
    [SerializeField] GameObject player;

    private OVRFaceExpressions faceExpressions;
    private string logFilePath;
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
    private Thread logThread;
    private bool isRunning = true;

    void Start()
    {
        faceExpressions = GetComponent<OVRFaceExpressions>();

        if (faceExpressions == null || player == null || (player != null && player.GetComponent<NetworkObject>().IsLocalPlayer == false))
        {
            Debug.LogError("TTT, ExpressionLogger::Start(), OVRFaceExpressions component not found on this GameObject, Start()");
            Debug.LogError("TTT, ExpressionLogger::Start(), PLAYER: " + player.ToString() + ",   SCRIPT: " + this.ToString());
            enabled = false;
            return;
        }

        // Create a unique file name with a timestamp
        string timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
        string fileName = $"face_log_{timeStamp}.txt";

        // Set the log file path to persistent data path with the unique file name
        logFilePath = Path.Combine(Application.persistentDataPath, fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        File.WriteAllText(logFilePath, "Expression Log\n");
        Debug.Log("TTT, Expression log file created at " + logFilePath + ", Start()");

        // Start the logging thread, my new thread
        logThread = new Thread(LogToFile);
        logThread.Start();
    }

    void Update()
    {
        try
        {
            if (faceExpressions.ValidExpressions)
            {
                for (int i = 0; i < faceExpressions.Count; i++)
                {
                    OVRFaceExpressions.FaceExpression expression = (OVRFaceExpressions.FaceExpression)i;
                    float weight = faceExpressions[expression];
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Expression {expression}: {weight}";

                    // Enqueue the log entry
                    logQueue.Enqueue(logEntry);
                    //Debug.Log("TTT, " + logEntry + ", Update()");
                }
            }
            else
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Face expressions are not valid.";
                logQueue.Enqueue(logEntry); // Log invalid state for future debugging
                                            //Debug.LogWarning("TTT, " + logEntry + ", Update()");
            }
        }
        catch (Exception e)
        {
            Debug.Log("TTT, ExpressionLogger::Update() crashed");
        }
    }

    private void LogToFile()
    {
        while (isRunning || !logQueue.IsEmpty)
        {
            if (logQueue.TryDequeue(out string logEntry))
            {
                File.AppendAllText(logFilePath, logEntry + "\n");
            }
            else
            {
                Thread.Sleep(10); // Prevent busy-waiting
            }
        }
    }

    void OnDestroy()
    {
        try
        {
            isRunning = false;
            logThread?.Join(); // Waiting for the thread to finish

            // Log shutdown for debugging purposes
            string shutdownLog = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Logging stopped and application shutting down.";
            File.AppendAllText(logFilePath, shutdownLog + "\n");
            Debug.Log("TTT, " + shutdownLog + ", OnDestroy()");
        }
        catch (Exception e)
        {
            Debug.Log("TTT, ExpressionLogger::OnDestroy() crashed");
        }  
    }
}
