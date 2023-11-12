using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Controllers
{
    public class staff_TableController : MonoBehaviour
    {
        public List<staff_TableSlot> tableSlot = new List<staff_TableSlot>();

        public LogicController _LogicController;

        public Button checkButton;

        // Dynamically generate table rows
        private void Start()
        {
            List<WorkerInfo> workerlist = _LogicController._workerInfos;
            GameObject workerTmp = (GameObject)Resources.Load("item");
            List<GameObject> prefabs = new List<GameObject>(workerlist.Count);
            for (int i = 0; i < workerlist.Count; i++)
            {
                GameObject prefab = Instantiate(workerTmp, gameObject.transform);
                prefabs.Add(prefab);
                prefab.gameObject.SetActive(false);
                tableSlot.Add(prefab.GetComponent<staff_TableSlot>());

                tableSlot[i]._id.text = (i+1).ToString();

                tableSlot[i]._name.text = workerlist[i].name;
                
                tableSlot[i]._ability.text = workerlist[i].ability;
                prefab.gameObject.SetActive(true);

                //_tableSlots[i]._name.text = _LogicController._workerInfos[i].name;
            }


        }

        //private void Start()
        //{
        //    List<WorkerInfo> workerlist = _LogicController._workerInfos;
        //    for (int i = 0; i < tableSlot.Count; i++)
        //    {

        //        tableSlot[i]._id.text = i.ToString();

        //        tableSlot[i]._name.text = workerlist[i].name;

        //        tableSlot[i]._ability.text = workerlist[i].ability;

        //    }

        //}
        
        public void ClickCheckButton()
        {
            for (int i = 0; i < tableSlot.Count; i++)
            {
                
                tableSlot[i]._eff.text = (Math.Round(_LogicController._workersList[i].efficiency,2)).ToString();
                
                tableSlot[i]._time.text = (Math.Round(_LogicController._workersList[i]._workTime/60,2)).ToString();
            }
        }

        private void Update()
        {
             List<Worker> workers = _LogicController._workersList;
            for (int i = 0; i < tableSlot.Count; i++)
            {

                tableSlot[i]._task.text = workers[i].workerinfo.joint_jobs.ToString();
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.gameObject.GetComponent<RectTransform>());
        }
    }
}