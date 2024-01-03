using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
	private Vector2 touchPosStart;
	private Vector2 touchPosCur;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.touchCount > 0)
		{
			print("Touching");
			if(touchPosStart == null){ }


		}
	}
}
