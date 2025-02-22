using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using System.IO;
using System;
using Unity.Netcode;

public class BodyBoneLogger : MonoBehaviour
{
    [SerializeField] GameObject player;

    private OVRSkeleton skeleton;
    private string logFilePath;
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
    private CancellationTokenSource cts = new();
    private Thread logThread;
    private bool isRunning = true;

    void Start()
    {
        skeleton = GetComponent<OVRSkeleton>();

        if (skeleton == null || player == null || (player != null && player.GetComponent<NetworkObject>().IsLocalPlayer == false))
        {
            Debug.LogError("TTT, BodyBoneLogger::Start(), OVRSkeleton component not found on this GameObject, Start()");
            Debug.LogError("TTT, BodyBoneLogger::Start(), PLAYER: " + player.ToString() + ",   SCRIPT: " + this.ToString());
            enabled = false;
            return;
        }

        // Create a unique file name with a timestamp
        string timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
        string fileName = $"bone_log_{timeStamp}.txt";

        // Set the log file path to persistent data path with the unique file name
        logFilePath = Path.Combine(Application.persistentDataPath, fileName);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        File.WriteAllText(logFilePath, "Bone Log\n");
        Debug.Log("TTT, Bone log file created at " + logFilePath + ", Start()");

        // Start the logging thread, my new thread
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
                        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Bone {bone.Id} - Position: (X: {position.x:F3}, Y: {position.y:F3}, Z: {position.z:F3}), Rotation: (X: {rotation.x:F3}, Y: {rotation.y:F3}, Z: {rotation.z:F3}, W: {rotation.w:F3})";

                        logQueue.Enqueue(logEntry);
                        //Debug.Log("TTT, " + logEntry + ", Update()");
                    }
                }
            }
            else
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Skeleton data is not valid or bones are missing.";
                logQueue.Enqueue(logEntry);
                //Debug.LogWarning("TTT, " + logEntry + ", Update()");
            }
        }
        catch (Exception e)
        {
            Debug.Log("TTT, BodyBoneLogger::Update() crashed");
        }
    }

    private void LogToFile(CancellationToken token)
    {
        while (isRunning || !logQueue.IsEmpty)
        {
            if (token.IsCancellationRequested) break;       // quits the loop to stop current process
            
            if (logQueue.TryDequeue(out string logEntry))
            {
                File.AppendAllText(logFilePath, logEntry + "\n\n");
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
            cts?.Cancel();      // stopping process, which makes Thread stopped
            cts?.Dispose();     // stopping process, which makes Thread stopped

            string shutdownLog = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Logging stopped and application shutting down.";
            File.AppendAllText(logFilePath, shutdownLog + "\n");
            Debug.Log("TTT, " + shutdownLog + ", OnDestroy()");
        }
        catch (Exception e)
        {
            Debug.Log("TTT, BodyBoneLogger::OnDestroy() crashed");
        }
    }
}
