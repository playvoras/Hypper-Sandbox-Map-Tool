using UnityEngine;

public class SmartDuplicator : MonoBehaviour
{
	public GameObject prefab;

	public int numberOfCopies = 5;

	public Vector3[] positionDeltas;

	public Vector3[] rotationDeltas;

	private void Start()
	{
		if (positionDeltas.Length == 0)
		{
			Debug.LogError("Position deltas array is empty!");
			return;
		}
		if (rotationDeltas.Length == 0)
		{
			Debug.LogError("Rotation deltas array is empty!");
			return;
		}
		GameObject gameObject = prefab;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < numberOfCopies; i++)
		{
			Vector3 vector = positionDeltas[num];
			Vector3 euler = rotationDeltas[num2];
			GameObject obj = Object.Instantiate(prefab, gameObject.transform.position + gameObject.transform.rotation * vector, gameObject.transform.rotation * Quaternion.Euler(euler));
			obj.transform.parent = gameObject.transform;
			gameObject = obj;
			num = (num + 1) % positionDeltas.Length;
			num2 = (num2 + 1) % rotationDeltas.Length;
		}
	}
}
