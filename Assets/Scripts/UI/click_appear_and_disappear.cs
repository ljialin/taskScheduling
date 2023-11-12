using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_appear_and_disappear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void click()
    {
        this.gameObject.SetActive(!gameObject.activeSelf);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
