using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class counterButtonsText : MonoBehaviour
{
    public int curValue;
    
    public void setValue(int setInt){
        curValue = setInt;
        curValue = Mathf.Clamp(curValue,0,100);
        this.GetComponent<TMP_Text>().text = curValue.ToString();
    }

    public void modValue(int modInt){
        curValue += modInt;
        curValue = Mathf.Clamp(curValue,0,100);
        this.GetComponent<TMP_Text>().text = curValue.ToString();
    }
}
