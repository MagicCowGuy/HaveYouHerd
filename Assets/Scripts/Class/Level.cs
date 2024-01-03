using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
	public string levelName;
	public int levelCode;
	public string description;
	public String instructions;

	public int seed = 0;

	//public string spawnType;
	public int sheepCount;

	[Range(3, 15)]
	public int levelWidth;
	
	[Range(3, 15)]
	public int levelHeight;

	public enum areaType{Blank,Grass,Space,Water};
	public areaType areaTerrain;
	public enum spawnType{Random, Center, Left, Right, Top, Bottom};
	public spawnType spawnPostion;
	
	public int targetX;
	public int targetY;

	public int targetWidth;
	public int targetHeight;

	public Vector2Int playerSpawnPoint;

	public List<int> tileTypes;
	public List<int> tileMods;


#if UNITY_EDITOR
[ContextMenu("ForceSave")]
public void ForceSave()
{
    UnityEditor.EditorUtility.SetDirty(this);
    UnityEditor.AssetDatabase.SaveAssets();
}
#endif

}

