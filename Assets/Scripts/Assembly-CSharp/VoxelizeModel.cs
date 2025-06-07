using UnityEngine;

public class VoxelizeModel : MonoBehaviour
{
	public GameObject pointCloudPrefab;

	public float density = 1f;

	public bool fillInside = true;

	public int numberOfFacesPerPrefab = 2;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("PropManager");
		if (gameObject == null)
		{
			Debug.LogError("PropManager not found in scene!");
			return;
		}
		MeshFilter componentInChildren = GetComponentInChildren<MeshFilter>();
		if (componentInChildren != null)
		{
			Mesh sharedMesh = componentInChildren.sharedMesh;
			Bounds bounds = sharedMesh.bounds;
			if (fillInside)
			{
				for (float num = bounds.min.x; num < bounds.max.x; num += density)
				{
					for (float num2 = bounds.min.y; num2 < bounds.max.y; num2 += density)
					{
						for (float num3 = bounds.min.z; num3 < bounds.max.z; num3 += density)
						{
							Vector3 vector = new Vector3(num, num2, num3);
							if (sharedMesh.bounds.Contains(vector))
							{
								GameObject obj = Object.Instantiate(pointCloudPrefab);
								obj.transform.position = base.transform.TransformPoint(vector);
								obj.transform.parent = gameObject.transform;
							}
						}
					}
				}
			}
			else
			{
				int[] triangles = sharedMesh.triangles;
				for (int i = 0; i < triangles.Length; i += numberOfFacesPerPrefab * 3)
				{
					Vector3 position = (sharedMesh.vertices[triangles[i]] + sharedMesh.vertices[triangles[i + 1]] + sharedMesh.vertices[triangles[i + 2]]) / 3f;
					if (!(Random.value > density))
					{
						GameObject obj2 = Object.Instantiate(pointCloudPrefab);
						obj2.transform.position = base.transform.TransformPoint(position);
						obj2.transform.parent = gameObject.transform;
					}
				}
			}
		}
		Object.Destroy(base.gameObject);
	}
}
