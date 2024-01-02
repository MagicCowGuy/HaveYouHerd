using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levPanel : MonoBehaviour
{
    public Level levSobject;
    public int levNumber;
    public TMP_Text levText;
    public Vector3Int worldchaplev;
    public menuControl mCon;

    public enum lStatus{Locked, Unlocked, Completed};
	public lStatus levelStatus;

    public void LevButtonPress(){
        if (worldchaplev != null){
            //Debug.Log("LOADING LEVEL: " + levSobject.levelName);
            mCon.StartLevelPrep(worldchaplev);
        } else {
            Debug.Log("NO LEVEL TO LOAD");
        }
    }
}
