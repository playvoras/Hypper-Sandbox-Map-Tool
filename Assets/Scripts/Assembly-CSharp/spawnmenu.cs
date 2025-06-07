using UnityEngine;
using UnityEngine.UI;

public class spawnmenu : MonoBehaviour
{
	public InputField inputField;

	public float maxDistance = 30f;

	private GameObject prefab;

	public GameObject allUl;

	public GameObject PropManager;

	private void Start()
	{
	}

	public void SpawnProp()
	{
		if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hitInfo, maxDistance))
		{
			GameObject gameObject = GameObject.Find("PropManager");
			string text = inputField.text;
			prefab = Resources.Load<GameObject>("props/" + text);
			Object.Instantiate(prefab, hitInfo.point, Quaternion.identity).transform.SetParent(gameObject.transform);
			Debug.Log("Tested");
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (!allUl.activeSelf)
			{
				allUl.SetActive(value: true);
			}
			else
			{
				allUl.SetActive(value: false);
			}
		}
	}
}
