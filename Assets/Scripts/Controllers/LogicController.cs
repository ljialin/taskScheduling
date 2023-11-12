


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;

namespace DefaultNamespace.Controllers
{
    public class LogicController : MonoBehaviour
    {
        public List<WorkerInfo> _workerInfos = new List<WorkerInfo>();
        public List<Worker> _workersList;

        public task_TableController _TableController;

        public List<JobInfo> jobList = new List<JobInfo>();
        public List<Job> allJobsList = new List<Job>();
        public List<Job> finishedJobs = new List<Job>();

        public List<Job> entireJobs = new List<Job>();


        private List<Job> currentJobs = new List<Job>();
        private List<Worker> workers = new List<Worker>();

        public GameObject jobTemp;
        public GameObject jobsBar;

        public GameObject _restAreaBar;
        
        public GameObject _restPtBat;


        public Params Params;

        public GameObject aBar;
        public GameObject bBar;
        public GameObject cBar;
        public GameObject dBar;

        private bool IfProcess = false;

        public Text _numText;

        public Ring_Chart _ringChart;

        public WorldTime _WorldTime;

        public Line_chart _LineChart;

        public Space _SpaceA;

        public Space _SpaceB;

        public Space _SpaceC;

        public Space _SpaceD;

        private void Awake()
        {
            _ringChart._allNum = jobList.Count;
        }

        private void Start()
        {
            FinishedJobInstant();
            _TableController.InitialTasks();
            Debug.Log(allJobsList.Count);
            
            _numText.text = allJobsList.Count.ToString();
            _ringChart._completedNum = 0;
            
            Process();
        }

        public void ClearData()
        {
            currentJobs.Clear();
            workers.Clear();
            _workerInfos.Clear();
            //_workersList.Clear();
            jobList.Clear();
            allJobsList.Clear();
            //finishedJobs
            entireJobs.Clear();


            IfProcess = false;
    }


        private void Update()
        {
            float p = 0;
            bool workable = true;
            
            if (allJobsList.Count > 0)
            {
                p = allJobsList[0].seq;
                
                int currentIndex = 0;
                currentJobs.Clear();

                while (currentIndex < allJobsList.Count)
                {
                    Job tmpJob = allJobsList[currentIndex];
                    currentIndex++;
                    
                    if (tmpJob.seq > p || tmpJob.IfProcess)
                    {
                       continue;
                    }
                    currentJobs.Add(tmpJob);

                    if (tmpJob.passive)
                    {
                        workers.Clear();
                        int skillNum = 0;
                        if (tmpJob.reqSkill != null)
                        {
                            string skillName = tmpJob.reqSkill.Keys.ToArray()[0];
                            skillNum = tmpJob.reqSkill[skillName];

                            for (int i = 0; i < skillNum; i++)
                            {
                                workers.Add(FindBestWorker(skillName));
                            }
                        }
                        for (int i = 0; i < tmpJob.reqPeople - skillNum; i++)
                        {
                            workers.Add(FindBestWorker());
                        }

                        workable = false;
                        
                        foreach (Worker worker in workers)
                        {
                            if (worker == null || !worker.Workable(tmpJob))
                            {
                                workable = true;
                                break;
                            }
                        }
                        if (workable)
                        {
                            break;
                        }
                        tmpJob.StartJob(workers);
                    }
                }
            }
            if (jobList.Count == 0)
            {
                Debug.Log("FINISHED ! !");
            }
        }

        public void Process()
        {
            IfProcess = true;
        }


        public void AddChartTime(int num1,int num2,int num3,int num4)
        {
            Debug.Log("_WorldTime._currentTime : "+_WorldTime._currentTime);
            _LineChart.AddFullData(_WorldTime._currentTime.ToString(),num1,num2,num3,num4);
        }


        private Worker FindBestWorker(params string[] args)
        {
            double maxEff = 0;
            Worker bestWorker = null;
            if (args.Length == 0)
            {
                foreach (Worker worker in _workersList)
                {
                    if (worker.IsFree() && !workers.Contains(worker))
                    {
                        double eff = worker.efficiency;
                        if (eff > maxEff)
                        {
                            maxEff = eff;
                            bestWorker = worker;
                        }
                    }
                }

                if (bestWorker != null)
                {
                    bestWorker.Process();
                }
                
                return bestWorker;
            }
            else
            {
                foreach (Worker worker in _workersList)
                {
                    if (worker.IsFree() && !workers.Contains(worker) 
                                        && worker.abilities.Contains(args[0]))
                    {
                        double eff = worker.efficiency;
                        if (eff > maxEff)
                        {
                            maxEff = eff;
                            bestWorker = worker;
                        }
                    }
                }
                if (bestWorker != null)
                {
                    bestWorker.Process();
                }
                
                return bestWorker;
            }
        }
        
        public void FinishedJobInstant()
        {
            jobTemp =  (GameObject)Resources.Load("Job");
            
            for (int i = 0; i < jobList.Count; i++)
            {
                if (jobList == null || jobList.Count == 0)
                {
                    return;
                }
                GameObject go = Instantiate(jobTemp, jobsBar.transform);
                
                go.SetActive(true);
                
                Job jobSlot = go.GetComponent<Job>();
                if (jobSlot == null) 
                {
                    continue;
                }
                jobSlot.SetData(jobList[i]);
                
                jobSlot._logicController = this;
                allJobsList.Add(jobSlot);
                entireJobs.Add(jobSlot);
            }
        }

        public bool GetCSVWorkerData(string name)
        {
            //TextAsset jobData = Resources.Load<TextAsset>(name);
            
            string[] data = File.ReadAllLines(name);
            //if (jobData == null) 
            //{
            //    Debug.Log("文件不存在");
            //    return false;
            //}
            //string[] data = jobData.text.Split(new char[] { '\n' });
            
            
            for (int i = 1; i < data.Length; i++)
            {
                WorkerInfo info = new WorkerInfo();
                string[] oneRow = data[i].Split(new char[] { ',' }); 
                for (int j = 0; j < oneRow.Length; j++)
                {
                    switch (j)
                    {
                        case 0:
                            info.name = oneRow[j];
                            break;
                        case 1:
                            info.ability = oneRow[j].ToString();
                            break;
                        case 2:
                            info.time = int.Parse(oneRow[j].ToString());
                            break;
                    }
                }
                _workerInfos.Add(info);
            }

            for (int i = 0; i < _workerInfos.Count; i++)
            {
                Debug.Log(i + "?");
                _workersList[i].name = _workerInfos[i].name;
                _workersList[i].abilities = _workerInfos[i].ability.Split('，');;
                _workersList[i].timeLimit = _workerInfos[i].time * 3600*4;
                _workersList[i].time = 0;
                _workersList[i].efficiency = 1.0;
            }

            return true;
        }
        
        
        public bool GetCSVJobData(string name)
        {
            //TextAsset jobData = Resources.Load<TextAsset>(name);

            string[] data = File.ReadAllLines(name);



            if (data == null) 
            {
                Debug.Log("文件不存在");
                return false;
            }
/*            else
            {
                Debug.Log(data.Length);
                return true;
            }*/
            int id = 0;
            //string[] data = jobData.text.Split(new char[] { '\n' });
            
            for (int i = 1; i < data.Length; i++)
            {
                JobInfo info = new JobInfo();
                string[] oneRow = data[i].Split(new char[] { ',' }); 
                for (int j = 0; j < oneRow.Length; j++)
                {
                    switch (j)
                    {
                        case 2:
                            info.name = oneRow[j];
                            break;
                        case 3:
                            info.reqPeople = int.Parse(oneRow[j].ToString());
                            break;
                        case 4:
                            info.reqSkill = oneRow[j].ToString();
                            break;
                        case 5:
                            info.avgTime = float.Parse(oneRow[j]);
                            break;
                        case 7:
                            info.seq = float.Parse(oneRow[j]);
                            break;
                        case 8:
                            info.safe = int.Parse(oneRow[j]);
                            break;
                        case 9:
                            info.pollution = int.Parse(oneRow[j]);
                            break;
                        case 10:
                            oneRow[j] = oneRow[j].Replace("%","");
                            info.areaRatio = int.Parse(oneRow[j]);
                            break;
                        case 11:
                            if (oneRow[j]=="A")
                            {
                                info.space = Params.SpaceA;
                            }
                            else if (oneRow[j] == "B")
                            {
                                info.space = Params.SpaceB;

                            }
                            else if (oneRow[j] == "C")
                            {
                                info.space = Params.SpaceC;

                            }
                            else if (oneRow[j] == "D")
                            {
                                info.space = Params.SpaceD;

                            }
                            
                            break;
                    }
                }
                jobList.Add(info);
            }
            jobList.Sort((job1, job2) => job1.seq.CompareTo(job2.seq));
            return true;
            
        }
        
    }
}