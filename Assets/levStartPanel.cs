using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levStartPanel : MonoBehaviour
{
    public TMP_Text levNumText;
    public TMP_Text levTitleText;

    public void prepLevel(Level levtoprep){
        levNumText.text = levtoprep.levelCode.ToString();
        levTitleText.text = levtoprep.levelName;
    }
}
