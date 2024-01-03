using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sheep_White1 : MonoBehaviour
{
	public levelControl lCon;
	private GameObject playerObj;
	private Vector3 advPlayerPos;
	private NavMeshAgent myNma;
	private List<GameObject> otherSheep = new List<GameObject>();

	public bool alerted;
	
	void Awake()
	{
		lCon = GameObject.FindWithTag("GameController").GetComponent<levelControl>();
		myNma = this.GetComponent<NavMeshAgent>();
		playerObj = GameObject.FindWithTag("Player");
		StartCoroutine("proxCheck");

		float randsomSeed = Random.Range(0.90f,1.10f);
		transform.localScale = new Vector3(randsomSeed,randsomSeed,randsomSeed);

		myNma.speed = Random.Range(5,8);
		myNma.acceleration = Random.Range(20,40);
	}

	// Update is called once per frame
	void Update()
	{
		if(myNma.remainingDistance < 2){
			alerted = false;
			myNma.ResetPath();
		}
	}

	private IEnumerator proxCheck(){
		while(true){
			Vector3 distToPlayer = transform.position - playerObj.transform.position;
			if(distToPlayer.magnitude < 4.5f && !alerted){
				runaway();
			}
			myNma.speed = Mathf.Clamp(10 - distToPlayer.magnitude * 0.5f, 3, 10);
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void runaway(){
		alerted = true;
		advPlayerPos = playerObj.transform.position + playerObj.GetComponent<Rigidbody>().velocity.normalized;
		Vector3 distToPlayer = transform.position - advPlayerPos;
		Vector3 targetPos = transform.position + distToPlayer.normalized * 5;

		if(lCon.playArea.Contains(new Vector2(targetPos.x,targetPos.z))){
			myNma.SetDestination(targetPos);
		} else {
			myNma.SetDestination(farpoint());
		}
		shareRun();
	}

	private void shareRun(){
		//otherSheep.Clear();
		otherSheep = lCon.sheepObjects;
		for(int i=0; i< otherSheep.Count; i++)
		{
			if(otherSheep[i].GetComponent<sheep_White>() != null){
				Vector3 distanceToSheep = otherSheep[i].transform.position - transform.position;
				if(distanceToSheep.magnitude < 4){
					Vector3 randomOffset = new Vector3(Random.Range(-1,1),0,Random.Range(-1,1));
					otherSheep[i].GetComponent<sheep_White>().remoteScare(myNma.destination + distanceToSheep + randomOffset);
				}
			}
		}
	}

	public void remoteScare(Vector3 remoteDest){
		if(!alerted){
			myNma.SetDestination(remoteDest);
			//alerted = true;
		}
	}

	private Vector3 farpoint(){
		Vector3 furthest = this.transform.position;
		int angleCount = 8;

		for (int a = 1; a < angleCount + 1; a++)
		{ 
			Vector3 dir = Vector3.forward;
			dir = Quaternion.Euler(0,(360/angleCount) * a + (180/angleCount),0) * dir;
			Vector3 testPoint = (dir.normalized * 6) + transform.position;

			if(lCon.safeArea.Contains(new Vector2(testPoint.x,testPoint.z))){
				if(Vector3.Distance(advPlayerPos, testPoint) > Vector3.Distance(advPlayerPos, furthest)){
					furthest = testPoint;
				}
			}

		}
		return furthest;
	}

	private void setRunInf(){

	}
}
