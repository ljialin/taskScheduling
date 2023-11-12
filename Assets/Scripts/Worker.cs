using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
using UnityEngine.UI;
using System.Linq;

public class WorkerInfo
{
    public string name;
    public string ability;
    public int time;
    public string joint_jobs = "";

}
public class Worker : MonoBehaviour
{

    public string name;
    public string[] abilities;
    public int timeLimit;
    public float time;
    public double efficiency = 1.0;
    public Job currentJob = null;

    public int _time = 0;
    public int workTime = 0;
    public int restTime = 0;

    public int slotTime = 2 * 60;
    
    public bool ifProcess = false;
    public WorkerInfo workerinfo = new WorkerInfo();
    
    
    //Below is the ui component

    public Text _uiName;

    public Image _uiEff;

    public Image _uiPerson;

    public Image _uiProgress;
    
    //

    public bool _startWork;

    public Image _person;

    public float _workTime;

    public float _tmpStart;

    public float _tmpEnd;
    

    private void Start()
    {

        _uiName.text = name;
        _uiEff.fillAmount = (float)efficiency;
    }
    

    void Update()
    {
        if (ifProcess)
        {
            if (this.currentJob == null)
            {
                RecoverEff();
            }
            else if (this.currentJob != null && _startWork && currentJob.jobProgress > 0 && this.currentJob._startJob)
            {
                CostEff();
                //_workTime
            }

            //efficiency = Mathf.Max((float)((float)(timeLimit - time) / (float)timeLimit),0);
            
            efficiency = Mathf.Max((float)((float)(slotTime - time) / (float)slotTime),0);
            efficiency = Mathf.Min((float)1, (float)efficiency);
            
            _uiEff.fillAmount = (float)efficiency;
               
            _time = Mathf.FloorToInt(UnityEngine.Time.time);
            
        }
    }

    public void Process()
    {
        ifProcess = true;
    }

    public void StartWork(Job job)
    {
        this.currentJob = job;
        _startWork = true;
        if (workerinfo.joint_jobs != null && !workerinfo.joint_jobs.Contains(job.name))
        {
            workerinfo.joint_jobs += job.name + " ";
        }
        
        job._preTime = (float)job._logicController._WorldTime._currentTime.TotalMinutes;
    }

    public void PauseWork()
    {
        currentJob = null;
        _startWork = false;
        _uiProgress.fillAmount = 1;
    }

    public void RecoverEff()
    {
        if (efficiency < 1)
        {
            this.time -= 3 * Params.TIME_SCALE;
            //this.time -= 3 * (int)(currentJob._logicController._WorldTime._currentTime.TotalMinutes - currentJob._preTime);
        }
    }

    public void CostEff()
    {
        //this.time += Params.TIME_SCALE;
        _workTime += (float)(currentJob._logicController._WorldTime._currentTime.TotalMinutes - currentJob._preTime);
        this.time += (float)(currentJob._logicController._WorldTime._currentTime.TotalMinutes - currentJob._preTime);
    }

    public bool Quitable()
    {
        return efficiency >= Params.QUIT_EFF;
    }
    
    public bool Workable(Job tmpJob)
    {
        Debug.Log(tmpJob.name + " QQQQ "+(tmpJob.space._areaRate - tmpJob.areaRatio));
        return efficiency >= Params.WORK_EFF && ((tmpJob.space._areaRate - tmpJob.areaRatio) >= 0);
    }

    public bool IsFree()
    {
        return currentJob == null;
    }
    
}
