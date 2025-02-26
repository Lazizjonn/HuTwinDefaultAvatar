using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using System.IO;
using System;
using Unity.Netcode;

public class BodyJointLogger : MonoBehaviour
{
    [SerializeField] GameObject player;

    private OVRSkeleton skeleton;
    private string logFilePath;
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
    private CancellationTokenSource cts = new();
    private Thread logThread;
    private bool isRunning = true;

    private string message = null;

    void Start()
    {
        skeleton = GetComponent<OVRSkeleton>();

        if (skeleton == null || player == null || (player != null && player.GetComponent<NetworkObject>().IsLocalPlayer == false))
        {
            Debug.LogError("TTT, BodyJointLogger::Start(), OVRSkeleton component not found.");
            enabled = false;
            return;
        }

        // Create a unique file name with a timestamp
        string timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
        string fileName = $"bone_log_{timeStamp}.csv";

        // Set the log file path to persistent data path with the unique file name
        logFilePath = Path.Combine(Application.persistentDataPath, fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        File.WriteAllText(logFilePath, "Time,Bone,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW\n");
        Debug.Log("TTT, BodyJointLogger::Start(), Bone log file created at: " + logFilePath);

        // Start the logging thread
        logThread = new Thread(() => LogToFile(cts.Token));
        logThread.Start();
    }

    void Update()
    {
        try
        {
            if (skeleton.IsDataValid && skeleton.Bones != null)
            {
                foreach (var bone in skeleton.Bones)
                {
                    if (bone != null && bone.Transform != null)
                    {
                        Vector3 position = bone.Transform.position;
                        Quaternion rotation = bone.Transform.rotation;
                        // Format the log entry
                        string logEntry =   $"{DateTime.Now:hh:mm:ss.fff tt}, Bone {bone.Id}, " +
                                            $"{position.x:F3}, {position.y:F3}, {position.z:F3}, " +
                                            $"{rotation.x:F3}, {rotation.y:F3}, {rotation.z:F3}, {rotation.w:F3}";

                        logQueue.Enqueue(logEntry);
                    }
                }
            }
            else
            {
                // Log invalid skeleton data
                string logEntry = $"{DateTime.Now:hh:mm:ss.fff tt}, Skeleton data is not valid or bones are missing.";
                logQueue.Enqueue(logEntry);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("TTT, BodyBoneLogger::Update() crashed: " + e.ToString());
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

            Thread.Sleep(50); // wait until logThread stops and then we can write final log

            // Final log for debugging purposes
            string shutdownLog = $"{DateTime.Now:hh:mm:ss.fff tt},Shutdown,Application shutting down.";
            File.AppendAllText(logFilePath, shutdownLog + "\n");
            Debug.Log("TTT, ExpressionLogger::OnDestroy(), message: " + shutdownLog);
        }
        catch (Exception e)
        {
            Debug.LogError("TTT, BodyBoneLogger::OnDestroy() crashed: " + e.ToString());
        }
    }
}
