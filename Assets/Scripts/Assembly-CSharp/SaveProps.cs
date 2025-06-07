using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveProps : MonoBehaviour
{
	public string fileName = "NewEngineSave";

	public InputField saveNameInput;

	private string directoryName = "HypperSaves";

	public Text propsCountText;

	private void Start()
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), directoryName);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public void UpdateFileName()
	{
		fileName = saveNameInput.text;
	}

	public void SaveAllProps()
	{
		if (base.transform.childCount != 0)
		{
			string text = SceneManager.GetActiveScene().name;
			string text2 = "{\n  \"map\": \"" + text + "\",\n  \"props\": [\n";
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				string text3 = child.name.Replace("(Clone)", "");
				text3 = text3.Replace("(1)", "");
				text2 = text2 + "    {\n      \"name\": \"" + text3 + "\",\n";
				text2 = text2 + "      \"uniqueId\": " + (float)child.GetInstanceID() + ",\n";
				text2 = text2 + "      \"position\": {\n        \"x\": " + child.position.x.ToString().Replace(',', '.') + ",\n";
				text2 = text2 + "        \"y\": " + child.position.y.ToString().Replace(',', '.') + ",\n        \"z\": " + child.position.z.ToString().Replace(',', '.') + "\n      },\n";
				text2 = text2 + "      \"rotation\": {\n        \"x\": " + child.rotation.x.ToString().Replace(',', '.') + ",\n";
				text2 = text2 + "        \"y\": " + child.rotation.y.ToString().Replace(',', '.') + ",\n        \"z\": " + child.rotation.z.ToString().Replace(',', '.') + ",\n";
				text2 = text2 + "        \"w\": " + child.rotation.w.ToString().Replace(',', '.') + "\n      },\n";
				text2 += "      \"isKinematic\": true,\n";
				text2 += "      \"instantiationData\": null,\n      \"runtimeData\": null\n    },\n";
			}
			text2 = text2.Substring(0, text2.Length - 2);
			text2 += "\n  ]\n}";
			File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), directoryName, fileName + ".svn"), text2);
		}
		else
		{
			Debug.Log("PropManagerNull");
		}
	}

	private void Update()
	{
		int childCount = base.gameObject.transform.childCount;
		propsCountText.text = "Props count: " + childCount;
	}
}
