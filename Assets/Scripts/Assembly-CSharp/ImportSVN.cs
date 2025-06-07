using System;
using System.Collections.Generic;
using System.IO;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.UI;

public class ImportSVN : MonoBehaviour
{
	public GameObject buttonPrefab;

	public Transform buttonParent;

	private string propManagerName = "PropManager";

	public GameObject panel;

	private SceneData data;

	private TransformGizmo transformGizmo;

	private List<Transform> ImportedRootsLink;

	private void Start()
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HypperSaves");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string[] files = Directory.GetFiles(path, "*.svn");
		transformGizmo = GetComponent<TransformGizmo>();
		ImportedRootsLink = GetComponent<TransformGizmo>().targetRootsOrdered;
		Vector3 position = buttonParent.position;
		position.y += 200f;
		string[] array = files;
		foreach (string file in array)
		{
			GameObject obj = UnityEngine.Object.Instantiate(buttonPrefab, buttonParent);
			Text[] componentsInChildren = obj.GetComponentsInChildren<Text>();
			obj.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(file);
			componentsInChildren[0].text = Path.GetFileNameWithoutExtension(file);
			data = JsonUtility.FromJson<SceneData>(File.ReadAllText(file));
			componentsInChildren[1].text = null;
			componentsInChildren[2].text = Path.GetExtension(file);
			obj.GetComponent<Button>().onClick.AddListener(delegate
			{
				data = JsonUtility.FromJson<SceneData>(File.ReadAllText(file));
				panel.SetActive(value: false);
				ImportProps();
			});
			position.y -= 30f;
			obj.GetComponent<RectTransform>().position = position;
		}
	}

	private void ImportProps()
	{
		GameObject gameObject = GameObject.Find(propManagerName);
		if (!(gameObject != null))
		{
			return;
		}
		PropData[] props = data.props;
		foreach (PropData propData in props)
		{
			if (Resources.Load<GameObject>("props/" + propData.name) != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("props/" + propData.name), gameObject.transform);
				gameObject2.transform.localPosition = propData.position;
				gameObject2.transform.localRotation = propData.rotation;
				gameObject2.GetComponent<Rigidbody>().isKinematic = true;
				ImportedRootsLink.Add(gameObject2.transform);
				gameObject2.GetComponent<Outline>().enabled = true;
			}
			else
			{
				Debug.LogError("Prefab " + propData.name + " not found");
			}
		}
	}
}
