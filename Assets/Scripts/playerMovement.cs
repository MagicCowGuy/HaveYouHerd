using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEditor;
using Unity.Mathematics;

public class playerMovement : MonoBehaviour
{
	[SerializeField]
	private GameObject playerObj;
	
	[SerializeField]
	private RectTransform gameStickUI;

	public Animator playerAnim;

	private Rigidbody playerRb;
	public List<GameObject> allSheep;
	public List<GameObject> closeSheep;

	private PlayerInput playerInput;
	[SerializeField]
	private GameObject dustPart;

	private InputAction playerMoveAction;
	
	private InputAction playerTouchPosAction;
	private InputAction playerTouchPressAction;
	private gameControl gCon;
	private levelControl lCon;

	private Vector2 inputForce;

	private bool stickControl = false;
	
	public GameObject editorPosMarker;
	public GameObject editorCellMarker;
	public Grid grid;
	public GameObject gridVisual;

	private Vector3 slopeRot;
	
	// Start is called before the first frame update
	void Awake()
	{
		playerRb = playerObj.GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		Application.targetFrameRate = 60;

		gCon = this.GetComponent<gameControl>();
		lCon = this.GetComponent<levelControl>();

		playerInput.actions.FindActionMap("Touch").Enable();
		playerInput.actions.FindActionMap("GameStick").Enable();

		playerMoveAction = playerInput.actions["PlayerMove"];
		playerTouchPressAction = playerInput.actions["TouchPress"];
		playerTouchPosAction = playerInput.actions["TouchPos"];

		//StartCoroutine(sheepCheck());
	}

	private void OnEnable(){
		playerMoveAction.performed += PlayerMovePress;
		playerMoveAction.canceled += PlayerMoveRelease;
		
		playerTouchPressAction.performed += PlayerTouchPress;
		playerTouchPressAction.canceled += PlayerTouchRelease;

	}

	

	public void disablePlay(){
		gameStickUI.gameObject.SetActive(false);
	}

	private void PlayerTouchPress(InputAction.CallbackContext value){
		
		

		if(stickControl == false){
			gameStickUI.position = playerTouchPosAction.ReadValue<Vector2>();
			stickControl = true;
		}		
	}

	private void PlayerTouchRelease(InputAction.CallbackContext value){
		
		if(stickControl == true){
			gameStickUI.position = new Vector2(-100, -100);
			stickControl = false;
		}	
	}

	private void PlayerMovePress(InputAction.CallbackContext value){
		inputForce = value.ReadValue<Vector2>();
	}

	private void PlayerMoveRelease(InputAction.CallbackContext value){
		inputForce = Vector2.zero;
	}

	bool IsGrounded()
	{
    	//return playerRb.velocity.y < 0.1f;
		return Physics.Raycast(playerObj.transform.position, -Vector3.up, 0.1f);
	}

	private void FixedUpdate() {
		Vector2 horizVelocity = new (playerRb.velocity.x,playerRb.velocity.z);
			
		if(inputForce != Vector2.zero){
			Vector3 tarDir = Quaternion.AngleAxis(45, Vector3.up) * new Vector3(inputForce.x,0,inputForce.y);
			Vector3 newDir = Vector3.RotateTowards(playerObj.transform.forward,tarDir , Time.deltaTime * 9, 0.1f);
			//playerRb.Move(playerObj.transform.position + tarDir * 0.1f, Quaternion.LookRotation(newDir));
			playerObj.transform.rotation = Quaternion.LookRotation(newDir);
			float maxSpeed = 9 * Mathf.Clamp(inputForce.magnitude, 0.5f, 1);
			if(horizVelocity.magnitude < maxSpeed){
				playerRb.AddForce(tarDir * 50);
			}
		}

		RaycastHit hit;
		//LayerMask Terrmask = LayerMask.GetMask("Terrain");
		if (Physics.Raycast(playerObj.transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.3f, 8)) {
			slopeRot = Vector3.Lerp(slopeRot, hit.normal, 0.15f);
		} else {
			slopeRot = Vector3.Lerp(slopeRot, Vector3.up, 0.15f);
		}
		playerObj.transform.rotation = Quaternion.FromToRotation (playerObj.transform.up, slopeRot) * playerObj.transform.rotation;
		
		playerAnim.SetFloat("Velocity", Mathf.Clamp(inputForce.magnitude, 0.6f, 1) * inputForce.normalized.magnitude);
		if(horizVelocity.magnitude > 5 && IsGrounded()){
			if(dustPart.GetComponent<ParticleSystem>().isStopped){
				dustPart.GetComponent<ParticleSystem>().Play();
			}
		} else {
			dustPart.GetComponent<ParticleSystem>().Stop();
		}

	float maxV = 10f;
	if(playerRb.velocity.sqrMagnitude > maxV){
		playerRb.velocity *= 0.95f;
	}


	}
	
}
