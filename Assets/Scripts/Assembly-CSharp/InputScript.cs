using System.Collections.Generic;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.UI;

public class InputScript : MonoBehaviour
{
	public InputField posXInput;

	public InputField posYInput;

	public InputField posZInput;

	public InputField rotXInput;

	public InputField rotYInput;

	public InputField rotZInput;

	private List<Transform> rootsLink;

	private void Start()
	{
		posXInput.onValueChanged.AddListener(delegate
		{
			UpdateTransformX();
		});
		posYInput.onValueChanged.AddListener(delegate
		{
			UpdateTransformY();
		});
		posZInput.onValueChanged.AddListener(delegate
		{
			UpdateTransformZ();
		});
		rotXInput.onValueChanged.AddListener(delegate
		{
			UpdateRotateX();
		});
		rotYInput.onValueChanged.AddListener(delegate
		{
			UpdateRotateY();
		});
		rotZInput.onValueChanged.AddListener(delegate
		{
			UpdateRotateZ();
		});
	}

	private void Update()
	{
		rootsLink = GetComponent<TransformGizmo>().targetRootsOrdered;
		if (rootsLink.Count == 1)
		{
			posXInput.text = rootsLink[0].transform.position.x.ToString();
			posYInput.text = rootsLink[0].transform.position.y.ToString();
			posZInput.text = rootsLink[0].transform.position.z.ToString();
			posXInput.interactable = true;
			posYInput.interactable = true;
			posZInput.interactable = true;
			rotXInput.interactable = true;
			rotYInput.interactable = true;
			rotZInput.interactable = true;
		}
		else
		{
			posXInput.text = "-";
			posYInput.text = "-";
			posZInput.text = "-";
			rotXInput.text = "-";
			rotYInput.text = "-";
			rotZInput.text = "-";
			posXInput.interactable = false;
			posYInput.interactable = false;
			posZInput.interactable = false;
			rotXInput.interactable = false;
			rotYInput.interactable = false;
			rotZInput.interactable = false;
		}
	}

	private void UpdateTransformX()
	{
		if (posXInput.interactable)
		{
			rootsLink[0].transform.position = new Vector3(float.Parse(posXInput.text), rootsLink[0].transform.position.y, rootsLink[0].transform.position.z);
		}
	}

	private void UpdateTransformY()
	{
		if (posYInput.interactable)
		{
			rootsLink[0].transform.position = new Vector3(rootsLink[0].transform.position.x, float.Parse(posYInput.text), rootsLink[0].transform.position.z);
		}
	}

	private void UpdateTransformZ()
	{
		if (posZInput.interactable)
		{
			rootsLink[0].transform.position = new Vector3(rootsLink[0].transform.position.x, rootsLink[0].transform.position.y, float.Parse(posZInput.text));
		}
	}

	private void UpdateRotateX()
	{
		if (rotXInput.interactable)
		{
			rootsLink[0].transform.eulerAngles = new Vector3(float.Parse(rotXInput.text), rootsLink[0].transform.rotation.eulerAngles.y, rootsLink[0].transform.rotation.eulerAngles.z);
		}
	}

	private void UpdateRotateY()
	{
		if (rotYInput.interactable)
		{
			rootsLink[0].transform.eulerAngles = new Vector3(rootsLink[0].transform.rotation.eulerAngles.x, float.Parse(rotYInput.text), rootsLink[0].transform.rotation.eulerAngles.z);
		}
	}

	private void UpdateRotateZ()
	{
		if (rotZInput.interactable)
		{
			rootsLink[0].transform.eulerAngles = new Vector3(rootsLink[0].transform.rotation.eulerAngles.x, rootsLink[0].transform.rotation.eulerAngles.y, float.Parse(rotZInput.text));
		}
	}
}
