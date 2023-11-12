using DefaultNamespace.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class finished_task_table_controller : MonoBehaviour
{
    public LogicController _LogicController;
    private List<string> jobs = new List<string>();
    GameObject jobTmp;
    // Dynamically generate table rows
    public int num;
    private void Start()
    {
        jobTmp = (GameObject)Resources.Load("finished_task_table_item");
    }
    void Update()
    {
        var finished = _LogicController.finishedJobs;
        num = finished.Count;
        if (jobs.Count < finished.Count)
        {
            GameObject prefab = Instantiate(jobTmp, gameObject.transform);
            prefab.GetComponent<Text>().text = finished[finished.Count - 1].name;
            jobs.Add(finished[finished.Count-1].name);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.gameObject.GetComponent<RectTransform>());
        }
    }
}
