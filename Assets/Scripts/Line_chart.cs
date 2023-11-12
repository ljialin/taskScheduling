using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class Line_chart : MonoBehaviour
{
    private LineChart chart;

    public Text _aNum;
    public Text _bNum;
    public Text _cNum;
    public Text _dNum;

    // Start is called before the first frame update
    void Start()
    {
        chart = gameObject.GetComponent < LineChart>();
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();
        chart.RemoveData();
        chart.AddSerie<Line>("区域A");
        chart.AddSerie<Line>("区域B");
        chart.AddSerie<Line>("区域C");
        chart.AddSerie<Line>("区域D");

        var room1 = chart.GetSerie<Line>(0);
        var room2 = chart.GetSerie<Line>(1);
        var room3 = chart.GetSerie<Line>(2);
        var room4 = chart.GetSerie<Line>(3);
        
    }


    public void AddFullData(string x_name, double value1, double value2, double value3, double value4)
    {
        
        List<string> preName = chart.EnsureChartComponent<XAxis>().data;

        if (preName.Count > 0)
        {
            if (x_name.Equals(preName[preName.Count - 1]))
            {
                Debug.Log("VVVV QAQ");
                chart.UpdateData("区域A", preName.Count - 1, value1);
                chart.UpdateData("区域B", preName.Count - 1, value2);
                chart.UpdateData("区域C", preName.Count - 1, value3);
                chart.UpdateData("区域D", preName.Count - 1, value4);
            }
            else
            {
                chart.AddXAxisData(x_name);
                chart.AddData(0, value1);
                chart.AddData(1, value2);
                chart.AddData(2, value3);
                chart.AddData(3, value4);
            }
        }
        else
        {
            chart.AddXAxisData(x_name);
            chart.AddData(0, value1);
            chart.AddData(1, value2);
            chart.AddData(2, value3);
            chart.AddData(3, value4);
        }
        Debug.Log("VVVV "+value1);
        _aNum.text = value1.ToString();
        _bNum.text = value2.ToString();
        _cNum.text = value3.ToString();
        _dNum.text = value4.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
