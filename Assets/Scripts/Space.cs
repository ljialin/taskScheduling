using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Space : MonoBehaviour
{
    public List<Worker> _workersList = new List<Worker>();

    public List<Job> _jobsQueue = new List<Job>();

    public int _workerNum = 0;

    public int _areaRate = 100;

    public void QuitWorker(Worker worker)
    {
        _workersList.Remove(worker);
    }

    public void EnterWorker(Worker worker)
    {
        _workersList.Add(worker);
    }
}
