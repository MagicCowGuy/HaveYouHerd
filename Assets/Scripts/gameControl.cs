using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;
using System.Linq.Expressions;
//using TreeEditor;

public class gameControl : MonoBehaviour
{
	public Level curLevel;
	public int curLevelNum;

	public Slider progressSlider;
	
	public int sheepTotal;
	public int sheepFree;

	public Rect playArea;
	public Rect safeArea;
	public RectTransform targetArea;
	private Rect tarAreaRect = new Rect(0,0,0,0);
	public Rect tarRectWorld;

	public Vector2Int levSize;

	public float sheepInRatio = 0;
	private levelControl lCon;
	public List<GameObject> sheepObjects = new List<GameObject>();
	
	private void Awake(){
		
		lCon = this.GetComponent<levelControl>();
		
	}

	private void Start(){

		StartCoroutine("sheepCheck");
	}

	private void ClearField(){
		//Clear out sheep after resetting sheep list
		sheepObjects.Clear();
		DestroyWithTag("Sheep");
		DestroyWithTag("Fence");
	}

	private void DestroyWithTag(string tagtogo){
		GameObject[] objtogo = GameObject.FindGameObjectsWithTag(tagtogo);
		foreach(GameObject ob in objtogo)
    		Destroy(ob);
	}

	private void FixedUpdate() {
		progressSlider.value = Mathf.Lerp(progressSlider.value,sheepInRatio, 0.1f);
	}
	

	private IEnumerator sheepCheck(){
		while(true){
			if(sheepObjects.Count > 0){
				sheepFree = 0;
				for(int i=0; i< sheepObjects.Count; i++)
				{
					if(tarRectWorld.Contains(new Vector2(sheepObjects[i].transform.position.x,sheepObjects[i].transform.position.z))){
						sheepFree += 1;
					}
				}
				sheepInRatio = (float)sheepFree/(float)sheepObjects.Count;

				if(sheepInRatio >= 1){
					print("LEVEL COMPLETE!!!!");
					curLevelNum += 1;
					//LoadLevel(levelList[curLevelNum]); 
					//yield break;
				}
			}			
			yield return new WaitForSeconds(0.3f);
		}
	}

}
