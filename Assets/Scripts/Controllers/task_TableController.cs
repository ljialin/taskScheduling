using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Controllers
{
    public class task_TableController : MonoBehaviour
    {

        public LogicController _LogicController;

        public Button _checkButton;

        public List<task_TableSlot> _tableSlots = new List<task_TableSlot>();

        public void Awake()
        {
        }

        // Dynamically generate table rows
       private void Start()
       {

       }

       public void ClickCheckButton()
       {
           for (int i = 0; i < _tableSlots.Count; i++)
           {
               _tableSlots[i]._current_time.text = (Math.Round(Math.Min(_LogicController.entireJobs[i].fullProgress/60 -
                                                    _LogicController.entireJobs[i].jobProgress / 60,
                   _LogicController.entireJobs[i].fullProgress/60),2)).ToString();
               
               _tableSlots[i]._needed_time.text = (_LogicController.entireJobs[i].fullProgress/60).ToString();
               
               if (_LogicController.finishedJobs.Contains(_LogicController.entireJobs[i]))
               {
                   _tableSlots[i]._is_complete.text = "是";
               }
               else
               {
                   _tableSlots[i]._is_complete.text = "否";
               }
           }
       }

       public void InitialTasks()
       {
           List<JobInfo> joblist = _LogicController.jobList;
            
           GameObject jobTmp = (GameObject)Resources.Load("taks_table_item");
           for (int i = 0; i < joblist.Count; i++)
           {
               GameObject prefab = Instantiate(jobTmp, gameObject.transform);
               prefab.gameObject.SetActive(false);
               task_TableSlot tableSlot = prefab.GetComponent<task_TableSlot>();
               _tableSlots.Add(tableSlot);

               tableSlot._id.text = (i + 1).ToString();

               tableSlot._name.text = joblist[i].name;

               tableSlot._abilities.text = joblist[i].reqSkill;
                prefab.gameObject.SetActive(true);
           }
       }
       
       
    }
}