using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SVNScript : MonoBehaviour
{
	public GameObject buttonPrefab;

	public Transform buttonParent;

	private string propManagerName = "PropManager";

	private SceneData data;

	private void Start()
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HypperSaves");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string[] files = Directory.GetFiles(path, "*.svn");
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
			componentsInChildren[1].text = data.map;
			componentsInChildren[2].text = Path.GetExtension(file);
			obj.GetComponent<Button>().onClick.AddListener(delegate
			{
				data = JsonUtility.FromJson<SceneData>(File.ReadAllText(file));
				SceneManager.sceneLoaded += OnSceneLoaded;
				SceneManager.LoadScene(data.map);
			});
			position.y -= 30f;
			obj.GetComponent<RectTransform>().position = position;
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
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
				GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("props/" + propData.name), gameObject.transform);
				obj.transform.localPosition = propData.position;
				obj.transform.localRotation = propData.rotation;
				obj.GetComponent<Rigidbody>().isKinematic = true;
			}
			else
			{
				Debug.LogError("Prefab " + propData.name + " not found");
			}
		}
	}
}
