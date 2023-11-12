using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using XCharts.Runtime;

public class Ring_Chart : MonoBehaviour
{
    private RingChart ringChart;

    public int _allNum;

    public int _completedNum;

    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        ringChart = gameObject.GetComponent<RingChart>();
        ringChart.UpdateData(0, 0, 1, _allNum);
        text = transform.Find("ratio").GetComponent<Text>();
        text.text = "00/00";
    }

    // 1st for serie index, 2nd for data index, 3rd for dimension, 4th for value.
    // Update is called once per frame
    void Update()
    {
        ringChart.UpdateData(0, 0, 0, _completedNum);
        text.text = _completedNum + "/" + _allNum;
    }
}
