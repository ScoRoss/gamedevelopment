using System.IO;
using UnityEngine;
using System.Diagnostics; 
using System.Linq;
using FMS_AdvancedZombieAI;

public class PerformanceLogger : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        filePath = "C:\\Users\\rossl\\OneDrive\\Desktop\\UWS\\hounors project\\CSV data\\PerformanceData.csv";


        // Write CSV header if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Time,FPS,CPU_Frequency(GHz),RAM_Used(MB),GPU_Model,Zombie_Count,Method\n");
        }
    }

    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        float cpuFrequency = SystemInfo.processorFrequency / 1000f; // CPU clock speed in GHz
        float ramUsed = SystemInfo.systemMemorySize - SystemInfo.graphicsMemorySize; // Approximate RAM usage
        string gpuModel = SystemInfo.graphicsDeviceName;
        int zombieCount = FindObjectsOfType<AntColonyZombieAI>().Length;
        string method = "AntColony"; // Change dynamically based on test

        // Write data to CSV
        File.AppendAllText(filePath, $"{Time.time},{fps},{cpuFrequency},{ramUsed},{gpuModel},{zombieCount},{method}\n");
    }
}
