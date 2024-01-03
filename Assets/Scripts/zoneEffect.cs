using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoneEffect : MonoBehaviour
{
    public RectTransform panelrect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        panelrect.offsetMin = new Vector2(-2 - Mathf.Sin (Time.fixedTime * Mathf.PI * 0.15f) * 2f,0);
    }
}
