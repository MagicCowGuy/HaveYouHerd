using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textUpdater : MonoBehaviour
{
    public Slider targetSlider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void textUpdate(){
        this.GetComponent<TMP_Text>().text = targetSlider.value.ToString();
    }
}
