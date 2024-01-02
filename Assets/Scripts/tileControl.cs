using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class tileControl : MonoBehaviour
{
	public List<GameObject> allTiles;
	public int tileOpt;
	public int tileMod;
	// Start is called before the first frame update
	void Awake()
	{
		foreach(Transform child in transform){
			allTiles.Add(child.gameObject);
		}
		//transform.position += new Vector3(0,Random.Range(-0.05f,0.05f),0);		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void cycleType(){
		foreach(Transform child in transform){
			child.gameObject.SetActive(false);
		}
		tileOpt ++;
		if(tileOpt > allTiles.Count - 1){
			tileOpt = 0;
		}
		allTiles[tileOpt].SetActive(true);
	}

	public void setType(int typeToSet){
		foreach(Transform child in transform){
			child.gameObject.SetActive(false);
		}
		tileOpt = typeToSet;
		allTiles[typeToSet].SetActive(true);
	}

	public void cycleMod(){
		tileMod ++;
		transform.rotation = Quaternion.Euler(0,90 * tileMod,0);
		if(tileMod == 4){
			tileMod = 0;
		}
	}

	public void setMod(int modToSet){
		tileMod = modToSet;
		transform.rotation = Quaternion.Euler(0,90 * tileMod,0);
	}
}
