using UnityEngine;
using Unity.Jobs;

public struct PrintJob : IJob
{
    public int jobId;

    public void Execute()
    {
        for (int i = 0; i < 5; i++)
            Debug.Log($"Job#{jobId} {i}");
    }
}

public class MultiThread : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Thread Start");

        PrintJob job1 = new PrintJob { jobId = 1 };
        JobHandle handle1 = job1.Schedule();

        PrintJob job2 = new PrintJob { jobId = 2 };
        JobHandle handle2 = job2.Schedule(handle1);

        handle2.Complete();

        Debug.Log("Main Program End");
    }
}