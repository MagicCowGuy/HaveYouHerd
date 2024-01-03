using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class TouchManager : MonoBehaviour
{
	[SerializeField]
	private GameObject playerObj;
	private NavMeshAgent playerNA;

	private Transform cameraTransform;
	
	private float touchTime;
	private Vector2 touchStartPos;
	private Vector2 camStartPos;
	private Vector2 camMovePos;
	private float touchDistance;
	private bool isTouch = false;

	private PlayerInput playerInput;

	private InputAction touchPosAction;
	private InputAction touchPressAction;

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		playerNA = playerObj.GetComponent<NavMeshAgent>();
		cameraTransform = Camera.main.gameObject.transform;

		touchPressAction = playerInput.actions["TouchPress"];
		touchPosAction = playerInput.actions["TouchPos"];
	}

	private void OnEnable() {
		touchPressAction.performed += TouchPressed;
		touchPressAction.canceled += TouchReleased;
	}

	 private void OnDisable() {
		touchPressAction.performed -= TouchPressed;
		touchPressAction.canceled -= TouchReleased;
	 }

	private void TouchPressed(InputAction.CallbackContext context){
		print("Touch Pressed!");
		touchTime = Time.time;
		touchStartPos = touchPosAction.ReadValue<Vector2>();
		isTouch = true;
		camStartPos = new Vector2(cameraTransform.position.x, cameraTransform.position.z);
			
	}

	private void TouchReleased(InputAction.CallbackContext context){
		print("Touch Released! ");
		isTouch = false;
		Vector3 position = Camera.main.ScreenToWorldPoint(touchPosAction.ReadValue<Vector2>());
		position.z = 0;
		//playerObj.transform.position = position;

		touchTime = Time.time - touchTime;
		print("Duration of touch: " + touchTime);

		touchDistance = Vector2.Distance(touchPosAction.ReadValue<Vector2>(), touchStartPos);
		print("Distance from start point: " + touchDistance);
		if(touchDistance > 150f){
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(touchPosAction.ReadValue<Vector2>());
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 100)) {
			//playerObj.transform.position = hit.point;
			playerNA.SetDestination(hit.point);
		}
	}
	void Update()
	{
		if(isTouch){


			camMovePos = camStartPos - (touchPosAction.ReadValue<Vector2>() - touchStartPos)/100f;
			cameraTransform.position = new Vector3(camMovePos.x, 20f, camMovePos.y);
			//print(touchPosAction.ReadValue<Vector2>());
			
		}
	}
}
