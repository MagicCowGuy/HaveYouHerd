using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class menuControl : MonoBehaviour
{

	public Level curLevel;
	private Level prepLevel;
	public TextMeshProUGUI tmDescript;
	public TextMeshProUGUI tmInstruct;
	public TextMeshProUGUI tmName;
	public GameObject startPan;

	public RectTransform gameStickUI;
	private levelControl lCon;
	public GameObject gridVisual;
	public GameObject editorPanels;
	public GameObject playerSpawnInd;
	public GameObject gridIndicator;
	//private GameObject playerObj;
	public Rigidbody playerRb;

	public Animation mainMenuAnim;

	public GameObject uiMainMenu;
	public GameObject uiChapPanelpf;
	public GameObject uiLevelIconpf;
	public GameObject uiWorldPan;
	public GameObject uiLevStartPan;
	public Vector3Int worldchaplev;
	public Vector3Int curWCL;

	public List<World> menuWorlds;


	[System.Serializable]
	public class World
	{
		public string worldName;
		public List<Chapter> chapterList;
	}
 
	[System.Serializable]
	public class Chapter
	{
		public string chapterName;
	    public List<Level> levelList;
	}

	private void Awake() {
		SetupLevelMenu();
		lCon = this.GetComponent<levelControl>();
	}
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
		if(Input.GetKeyDown("`")){
			switchMode();
		}
	}

	public void switchMode(){
		if(lCon.editorMode){
			gameStickUI.gameObject.SetActive(true);
			this.GetComponent<cameraControl>().overviewMode = false;
			lCon.editorMode = false;
			gridVisual.SetActive(false);
			editorPanels.SetActive(false);
			playerSpawnInd.SetActive(false);
			gridIndicator.SetActive(false);
			playerRb.isKinematic = false;
			playerRb.velocity = Vector3.zero;
		} else {
			gameStickUI.gameObject.SetActive(false);
			this.GetComponent<cameraControl>().overviewMode = true;
			lCon.editorMode = true;
			gridVisual.SetActive(true);
			editorPanels.SetActive(true);
			playerSpawnInd.SetActive(true);
			gridIndicator.SetActive(true);
			playerRb.velocity = Vector3.zero;
			playerRb.isKinematic = true;
		}
			
	}

	public void StartPanelShow(Level lvtoload){
		curLevel = lvtoload;
		startPan.SetActive(true);
		tmInstruct.text = curLevel.instructions;
		tmDescript.text = curLevel.description;
		tmName.text = curLevel.levelName;
	}

	public void SetupLevelMenu(){
		for (int w = 0; w < menuWorlds.Count; w++){
			for (int c = 0; c < menuWorlds[0].chapterList.Count; c++){
				GameObject newChapPan = Instantiate(uiChapPanelpf);
				newChapPan.transform.SetParent(uiWorldPan.transform);
				chapPanel curChapScript = newChapPan.GetComponent<chapPanel>();
				curChapScript.chapTitle.text = menuWorlds[0].chapterList[c].chapterName;
			
				for (int l = 0; l < menuWorlds[0].chapterList[c].levelList.Count; l++){
					worldchaplev = new Vector3Int(w,c,l);
					GameObject newLevPan = Instantiate(uiLevelIconpf);
					newLevPan.transform.SetParent(curChapScript.levPanCont);
					levPanel curLevScript = newLevPan.GetComponent<levPanel>();
					curLevScript.levText.text = (l + 1).ToString();
					curLevScript.mCon = this.GetComponent<menuControl>();
					curLevScript.worldchaplev = worldchaplev;
				}
			}
		}
	}

	public void StartLevelPrep(Vector3Int wclToLoad){
		curWCL = wclToLoad;
		uiLevStartPan.SetActive(true);
		prepLevel = menuWorlds[curWCL.x].chapterList[curWCL.y].levelList[curWCL.z];
		levStartPanel prepLevPan = uiLevStartPan.GetComponent<levStartPanel>();
		prepLevPan.levNumText.text = curWCL.z.ToString();
		prepLevPan.levTitleText.text = prepLevel.levelName;
	}

	public void LoadPrepedLevel(){
		if(prepLevel != null){
			lCon.voidLoadLevPub(prepLevel);
			uiMainMenu.SetActive(false);
		}

	}

	public void StartClose(){
		 //startPan.SetActive(false);
		 mainMenuAnim.Play("PanelTitle_Retract");
	}

	public void CloseLevStartPan(){
		uiLevStartPan.SetActive(false);
	}

}
