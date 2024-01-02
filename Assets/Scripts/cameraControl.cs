using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
	public Camera mainCam;
	private Vector3 camoffset;
	public GameObject playerObj;
	private Rigidbody playerRb;
	private Vector3 tarPos;
	private float ZoomUser = 0;
	private Vector3 playerHorizVel;
	private float ZoomVel = 0;
	private float ZoomBase = 25;
	private float ZoomTarget = 18;

	private Vector3 playerClampPos;
	public Rect clampArea;
	public bool overviewMode = false;

	private Vector3 editCamOff = new Vector3(-10,12.5f,-10);
	private Vector3 editCamRot = new Vector3(40,45,0);

	// Start is called before the first frame update
	void Start()
	{
	camoffset = mainCam.transform.position;
	playerRb = playerObj.GetComponent<Rigidbody>();

	}

	private void FixedUpdate() {

	if(Screen.height > Screen.width){
		//portrait
		ZoomBase = 25;
	} else {
		ZoomBase = 10.65f;
	}

		if(overviewMode){
			mainCam.transform.position = editCamOff;
			mainCam.fieldOfView = 45;
			mainCam.transform.eulerAngles = editCamRot;
			mainCam.orthographicSize = 20f;
		} else {
			mainCam.orthographicSize = 12.5f;
			mainCam.transform.eulerAngles = new Vector3(40,45,0);
			playerHorizVel.Set(playerRb.velocity.x, 0, playerRb.velocity.z);
			ZoomVel = Mathf.Lerp(ZoomVel, playerHorizVel.magnitude * 0.85f, 0.05f);
			
			ZoomTarget = ZoomBase + ZoomUser + ZoomVel;
			mainCam.fieldOfView = ZoomTarget;

			playerClampPos = new Vector3(Mathf.Clamp(playerObj.transform.position.x, clampArea.xMin, clampArea.xMax),0,Mathf.Clamp(playerObj.transform.position.z, clampArea.yMin, clampArea.yMax));
			tarPos = playerClampPos + (playerHorizVel * 0.4f) + camoffset;
			mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, tarPos, Mathf.Clamp(playerHorizVel.normalized.magnitude, 0.25f, 1) * 0.08f);
	
	}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
