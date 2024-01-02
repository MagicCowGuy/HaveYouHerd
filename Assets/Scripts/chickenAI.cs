using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class chickenAI : MonoBehaviour
{
	public GameObject playerObj;
	[SerializeField]
	private NavMeshAgent myNMA;

	public bool alerted = false;
	private float alertcool;

	private Vector3 distanceToPlayer;
	//private float personalSpaceRadius = 4;
	//private float runDistance = 5;
	private float driftRange = 1.5f;
	private float idleTicker = 20;

	public Renderer chickenRenderer;
	public Renderer sheepRen;

	public float randsomSeed;

	// Start is called before the first frame update
	void Start()
	{
		
		playerObj = GameObject.Find("Player");
		myNMA = this.GetComponent<NavMeshAgent>();
		transform.localScale = new Vector3(randsomSeed,randsomSeed,randsomSeed);
		
	}

	private void Awake() {
		playerObj = GameObject.Find("Player");
		randsomSeed = Random.Range(0.95f,1.05f);
		StartCoroutine(statusCheck());
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void PingAlert(){
		alerted = true;
		alertcool = 3;
		idleTicker = Random.Range(20,50);
		Debug.DrawRay(this.transform.position, Vector3.up, Color.green, 5.0f);
	}

	IEnumerator statusCheck() {
	
		while(true){
			if(alertcool <= 0){
				alerted = false;
			} else {
				alertcool -= 1;
			}

			if(!alerted && myNMA.velocity.magnitude < 0.1f){
				idleTicker -= 1;
			}

			if(idleTicker <= 0){
				myNMA.SetDestination(this.transform.position + new Vector3(Random.Range(-driftRange,driftRange),0,Random.Range(-driftRange,driftRange)));
				Debug.DrawRay(myNMA.destination, Vector3.up, Color.yellow, 5.0f);
				
				idleTicker = Random.Range(20,50);
			}
		yield return new WaitForSeconds(0.1f);
		}
	
		//	if(idleTicker <= 0){
		//			myNMA.SetDestination(this.transform.position + new Vector3(Random.Range(-driftRange,driftRange),0,Random.Range(-driftRange,driftRange)));
		//			idleTicker = 20;
		//	}
		
		//yield return;
		}
}
