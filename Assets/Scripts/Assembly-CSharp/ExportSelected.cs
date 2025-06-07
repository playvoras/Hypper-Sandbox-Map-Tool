using System;
using System.Collections.Generic;
using System.IO;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExportSelected : MonoBehaviour
{
	public string fileName = "NewExportedContent";

	public InputField saveNameInput;

	private string directoryName = "HypperSaves";

	private List<Transform> ExportedRootsLink;

	private void Start()
	{
		ExportedRootsLink = GetComponent<TransformGizmo>().targetRootsOrdered;
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), directoryName);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public void ExportSelectedProps()
	{
		if (ExportedRootsLink.Count != 0)
		{
			fileName = saveNameInput.text;
			string text = SceneManager.GetActiveScene().name;
			string text2 = "{\n  \"map\": \"" + text + "\",\n  \"props\": [\n";
			foreach (Transform item in ExportedRootsLink)
			{
				string text3 = item.name.Replace("(Clone)", "");
				text3 = text3.Replace("(1)", "");
				text2 = text2 + "    {\n      \"name\": \"" + text3 + "\",\n";
				text2 = text2 + "      \"uniqueId\": " + (float)item.GetInstanceID() + ",\n";
				text2 = text2 + "      \"position\": {\n        \"x\": " + item.position.x.ToString().Replace(',', '.') + ",\n";
				text2 = text2 + "        \"y\": " + item.position.y.ToString().Replace(',', '.') + ",\n        \"z\": " + item.position.z.ToString().Replace(',', '.') + "\n      },\n";
				text2 = text2 + "      \"rotation\": {\n        \"x\": " + item.rotation.x.ToString().Replace(',', '.') + ",\n";
				text2 = text2 + "        \"y\": " + item.rotation.y.ToString().Replace(',', '.') + ",\n        \"z\": " + item.rotation.z.ToString().Replace(',', '.') + ",\n";
				text2 = text2 + "        \"w\": " + item.rotation.w.ToString().Replace(',', '.') + "\n      },\n";
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
	}
}
