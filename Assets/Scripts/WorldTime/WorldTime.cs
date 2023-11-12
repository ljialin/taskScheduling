using System;
using System.Collections;
using System.IO;
using UnityEngine;

    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged; 

       [SerializeField]
       private float _dayLength; //in seconds Unchanged

       public TimeSpan _currentTime;
       public TimeSpan _previousTime;

       //[SerializeField] private WorldTimeDisplay _worldTimeDisplay;


       private float _minuteLength => _dayLength / WorldTimeConstants.MinutesInDay;
       private string path;

       private void Awake()
       {
           //string test = _currentTime.ToString();
           //File.WriteAllText(path,test);
           path = Application.dataPath + "/Scripts/WorldTime/timeDate.txt";
           string tmp = File.ReadAllText(path);
           _currentTime = TimeSpan.Parse(tmp);
           _previousTime = _currentTime;

           if (_currentTime.Hours == 0)
           {
               Debug.Log(1111);
               //_worldTimeDisplay.shadowLine.transform.rotation = new Quaternion(0, 0, 0,0);
           }
       }

       private void Start()
       {
            StartCoroutine(AddMinute());
       }

       private IEnumerator AddMinute()
       {
           _currentTime += TimeSpan.FromMinutes(1);
           //Debug.Log(_currentTime.Days);
           WorldTimeChanged?.Invoke(this,_currentTime);
           yield return new WaitForSeconds(_minuteLength);
           StartCoroutine(AddMinute());

           if (_currentTime.Days > _previousTime.Days)
           {
               string test = _currentTime.ToString();
               File.WriteAllText(path,test);
               _previousTime = _currentTime;
           }
       }

    }
