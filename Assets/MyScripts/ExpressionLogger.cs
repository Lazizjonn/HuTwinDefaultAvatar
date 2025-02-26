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
    private CancellationTokenSource cts = new();
    private Thread logThread;
    private bool isRunning = true;

    private string message = null;

    void Start()
    {
        faceExpressions = GetComponent<OVRFaceExpressions>();

        if (faceExpressions == null || player == null || (player != null && player.GetComponent<NetworkObject>().IsLocalPlayer == false))
        {
            Debug.LogError("TTT, ExpressionLogger::Start(), OVRFaceExpressions component not found.");
            enabled = false;
            return;
        }

        // Create a unique file name with a timestamp
        string timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
        string fileName = $"expression_log_{timeStamp}.csv";

        // Set the log file path to persistent data path with the unique file name
        logFilePath = Path.Combine(Application.persistentDataPath, fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        File.WriteAllText(logFilePath, "Time,Expression,Weight\n");
        Debug.Log("TTT, ExpressionLogger::Start(), Expression log file created at " + logFilePath);

        // Start the logging thread
        logThread = new Thread(() => LogToFile(cts.Token));
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

                    // Enqueue the log entry
                    string logEntry = $"{DateTime.Now:hh:mm:ss.fff tt},{expression},{weight}";
                    logQueue.Enqueue(logEntry);
                }
            }
            else
            {
                // Log invalid state
                string logEntry = $"{DateTime.Now:hh:mm:ss.fff tt},Invalid,N/A";
                logQueue.Enqueue(logEntry);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("TTT, ExpressionLogger::Update() crashed: " + e.ToString());
        }
    }

    private void LogToFile(CancellationToken token)
    {
        while (isRunning || !logQueue.IsEmpty)
        {
            if (token.IsCancellationRequested) break;       // quits the loop to stop current process

            if (message != null)
            {
                File.AppendAllText(logFilePath, message);
                message = null;
            }

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

    public void AddTaskLevelMessage(string taskLevel)
    {
        string taskLevelLog = $"########################   New level - {taskLevel}   ########################";
        message = "\n" + taskLevelLog + "\n";
    }

    void OnDestroy()
    {
        try
        {
            isRunning = false;

            // stopping process, which makes logThread stopped
            cts?.Cancel();
            cts?.Dispose();

            Thread.Sleep(50); // wait until logThread stoppes and then we can write final log

            // Final log for debugging purposes
            string shutdownLog = $"{DateTime.Now:hh:mm:ss.fff tt},Shutdown,Application shutting down.";
            File.AppendAllText(logFilePath, shutdownLog + "\n");
            Debug.Log("TTT, ExpressionLogger::OnDestroy(), message: " + shutdownLog);
        }
        catch (Exception e)
        {
            Debug.LogError("TTT, ExpressionLogger::OnDestroy() crashed: " + e.ToString());
        }  
    }
}