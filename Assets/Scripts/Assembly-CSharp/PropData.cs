using System;
using UnityEngine;

[Serializable]
public class PropData
{
	public string name;

	public float uniqueId;

	public Vector3 position;

	public Quaternion rotation;

	public bool isKinematic;

	public object instantiationData;

	public object runtimeData;
}
