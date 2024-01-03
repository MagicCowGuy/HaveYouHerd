using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
//using UnityEngine.WSA;
using System.IO;
using UnityEngine.AI;
//using System.Numerics;

public class levelControl : MonoBehaviour
{
	public TMP_Dropdown dropdownChapters;
	public TMP_Dropdown dropdownLevels;

	public List<Level> chapters;
	public List<string> chapNames;
	public List<Level> levels;
	public List<string> levelNames;

	public GameObject playerObj;
	public GameObject SheepPrefab;
	public GameObject FencePrefab;
	public GameObject TilePrefab;
	public GameObject navMeshPlane;
	public NavMeshSurface nms;
	
	public int sheepTotal;
	private Vector3 sheepSpawnPos;
	public counterButtonsText sheepWhiteUI;
	public List<GameObject> sheepObjects = new List<GameObject>();

	public Grid grid;
	public GameObject gridPlane;
	public GameObject backupPlane;

	public Rect playArea;
	public Rect safeArea;
	public RectTransform targetArea;
	public Rect tarAreaRect = new Rect(0,0,0,0);
	public Rect tarRectWorld;
	private Vector2 tarPos;
	private Vector2 tarSize;
	public Vector2Int levSize;
	private float cambuffer = 5f;
	private float unitSize = 4f;
	
	public List<int> tempLevTiles;
	public List<int> tempLevMods;
	public List<tileControl> levelTilesCon;
	private Vector2Int tempPlayerPos;
	
	public bool editorMode = false;
	private int editSetting = 0;
	public GameObject editorPosMarker;
	public GameObject editorCellMarker;
	public GameObject playerSpawnMarker;

	public Slider sliderx;
	public Slider slidery;

	public Level currentLevel;
	private Vector3Int gridPos;
	private bool mouseOverLevel = false;
	
	private Vector3 tarStart;
	private Vector3 tarEnd;

	private int curSeed;
	public TMP_Text seedText;

	public Level testLevel;
	public Object[] levelSOs;

	private void Awake(){
		//levelSOs = Resources.LoadAll("Levels/", typeof(Level));
		playerObj = GameObject.FindWithTag("Player");
	}

	// Start is called before the first frame update
	void Start()
	{   
		if(Application.isEditor){
			ChapListUpdate();
			LevListUpdate();
		}
		//LoadLevel(testLevel);
		
	}

	private void ChapListUpdate(){
		dropdownChapters.ClearOptions();
		foreach (var d in Directory.GetDirectories("Assets/Resources/Levels"))
		{
			var dir = new DirectoryInfo(d);
			var dirName = dir.Name;
			chapNames.Add(dirName);
		}
		dropdownChapters.AddOptions(chapNames);
	}

	public void LevListUpdate(){
		dropdownLevels.ClearOptions();
		levels = Resources.LoadAll<Level>("Levels/" + dropdownChapters.options[dropdownChapters.value].text).ToList();
		print(levels[0].levelName);
		for (int a = 0; a < levels.Count; a++){
			levelNames.Add(levels[a].levelName.ToString());
			dropdownLevels.options.Add(new TMP_Dropdown.OptionData(levels[a].levelName));
		}		
	}

	private void ClearField(){
		//Clear out sheep after resetting sheep list
		sheepObjects.Clear();
		DestroyWithTag("Sheep");
		DestroyWithTag("Fence");
		DestroyWithTag("Tile");
		levelTilesCon.Clear();
	}

	private void DestroyWithTag(string tagtogo){
		GameObject[] objtogo = GameObject.FindGameObjectsWithTag(tagtogo);
		foreach(GameObject ob in objtogo)
    		Destroy(ob);
	}

	public void LoadButton(){
		LoadLevel(levels[dropdownLevels.value]);
	}

	public void voidLoadLevPub(Level ltol){
		LoadLevel(ltol);
	}

	private void SetSeed(int newSeed){
		curSeed = newSeed;
		seedText.text = curSeed.ToString();
		Random.InitState(curSeed);
	}

	public void GenerateSeed(){
		Random.InitState((int)Time.frameCount);
		currentLevel.seed = Random.Range(1,99999);
		SetSeed(currentLevel.seed);
		saveLevel();
	}

	private void LoadLevel(Level levLoad)
	{
		if(levLoad.seed == 0){
			levLoad.seed = Random.Range(1,99999);
		}
		SetSeed(levLoad.seed);
		

		print("Loading level: " + levLoad.levelName);
		ClearField();
		currentLevel = levLoad;
		levSize = new Vector2Int(levLoad.levelWidth,levLoad.levelHeight);
		
		playArea = new Rect(-(float)levSize.x * unitSize * 0.5f, -(float)levSize.y * unitSize * 0.5f, levSize.x * unitSize, levSize.y * unitSize);
		float buffer = 1.75f;
		safeArea = new Rect(playArea.xMin + buffer, playArea.yMin + buffer, playArea.width - buffer * 2, playArea.height - buffer * 2);
		this.GetComponent<cameraControl>().clampArea = new Rect(playArea.xMin + cambuffer, playArea.yMin + cambuffer, playArea.width - cambuffer * 2, playArea.height - cambuffer * 2);
		float levelFront = (-unitSize/2) * (float)levSize.y;
		float levelLeft = (-unitSize/2) * (float)levSize.x;
		Vector2 levelCorner = new Vector2(levelFront, levelLeft);

		grid.transform.position = new Vector3(levelLeft, 0, levelFront);
		gridPlane.transform.localScale = new Vector3(levSize.x * 0.4f, levSize.y * 0.4f, levSize.y * 0.4f);
		backupPlane.transform.localScale = new Vector3(levSize.x * 0.4f, levSize.y * 0.4f, levSize.y * 0.4f);
		
		
		//target area layout
		tarSize = new Vector2(levLoad.targetWidth,levLoad.targetHeight);
		tarPos = new Vector2(levLoad.targetX,levLoad.targetY);
		tarAreaRect = new Rect(tarPos.x,tarPos.y,tarSize.x,tarSize.y);
		tarRectWorld = tarAreaRect;
		tarRectWorld.position = tarPos - new Vector2(tarAreaRect.width/2,tarAreaRect.height/2);
		

		targetArea.sizeDelta = new Vector2(tarAreaRect.width,tarAreaRect.height);
		targetArea.position = new Vector3(tarAreaRect.x,0.1f,tarAreaRect.y);
		
		sliderx.value = levLoad.levelWidth;
		slidery.value = levLoad.levelHeight;
		//spawn fences
		for (int i=0; i< levSize.x; i++)
		{
			GameObject frontFence = Instantiate(FencePrefab, new Vector3(playArea.x + (unitSize * i), 0, playArea.y), Quaternion.AngleAxis(180, Vector3.up));
			GameObject backFence = Instantiate(FencePrefab, new Vector3(playArea.max.x - (unitSize * i), 0, playArea.max.y), Quaternion.AngleAxis(0, Vector3.up));
		}
		
		for (int i=0; i< levSize.y; i++)
		{
			GameObject leftFence = Instantiate(FencePrefab, new Vector3(playArea.x, 0, playArea.max.y - (unitSize * i)), Quaternion.AngleAxis(-90, Vector3.up));
			GameObject rightFence = Instantiate(FencePrefab, new Vector3(playArea.max.x, 0, playArea.y + (unitSize * i)), Quaternion.AngleAxis(90, Vector3.up));
			for (int b=0; b< levSize.x; b++){
				GameObject tileBase = Instantiate(TilePrefab, grid.GetCellCenterWorld(new Vector3Int(b,0,i)), Quaternion.AngleAxis(0, Vector3.up));
				levelTilesCon.Add(tileBase.GetComponent<tileControl>());
			}
		}

		//for (int i=0; i< levLoad.tileTypes.Count; i++)
		for (int i=0; i< levLoad.levelWidth * levLoad.levelHeight; i++)
		{
			tempLevTiles.Add(0);
			tempLevMods.Add(0);
			if(currentLevel.tileTypes.Count > i){
				levelTilesCon[i].setType(levLoad.tileTypes[i]);
			}
			if(currentLevel.tileMods.Count > i){
				levelTilesCon[i].setMod(levLoad.tileMods[i]);
			}
		}

		navMeshPlane.transform.localScale = new Vector3(playArea.x * 0.2f,-0.01f,playArea.y * 0.2f);
		
		tempPlayerPos = levLoad.playerSpawnPoint;
		playerObj.transform.position = new Vector3(tempPlayerPos.x, 2, tempPlayerPos.y);

		//Spawn Sheep
		for (int i=0; i< levLoad.sheepCount; i++)
		{
			do{
				sheepSpawnPos = new Vector3(Random.Range(playArea.x, playArea.max.x), 0, Random.Range(playArea.y, playArea.max.y));
			} while(tarRectWorld.Contains(new Vector2(sheepSpawnPos.x, sheepSpawnPos.z)));
			GameObject spawnSheep = Instantiate(SheepPrefab, sheepSpawnPos, transform.rotation);
			
			spawnSheep.transform.rotation = Quaternion.Euler(new Vector3(0,Random.Range(0,360),0));
			sheepObjects.Add(spawnSheep);
		}
		sheepTotal = levLoad.sheepCount;
		this.GetComponent<gameControl>().sheepTotal = sheepTotal;
		this.GetComponent<gameControl>().sheepObjects = sheepObjects;
		this.GetComponent<gameControl>().tarRectWorld = tarRectWorld;
		sheepWhiteUI.setValue(levLoad.sheepCount);
		this.GetComponent<playerMovement>().allSheep = sheepObjects;
		
		NavMesh.RemoveAllNavMeshData();
		nms.BuildNavMesh();
		StartCoroutine("rebuildNM");
	}
	
	public void saveLevel(){
		currentLevel.levelHeight = (int)slidery.value;
		currentLevel.levelWidth = (int)sliderx.value;

		currentLevel.playerSpawnPoint = tempPlayerPos;

		int tileCount = levSize.x * levSize.y;
		tempLevTiles.Clear();
		currentLevel.tileTypes.Clear();
		currentLevel.tileMods.Clear();
		for (int i=0; i< tileCount; i++)
		{
			tempLevTiles.Add(levelTilesCon[i].tileOpt);
			tempLevMods.Add(levelTilesCon[i].tileMod);
			currentLevel.tileTypes.Add(levelTilesCon[i].tileOpt);
			currentLevel.tileMods.Add(levelTilesCon[i].tileMod);
		}

		currentLevel.sheepCount = sheepWhiteUI.curValue;

		currentLevel.targetX = (int)tarPos.x;
		currentLevel.targetY = (int)tarPos.y;
		currentLevel.targetWidth = (int)tarSize.x;
		currentLevel.targetHeight = (int)tarSize.y;

		#if UNITY_EDITOR
			currentLevel.ForceSave();
		#endif
		LoadLevel(currentLevel);
	}
	
	public void setTile(int tilePos, int tileCode){
		//tempLevTiles[tilePos] = tileCode;
	}

	public void setEditSetting(int newSetting){
		editSetting = newSetting;
	}

	private void layTargetArea(){
		
		tarSize = new Vector2(Mathf.Abs(tarStart.x - tarEnd.x) + 4,Mathf.Abs(tarStart.z - tarEnd.z) + 4);
		tarPos = new Vector2((tarStart.x + tarEnd.x)/2, (tarStart.z + tarEnd.z)/2);
		tarAreaRect = new Rect(tarPos.x,tarPos.y,tarSize.x,tarSize.y);
		tarRectWorld = tarAreaRect;
		tarRectWorld.position = tarPos - new Vector2(tarAreaRect.width/2,tarAreaRect.height/2);

		targetArea.sizeDelta = new Vector2(tarAreaRect.width,tarAreaRect.height);
		targetArea.position = new Vector3(tarAreaRect.x,0.1f,tarAreaRect.y);
	}

	IEnumerator rebuildNM(){

		yield return 0;
		nms.BuildNavMesh();

	}

	private void Update(){
		//EditorGUI.DrawRect(tarAreaRect, Color.red);
		//Debug.DrawLine(new Vector3(tarRectWorld.min.x, 0, tarRectWorld.min.y), new Vector3(tarRectWorld.max.x, 0, tarRectWorld.max.y), Color.red);

		if(editorMode){
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, 200f, 1 << 8)){
				editorCellMarker.SetActive(true);
				gridPos = grid.WorldToCell(hit.point);
				editorCellMarker.transform.position = grid.GetCellCenterWorld(gridPos);
				mouseOverLevel = true;
			} else {
				
				editorCellMarker.SetActive(false);
				mouseOverLevel = false;
			}
			if(mouseOverLevel){
				//Left click
				if(Input.GetMouseButtonDown(0)){
					switch (editSetting) {
						case 0:
						//Cycle Terrain
						//editorPosMarker.transform.position = grid.GetCellCenterWorld(gridPos);
					
						print("Grid Pos. " + gridPos + " || Tile no. " + (gridPos.x + (gridPos.z * levSize.x)));
						setTile(gridPos.x + (gridPos.z * levSize.x), 10);
						levelTilesCon[gridPos.x + (gridPos.z * levSize.x)].cycleType();
						nms.BuildNavMesh();
						break;

						case 1:
						//Player Spawn Pos

						playerSpawnMarker.transform.position = grid.GetCellCenterWorld(gridPos);
						tempPlayerPos = new Vector2Int((int)grid.GetCellCenterWorld(gridPos).x,(int)grid.GetCellCenterWorld(gridPos).z);
						playerObj.transform.position = new Vector3(tempPlayerPos.x, 2, tempPlayerPos.y);
						break;

						case 2:
						//Target Area
						tarStart = grid.GetCellCenterWorld(gridPos);
						//tarEnd = grid.GetCellCenterWorld(gridPos);
						layTargetArea();
						break;
					}
				}
				if(Input.GetMouseButton(0)){
					switch (editSetting) {
						case 2:
						tarEnd = grid.GetCellCenterWorld(gridPos);
						layTargetArea();
						break;
					}
				}
				//Right click
				if(Input.GetMouseButtonDown(1)){
					switch(editSetting){
						case 0:
						//Cycle Terrain
						setTile(gridPos.x + (gridPos.z * levSize.x), 10);
						levelTilesCon[gridPos.x + (gridPos.z * levSize.x)].cycleMod();
						nms.BuildNavMesh();
						break;

						case 2:
						
						break;
					}
				}
			}
		}
	}
}

		