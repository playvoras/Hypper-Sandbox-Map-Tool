using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{
	[Space]
	[SerializeField]
	[Tooltip("The script is currently active")]
	private bool _active = true;

	[Space]
	[SerializeField]
	[Tooltip("Camera rotation by mouse movement is active")]
	private bool _enableRotation = true;

	[SerializeField]
	[Tooltip("Sensitivity of mouse rotation")]
	private float _mouseSense = 1.8f;

	[Space]
	[SerializeField]
	[Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
	private bool _enableTranslation = true;

	[SerializeField]
	[Tooltip("Velocity of camera zooming in/out")]
	private float _translationSpeed = 55f;

	[Space]
	[SerializeField]
	[Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
	private bool _enableMovement = true;

	[SerializeField]
	[Tooltip("Camera movement speed")]
	private float _movementSpeed = 10f;

	[SerializeField]
	[Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
	private float _boostedSpeed = 50f;

	[SerializeField]
	[Tooltip("Boost speed")]
	private KeyCode _boostSpeed = KeyCode.LeftShift;

	[SerializeField]
	[Tooltip("Move up")]
	private KeyCode _moveUp = KeyCode.E;

	[SerializeField]
	[Tooltip("Move down")]
	private KeyCode _moveDown = KeyCode.Q;

	[Space]
	[SerializeField]
	[Tooltip("Acceleration at camera movement is active")]
	private bool _enableSpeedAcceleration = true;

	[SerializeField]
	[Tooltip("Rate which is applied during camera movement")]
	private float _speedAccelerationFactor = 1.5f;

	[Space]
	[SerializeField]
	[Tooltip("This keypress will move the camera to initialization position")]
	private KeyCode _initPositonButton = KeyCode.R;

	private CursorLockMode _wantedMode;

	private float _currentIncrease = 1f;

	private float _currentIncreaseMem;

	private Vector3 _initPosition;

	private Vector3 _initRotation;

	private void Start()
	{
		_initPosition = base.transform.position;
		_initRotation = base.transform.eulerAngles;
	}

	private void OnEnable()
	{
		if (_active)
		{
			_wantedMode = CursorLockMode.Locked;
		}
	}

	private void SetCursorState()
	{
		if (Input.GetMouseButtonUp(1) && _wantedMode == CursorLockMode.Locked)
		{
			Cursor.lockState = (_wantedMode = CursorLockMode.None);
		}
		if (Input.GetMouseButtonDown(1))
		{
			_wantedMode = CursorLockMode.Locked;
		}
		Cursor.lockState = _wantedMode;
		Cursor.visible = CursorLockMode.Locked != _wantedMode;
	}

	private void CalculateCurrentIncrease(bool moving)
	{
		_currentIncrease = Time.deltaTime;
		if (!_enableSpeedAcceleration || (_enableSpeedAcceleration && !moving))
		{
			_currentIncreaseMem = 0f;
			return;
		}
		_currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1f);
		_currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3f) * Time.deltaTime;
	}

	private void Update()
	{
		if (!_active)
		{
			return;
		}
		SetCursorState();
		if (Cursor.visible)
		{
			return;
		}
		if (_enableTranslation)
		{
			base.transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
		}
		if (_enableMovement)
		{
			Vector3 zero = Vector3.zero;
			float num = _movementSpeed;
			if (Input.GetKey(_boostSpeed))
			{
				num = _boostedSpeed;
			}
			if (Input.GetKey(KeyCode.W))
			{
				zero += base.transform.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				zero -= base.transform.forward;
			}
			if (Input.GetKey(KeyCode.A))
			{
				zero -= base.transform.right;
			}
			if (Input.GetKey(KeyCode.D))
			{
				zero += base.transform.right;
			}
			if (Input.GetKey(_moveUp))
			{
				zero += base.transform.up;
			}
			if (Input.GetKey(_moveDown))
			{
				zero -= base.transform.up;
			}
			CalculateCurrentIncrease(zero != Vector3.zero);
			base.transform.position += zero * num * _currentIncrease;
		}
		if (_enableRotation)
		{
			base.transform.rotation *= Quaternion.AngleAxis((0f - Input.GetAxis("Mouse Y")) * _mouseSense, Vector3.right);
			base.transform.rotation = Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense, base.transform.eulerAngles.z);
		}
		if (Input.GetKeyDown(_initPositonButton))
		{
			base.transform.position = _initPosition;
			base.transform.eulerAngles = _initRotation;
		}
	}
}
