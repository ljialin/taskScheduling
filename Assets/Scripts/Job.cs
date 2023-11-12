 using System.Collections;
using System.Linq;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Controllers;
 using DG.Tweening;
 using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
using System;
using System.Collections.Generic;

public class JobInfo
{
    public string name;
    public int reqPeople;
    public string reqSkill;
    public float avgTime;
    public float seq;
    public int safe;
    public int pollution;
    public Space space;
    public float areaRatio;

    public bool notworkable;
    public float jobProgress;
    
    public override string ToString()
    {
        return name+" , "+reqPeople+" , "+areaRatio;
    }
}


public class Job : MonoBehaviour
{
    public string name;
    public int reqPeople;
    public Dictionary<string, int> reqSkill;
    public float avgTime;
    public float seq;
    public int safe;
    public int pollution;
    public Space space;
    public float areaRatio;

    public bool notworkable;
    public float jobProgress;
    public List<Worker> workers;

    public bool passive = true;

    public bool IfProcess = false;
    public LogicController _logicController;

    public List<Job> jobList = new List<Job>();
    
    // Below is the ui component
    public Text _uiName;

    public Text _reqSkill;
    
    public float fullProgress;

    public bool _startJob = false;

    public Text _ProText;

    private GameObject mask;

    public float _timeProcess;

    public float _preTime;

    public float _endTime;
    
    private void Awake()
    {
        //_logicController = GameObject.Find("Controller1").GetComponent<LogicController>();
    }


    private void Start()
    {
        _logicController = GameObject.Find("Controller1").GetComponent<LogicController>();

        
        _uiName.text = name;
        mask = transform.Find("mask").gameObject;
    }


    public void SetData(JobInfo jobInfo) 
    {
        if (jobInfo == null) 
        {
            return;
        }
        

        this.name = jobInfo.name;
        this.reqPeople = jobInfo.reqPeople;
        this.reqSkill = !string.IsNullOrEmpty(jobInfo.reqSkill) ? new Dictionary<string, int>() {
        {
            jobInfo.reqSkill.Substring(1), int.Parse(jobInfo.reqSkill.Substring(0, 1))
        } } : null;
        this.avgTime = jobInfo.avgTime;
        this.seq = jobInfo.seq;
        this.safe = jobInfo.safe;
        this.pollution = jobInfo.pollution;
        this.space = jobInfo.space;
        //Debug.Log(" DDDD : "+name+" : "+space.name );
        this.areaRatio = jobInfo.areaRatio;

        _reqSkill.text = jobInfo.reqSkill=="" ? "无" : jobInfo.reqSkill;
        notworkable = false;
        jobProgress = avgTime * reqPeople * 60;
        fullProgress = jobProgress;
        workers = new List<Worker>();
    }

    public void passivate()
    {
        passive = true;
    }

    float ptTime = 0f;

    void Update()
    {
        if (IfProcess && _startJob)
        {
            jobList = _logicController.allJobsList;

            passivate();
            notworkable = false;
            if (seq <= jobList[0].seq)
            {
                if (EvaluateReq())
                {
                    if (jobProgress > 0)
                    {
                        
                        foreach (var worker in workers)
                        {
                            worker._uiProgress.fillAmount = Mathf.Max(0, 1-((fullProgress - jobProgress) / fullProgress));
                        }
                        
                        
                        foreach (var worker in workers)
                        {
                            if (!worker.Quitable())
                            {
                                PauseJob();
                                this._ProText.text = "已暂停";
                                this.IfProcess = false;
                                passivate();
                                break;
                            }
                        }
                        if (notworkable)
                        {
                            notworkable = false;
                        }
                        float p = 0;
                        
                        foreach (var worker in workers)
                        {
                            p += (float)(Math.Sqrt(worker.efficiency) * (float)((float)_logicController._WorldTime._currentTime.TotalMinutes - (float)_preTime));
                        }
                        
                        Debug.Log("PKK "+ (p == 0));
                        
                        if (p != 0)
                        {
                            Debug.Log("PKK3 "+ p.ToString("f4"));
                        }
                        
                        /*ptTime += Time.deltaTime;
                        
                        if (ptTime >= p/reqPeople) 
                        {
                            jobProgress -= p;
                            ptTime = 0;
                        }*/

                        jobProgress -= p;
                        Debug.Log("PKK2 "+jobProgress);
                        Debug.Log("TTTTime "+_preTime);
                        _preTime = (float)_logicController._WorldTime._currentTime.TotalMinutes;
                    }
                    else
                    {
                        Debug.Log("Job "+jobProgress+" , "+name+" finished");
                        jobList.Remove(this);
                        _logicController._numText.text = jobList.Count.ToString();
                        _logicController.allJobsList.Remove(this);
                        _logicController._ringChart._completedNum++;
                        
                        Debug.Log("allJobsList Length : "+_logicController.allJobsList.Count);
                        PauseJob();
                        this._ProText.text = "已完成";
                        this.gameObject.SetActive(false);
                        this.IfProcess = false;
                        _logicController.finishedJobs.Add(this);
                    }
                }
                
            }
        }
    }



    public void StartJob(List<Worker> workers2)
    {
        Debug.Log("StartJob ! ! ============================ "+name);
        
        this.gameObject.SetActive(true);

        this._ProText.text = "进行中";

        this.mask.SetActive(false);
        
        passivate();
        this.workers = new List<Worker>(workers2);
        
        Random rand = new Random();
        
        float minValue = 3f;  // 最小值
        float maxValue = 5.5f; // 最大值
        
        foreach (var work in this.workers)
        {
            //work.StartWork(this);
            //work.gameObject.transform.parent = _workersBar.transform;
            
            work.currentJob = this;
            float randomValue = (float)(rand.NextDouble() * (maxValue - minValue) + minValue);

            if (this.space.name.Equals("areaA"))
            {
                AAreaMove(work,randomValue);
            }
            else if (this.space.name.Equals("areaB"))
            {
                BAreaMove(work,randomValue);
            }
            else if (this.space.name.Equals("areaC"))
            {
                CAreaMove(work,randomValue);
            }
            else if (this.space.name.Equals("areaD"))
            {
                DAreaMove(work,randomValue);
            }
            
        }

        this.space._workerNum += this.workers.Count;
        this.space._areaRate -= (int)this.areaRatio;
        
        _logicController.AddChartTime(_logicController._SpaceA._workerNum,_logicController._SpaceB._workerNum,
            _logicController._SpaceC._workerNum,_logicController._SpaceD._workerNum);
        Process();
    }
    
    
    public void AAreaMove(Worker worker,float randomValue)
    {
        //worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        
        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);

        Vector3 pos1 = new Vector3(-6.6f, 3.5f, 0);
        Vector3 pos2 = new Vector3(-6.6f, -0.2f, 0);
        Vector3 pos3 = new Vector3(-5.5f, -0.2f, 0);
        Vector3 pos4 = new Vector3(-5.5f, 2f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.left,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.left,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 3).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.right,pos3);
                worker._uiPerson.transform.rotation = qua3;
                
                worker.gameObject.transform.DOMove(pos3, 1).OnComplete((() =>
                {
                    //worker._uiPerson.transform.Rotate(new Vector3(180,0,0));
                    
                    Quaternion qua4 = Quaternion.FromToRotation(Vector3.down,pos4);
                    worker._uiPerson.transform.rotation = qua4;

                    worker.gameObject.transform.DOMove(pos4, 3).OnComplete((() =>
                    {
                        worker.gameObject.transform.parent = _logicController.aBar.transform;
                        
                        
                        worker.StartWork(this);
                        this._startJob = true;
                    }));
                }));
            }));
        }));
    }
    
    public void BAreaMove(Worker worker,float randomValue)
    {
        //Debug.Log(worker.gameObject.transform.parent.name);
        //Debug.Log(_logicController._restPtBat.transform.name);

        //worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        
        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);

        Vector3 pos1 = new Vector3(-6.6f, 3.5f, 0);
        Vector3 pos2 = new Vector3(-6.6f, -0.2f, 0);
        Vector3 pos3 = new Vector3(-5.5f, -0.2f, 0);
        Vector3 pos4 = new Vector3(-5.5f, -2.4f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.left,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.left,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 3).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.left,pos3);
                worker._uiPerson.transform.rotation = qua3;
                
                worker.gameObject.transform.DOMove(pos3, 1).OnComplete((() =>
                {
                    //worker._uiPerson.transform.Rotate(new Vector3(180,0,0));
                    
                    //Quaternion qua4 = Quaternion.FromToRotation(Vector3.down,pos4);
                    //worker._uiPerson.transform.rotation = qua4;

                    worker.gameObject.transform.DOMove(pos4, 3).OnComplete((() =>
                    {
                        worker.gameObject.transform.parent = _logicController.bBar.transform;
                        
                        
                        worker.StartWork(this);
                        this._startJob = true;
                    }));
                }));
            }));
        }));
    }
    
    public void CAreaMove(Worker worker,float randomValue)
    {
        //worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        //Debug.Log("RecTransform : "+worker.gameObject.transform.position.x);
        float duration1 = randomValue;
        //Debug.Log("Duration : "+duration1);

        Vector3 pos1 = new Vector3(2.1f, 3.5f, 0);
        Vector3 pos2 = new Vector3(2.1f, -0.2f, 0);
        Vector3 pos3 = new Vector3(0.9f, -0.2f, 0);
        Vector3 pos4 = new Vector3(0.9f, 2f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.right,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.right,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 3).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.right,pos3);
                worker._uiPerson.transform.rotation = qua3;
                worker.gameObject.transform.DOMove(pos3, 1).OnComplete((() =>
                {
                    //worker._uiPerson.transform.Rotate(new Vector3(180,0,0));
                    
                    Quaternion qua4 = Quaternion.FromToRotation(Vector3.down,pos4);
                    worker._uiPerson.transform.rotation = qua4;

                    worker.gameObject.transform.DOMove(pos4, 3).OnComplete((() =>
                    {
                        worker.gameObject.transform.parent = _logicController.cBar.transform;
                        
                        
                        worker.StartWork(this);
                        this._startJob = true;
                    }));
                }));
            }));
        }));
    }
    
    public void DAreaMove(Worker worker,float randomValue)
    {
        //worker.gameObject.transform.parent = _logicController._restPtBat.transform;

        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);

        Vector3 pos1 = new Vector3(2.1f, 3.5f, 0);
        Vector3 pos2 = new Vector3(2.1f, -0.2f, 0);
        Vector3 pos3 = new Vector3(0.9f, -0.2f, 0);
        Vector3 pos4 = new Vector3(0.9f, -2.4f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.right,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.right,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 3).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.right,pos3);
                worker._uiPerson.transform.rotation = qua3;
                worker.gameObject.transform.DOMove(pos3, 1).OnComplete((() =>
                {
                    //worker._uiPerson.transform.Rotate(new Vector3(180,0,0));
                    
                    Quaternion qua4 = Quaternion.FromToRotation(Vector3.down,pos4);
                    worker._uiPerson.transform.rotation = qua4;

                    worker.gameObject.transform.DOMove(pos4, 3).OnComplete((() =>
                    {
                        worker.gameObject.transform.parent = _logicController.dBar.transform;
                        
                        
                        worker.StartWork(this);
                        this._startJob = true;
                    }));
                }));
            }));
        }));
    }
    
    public void AAreaBack(Worker worker,float randomValue)
    {
        worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        
        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);
        
        Vector3 pos3 = new Vector3(-6.6f, 3.5f, 0);
        Vector3 pos2 = new Vector3(-6.6f, -0.2f, 0);
        Vector3 pos1 = new Vector3(-5.5f, -0.2f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.left,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.Linear).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.down,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 1).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.right,pos3);
                worker._uiPerson.transform.rotation = qua3;
                
                worker.gameObject.transform.DOMove(pos3, 3).OnComplete((() =>
                {
                    worker._uiPerson.transform.Rotate(0,0,0);
                    worker.gameObject.transform.parent = _logicController._restAreaBar.transform;
                    worker.PauseWork();
                }));
            }));
        }));
    }
    
    public void BAreaBack(Worker worker,float randomValue)
    {
        worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        
        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);
        
        Vector3 pos3 = new Vector3(-6.6f, 3.5f, 0);
        Vector3 pos2 = new Vector3(-6.6f, -0.2f, 0);
        Vector3 pos1 = new Vector3(-5.5f, -0.2f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.right,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.Linear).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.down,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 1).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.right,pos3);
                worker._uiPerson.transform.rotation = qua3;
                
                worker.gameObject.transform.DOMove(pos3, 3).OnComplete((() =>
                {
                    worker._uiPerson.transform.Rotate(0,0,0);
                    worker.gameObject.transform.parent = _logicController._restAreaBar.transform;
                    worker.PauseWork();
                }));
            }));
        }));
    }
    
    public void CAreaBack(Worker worker,float randomValue)
    {
        worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        
        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);
        
        Vector3 pos3 = new Vector3(2.1f, 3.5f, 0);
        Vector3 pos2 = new Vector3(2.1f, -0.2f, 0);
        Vector3 pos1 = new Vector3(0.9f, -0.2f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.right,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.Linear).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.down,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 1).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.left,pos3);
                worker._uiPerson.transform.rotation = qua3;
                
                worker.gameObject.transform.DOMove(pos3, 3).OnComplete((() =>
                {
                    worker._uiPerson.transform.Rotate(0,0,0);
                    worker.gameObject.transform.parent = _logicController._restAreaBar.transform;
                    worker.PauseWork();
                }));
            }));
        }));
    }
    
    public void DAreaBack(Worker worker,float randomValue)
    {
        worker.gameObject.transform.parent = _logicController._restPtBat.transform;
        
        float duration1 = randomValue;
        Debug.Log("Duration : "+duration1);
        
        Vector3 pos3 = new Vector3(2.1f, 3.5f, 0);
        Vector3 pos2 = new Vector3(2.1f, -0.2f, 0);
        Vector3 pos1 = new Vector3(0.9f, -0.2f, 0);
        
        Quaternion qua = Quaternion.FromToRotation(Vector3.left,pos1);
        worker._uiPerson.transform.rotation = qua;
        
        worker.gameObject.transform.DOMove(pos1, duration1).SetEase(Ease.Linear).OnComplete((() =>
        {
            
            Quaternion qua2 = Quaternion.FromToRotation(Vector3.left,pos2);
            worker._uiPerson.transform.rotation = qua2;
            
            worker.gameObject.transform.DOMove(pos2, 1).OnComplete((() =>
            {
                Quaternion qua3 = Quaternion.FromToRotation(Vector3.left,pos3);
                worker._uiPerson.transform.rotation = qua3;
                
                worker.gameObject.transform.DOMove(pos3, 3).OnComplete((() =>
                {
                    worker._uiPerson.transform.Rotate(0,0,0);
                    worker.gameObject.transform.parent = _logicController._restAreaBar.transform;
                    worker.PauseWork();
                }));
            }));
        }));
    }

    public void Process()
    {
        IfProcess = true;
    }
    

    public void PauseJob()
    {
        notworkable = true;
        this._startJob = false;
        
        Random rand = new Random();
        
        float minValue = 2.2f;  // 最小值
        float maxValue = 3.5f; // 最大值
        
        foreach (var worker in workers)
        {
            float randomValue = (float)(rand.NextDouble() * (maxValue - minValue) + minValue);
            //worker.PauseWork();
            if (this.space.name.Equals("areaA"))
            {
                AAreaBack(worker,randomValue);
                
            }
            else if (this.space.name.Equals("areaB"))
            {
                BAreaBack(worker,randomValue);
                
            }
            else if (this.space.name.Equals("areaC"))
            {
                CAreaBack(worker,randomValue);
                
            }
            else if (this.space.name.Equals("areaD"))
            {
                DAreaBack(worker,randomValue);
            } 
            
        }
        
        this.space._areaRate += (int)this.areaRatio;
        this.space._workerNum -= this.workers.Count;
        this.mask.SetActive(true);
        
        _logicController.AddChartTime(_logicController._SpaceA._workerNum,_logicController._SpaceB._workerNum,
            _logicController._SpaceC._workerNum,_logicController._SpaceD._workerNum);
        
        Debug.Log("this.space._areaRate += (int)this.areaRatio : "+this.areaRatio);
        passivate();
    }

    public bool EvaluateReq()
    {
        if (reqSkill == null)
        {
            return true;
        }
        else
        {
            string skillName = reqSkill.Keys.ToArray()[0];
            int skillNum = reqSkill[skillName];

            int skilledWorkers = 0;
            foreach (var worker in workers)
            {
                if (Array.Exists(worker.abilities,element => element == skillName))
                {
                    skilledWorkers += 1;
                }
            }
            if (skilledWorkers >= skillNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
}