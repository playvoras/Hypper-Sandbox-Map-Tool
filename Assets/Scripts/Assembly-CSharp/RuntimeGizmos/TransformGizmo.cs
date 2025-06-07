using System;
using System.Collections;
using System.Collections.Generic;
using CommandUndoRedo;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeGizmos
{
	[RequireComponent(typeof(Camera))]
	public class TransformGizmo : MonoBehaviour
	{
		private Renderer[] renderers;

		private InputField rotXLink;

		private InputField rotYLink;

		private InputField rotZLink;

		public TransformSpace space;

		public TransformType transformType;

		public TransformPivot pivot;

		public CenterType centerType;

		public GameObject PropManager;

		public KeyCode SetMoveType = KeyCode.E;

		public KeyCode SetRotateType = KeyCode.R;

		public KeyCode SetRectToolType = KeyCode.T;

		public KeyCode SetAllTransformType = KeyCode.Y;

		public KeyCode SetSpaceToggle = KeyCode.X;

		public KeyCode SetPivotModeToggle = KeyCode.Z;

		public KeyCode SetCenterTypeToggle = KeyCode.H;

		public KeyCode translationSnapping = KeyCode.LeftControl;

		public KeyCode AddSelection = KeyCode.LeftShift;

		public KeyCode RemoveSelection = KeyCode.LeftAlt;

		public KeyCode ActionKey = KeyCode.LeftShift;

		public KeyCode UndoAction = KeyCode.Z;

		public KeyCode RedoAction = KeyCode.Y;

		public KeyCode Delete = KeyCode.Delete;

		public KeyCode SelectAll = KeyCode.G;

		public Color xColor = new Color(1f, 0f, 0f, 0.8f);

		public Color yColor = new Color(0f, 1f, 0f, 0.8f);

		public Color zColor = new Color(0f, 0f, 1f, 0.8f);

		public Color allColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);

		public Color selectedColor = new Color(1f, 1f, 0f, 0.8f);

		public Color hoverColor = new Color(1f, 0.75f, 0f, 0.8f);

		public float planesOpacity = 0.5f;

		public float movementSnap = 0.25f;

		public float rotationSnap = 15f;

		public float scaleSnap = 1f;

		public float handleLength = 0.25f;

		public float handleWidth = 0.003f;

		public float planeSize = 0.035f;

		public float triangleSize = 0.03f;

		public float boxSize = 0.03f;

		public int circleDetail = 40;

		public float allMoveHandleLengthMultiplier = 1f;

		public float allRotateHandleLengthMultiplier = 1.4f;

		public float allScaleHandleLengthMultiplier = 1.6f;

		public float minSelectedDistanceCheck = 0.01f;

		public float moveSpeedMultiplier = 1f;

		public float scaleSpeedMultiplier = 1f;

		public float rotateSpeedMultiplier = 1f;

		public float allRotateSpeedMultiplier = 20f;

		public bool useFirstSelectedAsMain = true;

		public bool circularRotationMethod;

		public bool forceUpdatePivotPointOnChange = true;

		public int maxUndoStored = 100;

		public bool manuallyHandleGizmo;

		public GameObject ClonedObjects;

		public LayerMask selectionMask = -5;

		public Action onCheckForSelectedAxis;

		public Action onDrawCustomGizmo;

		private Vector3 totalCenterPivotPoint;

		private AxisInfo axisInfo;

		private Axis nearAxis;

		private Axis planeAxis;

		private TransformType translatingType;

		private AxisVectors handleLines = new AxisVectors();

		private AxisVectors handlePlanes = new AxisVectors();

		private AxisVectors handleTriangles = new AxisVectors();

		private AxisVectors circlesLines = new AxisVectors();

		public List<Transform> targetRootsOrdered = new List<Transform>();

		private Dictionary<Transform, TargetInfo> targetRoots = new Dictionary<Transform, TargetInfo>();

		private HashSet<Renderer> highlightedRenderers = new HashSet<Renderer>();

		private HashSet<Transform> children = new HashSet<Transform>();

		private List<Transform> childrenBuffer = new List<Transform>();

		private List<Renderer> renderersBuffer = new List<Renderer>();

		private List<Material> materialsBuffer = new List<Material>();

		private WaitForEndOfFrame waitForEndOFFrame = new WaitForEndOfFrame();

		private Coroutine forceUpdatePivotCoroutine;

		private static Material lineMaterial;

		private static Material outlineMaterial;

		public Camera myCamera { get; private set; }

		public bool isTransforming { get; private set; }

		public float totalScaleAmount { get; private set; }

		public Quaternion totalRotationAmount { get; private set; }

		public Axis translatingAxis => nearAxis;

		public Axis translatingAxisPlane => planeAxis;

		public bool hasTranslatingAxisPlane
		{
			get
			{
				if (translatingAxisPlane != Axis.None)
				{
					return translatingAxisPlane != Axis.Any;
				}
				return false;
			}
		}

		public TransformType transformingType => translatingType;

		public Vector3 pivotPoint { get; private set; }

		public Transform mainTargetRoot
		{
			get
			{
				if (targetRootsOrdered.Count <= 0)
				{
					return null;
				}
				if (!useFirstSelectedAsMain)
				{
					return targetRootsOrdered[targetRootsOrdered.Count - 1];
				}
				return targetRootsOrdered[0];
			}
		}

		private void Start()
		{
			rotXLink = GetComponent<InputScript>().rotXInput;
			rotYLink = GetComponent<InputScript>().rotYInput;
			rotZLink = GetComponent<InputScript>().rotZInput;
		}

		private void Awake()
		{
			myCamera = GetComponent<Camera>();
			SetMaterial();
		}

		private void OnEnable()
		{
			forceUpdatePivotCoroutine = StartCoroutine(ForceUpdatePivotPointAtEndOfFrame());
		}

		private void OnDisable()
		{
			ClearTargets();
			StopCoroutine(forceUpdatePivotCoroutine);
		}

		private void OnDestroy()
		{
			ClearAllHighlightedRenderers();
		}

		private void Update()
		{
			if (Input.GetKeyDown(Delete))
			{
				for (int count = targetRootsOrdered.Count; count >= 0; count = targetRootsOrdered.Count)
				{
					UnityEngine.Object.Destroy(targetRootsOrdered[targetRootsOrdered.Count - 1].gameObject);
					targetRootsOrdered.RemoveAt(targetRootsOrdered.Count - 1);
				}
				targetRootsOrdered.Clear();
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				foreach (Transform item in targetRootsOrdered)
				{
					item.gameObject.GetComponent<Outline>().enabled = false;
				}
				int num = 1;
				int count2 = targetRootsOrdered.Count;
				for (int i = 0; i < count2; i++)
				{
					ClonedObjects = UnityEngine.Object.Instantiate(targetRootsOrdered[targetRootsOrdered.Count - num].gameObject);
					ClonedObjects.transform.SetParent(PropManager.transform);
					ClonedObjects = null;
					num++;
				}
				foreach (Transform item2 in targetRootsOrdered)
				{
					item2.gameObject.GetComponent<Outline>().enabled = true;
				}
			}
			if (Input.GetKeyDown(SelectAll))
			{
				foreach (Transform item3 in PropManager.transform)
				{
					AddTarget(item3);
				}
			}
			if (IsPointerOverUIObject())
			{
				return;
			}
			HandleUndoRedo();
			SetSpaceAndType();
			if (manuallyHandleGizmo)
			{
				if (onCheckForSelectedAxis != null)
				{
					onCheckForSelectedAxis();
				}
			}
			else
			{
				SetNearAxis();
			}
			GetTarget();
			if (!(mainTargetRoot == null))
			{
				TransformSelected();
			}
		}

		private void LateUpdate()
		{
			if (mainTargetRoot == null)
			{
				return;
			}
			SetAxisInfo();
			if (manuallyHandleGizmo)
			{
				if (onDrawCustomGizmo != null)
				{
					onDrawCustomGizmo();
				}
			}
			else
			{
				SetLines();
			}
		}

		private void OnPostRender()
		{
			if (!(mainTargetRoot == null) && !manuallyHandleGizmo)
			{
				lineMaterial.SetPass(0);
				Color nearColor = ((nearAxis != Axis.X) ? xColor : (isTransforming ? selectedColor : hoverColor));
				Color nearColor2 = ((nearAxis != Axis.Y) ? yColor : (isTransforming ? selectedColor : hoverColor));
				Color nearColor3 = ((nearAxis != Axis.Z) ? zColor : (isTransforming ? selectedColor : hoverColor));
				Color nearColor4 = ((nearAxis != Axis.Any) ? allColor : (isTransforming ? selectedColor : hoverColor));
				TransformType type = ((transformType == TransformType.Scale || (isTransforming && translatingType == TransformType.Scale)) ? TransformType.Scale : TransformType.Move);
				DrawQuads(handleLines.z, GetColor(type, zColor, nearColor3, hasTranslatingAxisPlane));
				DrawQuads(handleLines.x, GetColor(type, xColor, nearColor, hasTranslatingAxisPlane));
				DrawQuads(handleLines.y, GetColor(type, yColor, nearColor2, hasTranslatingAxisPlane));
				DrawTriangles(handleTriangles.x, GetColor(TransformType.Move, xColor, nearColor, hasTranslatingAxisPlane));
				DrawTriangles(handleTriangles.y, GetColor(TransformType.Move, yColor, nearColor2, hasTranslatingAxisPlane));
				DrawTriangles(handleTriangles.z, GetColor(TransformType.Move, zColor, nearColor3, hasTranslatingAxisPlane));
				DrawQuads(handlePlanes.z, GetColor(TransformType.Move, zColor, nearColor3, planesOpacity, !hasTranslatingAxisPlane));
				DrawQuads(handlePlanes.x, GetColor(TransformType.Move, xColor, nearColor, planesOpacity, !hasTranslatingAxisPlane));
				DrawQuads(handlePlanes.y, GetColor(TransformType.Move, yColor, nearColor2, planesOpacity, !hasTranslatingAxisPlane));
				DrawQuads(circlesLines.all, GetColor(TransformType.Rotate, allColor, nearColor4));
				DrawQuads(circlesLines.x, GetColor(TransformType.Rotate, xColor, nearColor));
				DrawQuads(circlesLines.y, GetColor(TransformType.Rotate, yColor, nearColor2));
				DrawQuads(circlesLines.z, GetColor(TransformType.Rotate, zColor, nearColor3));
			}
		}

		private Color GetColor(TransformType type, Color normalColor, Color nearColor, bool forceUseNormal = false)
		{
			return GetColor(type, normalColor, nearColor, setAlpha: false, 1f, forceUseNormal);
		}

		private Color GetColor(TransformType type, Color normalColor, Color nearColor, float alpha, bool forceUseNormal = false)
		{
			return GetColor(type, normalColor, nearColor, setAlpha: true, alpha, forceUseNormal);
		}

		private Color GetColor(TransformType type, Color normalColor, Color nearColor, bool setAlpha, float alpha, bool forceUseNormal = false)
		{
			Color result = ((forceUseNormal || !TranslatingTypeContains(type, checkIsTransforming: false)) ? normalColor : nearColor);
			if (setAlpha)
			{
				result.a = alpha;
			}
			return result;
		}

		private void HandleUndoRedo()
		{
			if (maxUndoStored != UndoRedoManager.maxUndoStored)
			{
				UndoRedoManager.maxUndoStored = maxUndoStored;
			}
			if (Input.GetKey(ActionKey))
			{
				if (Input.GetKeyDown(UndoAction))
				{
					UndoRedoManager.Undo();
				}
				else if (Input.GetKeyDown(RedoAction))
				{
					UndoRedoManager.Redo();
				}
			}
		}

		public TransformSpace GetProperTransformSpace()
		{
			if (transformType != TransformType.Scale)
			{
				return space;
			}
			return TransformSpace.Local;
		}

		public bool TransformTypeContains(TransformType type)
		{
			return TransformTypeContains(transformType, type);
		}

		public bool TranslatingTypeContains(TransformType type, bool checkIsTransforming = true)
		{
			TransformType mainType = ((!checkIsTransforming || isTransforming) ? translatingType : transformType);
			return TransformTypeContains(mainType, type);
		}

		public bool TransformTypeContains(TransformType mainType, TransformType type)
		{
			return mainType.TransformTypeContains(type, GetProperTransformSpace());
		}

		public float GetHandleLength(TransformType type, Axis axis = Axis.None, bool multiplyDistanceMultiplier = true)
		{
			float num = handleLength;
			if (transformType == TransformType.All)
			{
				if (type == TransformType.Move)
				{
					num *= allMoveHandleLengthMultiplier;
				}
				if (type == TransformType.Rotate)
				{
					num *= allRotateHandleLengthMultiplier;
				}
			}
			if (multiplyDistanceMultiplier)
			{
				num *= GetDistanceMultiplier();
			}
			if (type == TransformType.Scale && isTransforming && (translatingAxis == axis || translatingAxis == Axis.Any))
			{
				num += totalScaleAmount;
			}
			return num;
		}

		private void SetSpaceAndType()
		{
			if (Input.GetKey(ActionKey))
			{
				return;
			}
			if (Input.GetKeyDown(SetMoveType))
			{
				transformType = TransformType.Move;
			}
			else if (Input.GetKeyDown(SetRotateType))
			{
				transformType = TransformType.Rotate;
			}
			else if (Input.GetKeyDown(SetAllTransformType))
			{
				transformType = TransformType.All;
			}
			if (!isTransforming)
			{
				translatingType = transformType;
			}
			if (Input.GetKeyDown(SetPivotModeToggle))
			{
				if (pivot == TransformPivot.Pivot)
				{
					pivot = TransformPivot.Center;
				}
				else if (pivot == TransformPivot.Center)
				{
					pivot = TransformPivot.Pivot;
				}
				SetPivotPoint();
			}
			if (Input.GetKeyDown(SetCenterTypeToggle))
			{
				if (centerType == CenterType.All)
				{
					centerType = CenterType.Solo;
				}
				else if (centerType == CenterType.Solo)
				{
					centerType = CenterType.All;
				}
				SetPivotPoint();
			}
			if (Input.GetKeyDown(SetSpaceToggle))
			{
				if (space == TransformSpace.Global)
				{
					space = TransformSpace.Local;
				}
				else if (space == TransformSpace.Local)
				{
					space = TransformSpace.Global;
				}
			}
		}

		private void TransformSelected()
		{
			if (mainTargetRoot != null)
			{
				if (nearAxis != Axis.None && Input.GetMouseButtonDown(0))
				{
					StartCoroutine(TransformSelected(translatingType));
				}
			}
			else
			{
				RotFieldSet();
			}
		}

		private IEnumerator TransformSelected(TransformType transType)
		{
			isTransforming = true;
			totalScaleAmount = 0f;
			totalRotationAmount = Quaternion.identity;
			Vector3 originalPivot = pivotPoint;
			Vector3 otherAxis1;
			Vector3 otherAxis2;
			Vector3 axis = GetNearAxisDirection(out otherAxis1, out otherAxis2);
			Vector3 planeNormal = (hasTranslatingAxisPlane ? axis : (base.transform.position - originalPivot).normalized);
			Vector3 projectedAxis = Vector3.ProjectOnPlane(axis, planeNormal).normalized;
			Vector3 previousMousePosition = Vector3.zero;
			Vector3 currentSnapMovementAmount = Vector3.zero;
			float currentSnapRotationAmount = 0f;
			List<ICommand> transformCommands = new List<ICommand>();
			for (int i = 0; i < targetRootsOrdered.Count; i++)
			{
				transformCommands.Add(new TransformCommand(this, targetRootsOrdered[i]));
			}
			while (!Input.GetMouseButtonUp(0))
			{
				Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 vector = Geometry.LinePlaneIntersect(ray.origin, ray.direction, originalPivot, planeNormal);
				bool key = Input.GetKey(translationSnapping);
				if (previousMousePosition != Vector3.zero && vector != Vector3.zero)
				{
					switch (transType)
					{
					case TransformType.Move:
					{
						_ = Vector3.zero;
						Vector3 vector2;
						if (hasTranslatingAxisPlane)
						{
							vector2 = vector - previousMousePosition;
						}
						else
						{
							float num2 = ExtVector3.MagnitudeInDirection(vector - previousMousePosition, projectedAxis) * moveSpeedMultiplier;
							vector2 = axis * num2;
						}
						if (key && movementSnap > 0f)
						{
							currentSnapMovementAmount += vector2;
							vector2 = Vector3.zero;
							if (hasTranslatingAxisPlane)
							{
								float currentAmount = ExtVector3.MagnitudeInDirection(currentSnapMovementAmount, otherAxis1);
								float currentAmount2 = ExtVector3.MagnitudeInDirection(currentSnapMovementAmount, otherAxis2);
								float remainder2;
								float num3 = CalculateSnapAmount(movementSnap, currentAmount, out remainder2);
								float remainder3;
								float num4 = CalculateSnapAmount(movementSnap, currentAmount2, out remainder3);
								if (num3 != 0f)
								{
									Vector3 vector3 = otherAxis1 * num3;
									vector2 += vector3;
									currentSnapMovementAmount -= vector3;
								}
								if (num4 != 0f)
								{
									Vector3 vector4 = otherAxis2 * num4;
									vector2 += vector4;
									currentSnapMovementAmount -= vector4;
								}
							}
							else
							{
								float remainder4;
								float num5 = CalculateSnapAmount(movementSnap, currentSnapMovementAmount.magnitude, out remainder4);
								if (num5 != 0f)
								{
									vector2 = currentSnapMovementAmount.normalized * num5;
									currentSnapMovementAmount = currentSnapMovementAmount.normalized * remainder4;
								}
							}
						}
						for (int k = 0; k < targetRootsOrdered.Count; k++)
						{
							targetRootsOrdered[k].Translate(vector2, Space.World);
						}
						SetPivotPointOffset(vector2);
						break;
					}
					case TransformType.Rotate:
					{
						float angle = 0f;
						Vector3 axis2 = axis;
						if (nearAxis == Axis.Any)
						{
							Quaternion.Euler(base.transform.TransformDirection(new Vector3(Input.GetAxis("Mouse Y"), 0f - Input.GetAxis("Mouse X"), 0f))).ToAngleAxis(out angle, out axis2);
							angle *= allRotateSpeedMultiplier;
						}
						else if (circularRotationMethod)
						{
							angle = Vector3.SignedAngle(previousMousePosition - originalPivot, vector - originalPivot, axis) * rotateSpeedMultiplier;
						}
						else
						{
							Vector3 direction = ((nearAxis == Axis.Any || ExtVector3.IsParallel(axis, planeNormal)) ? planeNormal : Vector3.Cross(axis, planeNormal));
							angle = ExtVector3.MagnitudeInDirection(vector - previousMousePosition, direction) * (rotateSpeedMultiplier * 100f) / GetDistanceMultiplier();
						}
						if (key && rotationSnap > 0f)
						{
							currentSnapRotationAmount += angle;
							angle = 0f;
							float remainder;
							float num = CalculateSnapAmount(rotationSnap, currentSnapRotationAmount, out remainder);
							if (num != 0f)
							{
								angle = num;
								currentSnapRotationAmount = remainder;
							}
						}
						for (int j = 0; j < targetRootsOrdered.Count; j++)
						{
							Transform transform = targetRootsOrdered[j];
							if (pivot == TransformPivot.Pivot)
							{
								transform.Rotate(axis2, angle, Space.World);
							}
							else if (pivot == TransformPivot.Center)
							{
								transform.RotateAround(originalPivot, axis2, angle);
							}
						}
						totalRotationAmount *= Quaternion.Euler(axis2 * angle);
						break;
					}
					}
				}
				previousMousePosition = vector;
				yield return null;
			}
			for (int l = 0; l < transformCommands.Count; l++)
			{
				((TransformCommand)transformCommands[l]).StoreNewTransformValues();
			}
			CommandGroup commandGroup = new CommandGroup();
			commandGroup.Set(transformCommands);
			UndoRedoManager.Insert(commandGroup);
			totalRotationAmount = Quaternion.identity;
			totalScaleAmount = 0f;
			isTransforming = false;
			SetTranslatingAxis(transformType, Axis.None);
			SetPivotPoint();
		}

		private float CalculateSnapAmount(float snapValue, float currentAmount, out float remainder)
		{
			remainder = 0f;
			if (snapValue <= 0f)
			{
				return currentAmount;
			}
			float num = Mathf.Abs(currentAmount);
			if (num > snapValue)
			{
				remainder = num % snapValue;
				return snapValue * (Mathf.Sign(currentAmount) * Mathf.Floor(num / snapValue));
			}
			return 0f;
		}

		private Vector3 GetNearAxisDirection(out Vector3 otherAxis1, out Vector3 otherAxis2)
		{
			otherAxis1 = (otherAxis2 = Vector3.zero);
			if (nearAxis != Axis.None)
			{
				if (nearAxis == Axis.X)
				{
					otherAxis1 = axisInfo.yDirection;
					otherAxis2 = axisInfo.zDirection;
					return axisInfo.xDirection;
				}
				if (nearAxis == Axis.Y)
				{
					otherAxis1 = axisInfo.xDirection;
					otherAxis2 = axisInfo.zDirection;
					return axisInfo.yDirection;
				}
				if (nearAxis == Axis.Z)
				{
					otherAxis1 = axisInfo.xDirection;
					otherAxis2 = axisInfo.yDirection;
					return axisInfo.zDirection;
				}
				if (nearAxis == Axis.Any)
				{
					return Vector3.one;
				}
			}
			return Vector3.zero;
		}

		private void RotFieldSet()
		{
			rotXLink.text = targetRootsOrdered[0].transform.rotation.x.ToString();
			rotYLink.text = targetRootsOrdered[0].transform.rotation.y.ToString();
			rotZLink.text = targetRootsOrdered[0].transform.rotation.z.ToString();
		}

		private void GetTarget()
		{
			if (nearAxis != Axis.None || !Input.GetMouseButtonDown(0))
			{
				return;
			}
			bool key = Input.GetKey(AddSelection);
			bool key2 = Input.GetKey(RemoveSelection);
			if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, float.PositiveInfinity, selectionMask))
			{
				Transform target = hitInfo.transform;
				if (key)
				{
					AddTarget(target);
				}
				else if (key2)
				{
					RemoveTarget(target);
				}
				else if (!key && !key2)
				{
					ClearAndAddTarget(target);
				}
			}
			else if (!key && !key2)
			{
				ClearTargets();
			}
		}

		public void AddTarget(Transform target, bool addCommand = true)
		{
			if (target != null && !targetRoots.ContainsKey(target) && !children.Contains(target))
			{
				if (addCommand)
				{
					UndoRedoManager.Insert(new AddTargetCommand(this, target, targetRootsOrdered));
				}
				target.GetComponent<Outline>().enabled = true;
				AddTargetRoot(target);
				AddTargetHighlightedRenderers(target);
				SetPivotPoint();
			}
		}

		public void RemoveTarget(Transform target, bool addCommand = true)
		{
			if (target != null && targetRoots.ContainsKey(target))
			{
				if (addCommand)
				{
					UndoRedoManager.Insert(new RemoveTargetCommand(this, target));
				}
				target.GetComponent<Outline>().enabled = false;
				RemoveTargetHighlightedRenderers(target);
				RemoveTargetRoot(target);
				SetPivotPoint();
			}
		}

		public void ClearTargets(bool addCommand = true)
		{
			if (addCommand)
			{
				UndoRedoManager.Insert(new ClearTargetsCommand(this, targetRootsOrdered));
			}
			foreach (Transform item in targetRootsOrdered)
			{
				item.GetComponent<Outline>().enabled = false;
			}
			ClearAllHighlightedRenderers();
			targetRoots.Clear();
			targetRootsOrdered.Clear();
			children.Clear();
		}

		private void ClearAndAddTarget(Transform target)
		{
			UndoRedoManager.Insert(new ClearAndAddTargetCommand(this, target, targetRootsOrdered));
			ClearTargets(addCommand: false);
			AddTarget(target, addCommand: false);
		}

		private void AddTargetHighlightedRenderers(Transform target)
		{
			if (!(target != null))
			{
				return;
			}
			GetTargetRenderers(target, renderersBuffer);
			for (int i = 0; i < renderersBuffer.Count; i++)
			{
				Renderer renderer = renderersBuffer[i];
				if (!highlightedRenderers.Contains(renderer))
				{
					materialsBuffer.Clear();
					materialsBuffer.AddRange(renderer.sharedMaterials);
					if (!materialsBuffer.Contains(outlineMaterial))
					{
						materialsBuffer.Add(outlineMaterial);
						renderer.materials = materialsBuffer.ToArray();
					}
					highlightedRenderers.Add(renderer);
				}
			}
			materialsBuffer.Clear();
		}

		private void GetTargetRenderers(Transform target, List<Renderer> renderers)
		{
			renderers.Clear();
			if (target != null)
			{
				target.GetComponentsInChildren(includeInactive: true, renderers);
			}
		}

		private void ClearAllHighlightedRenderers()
		{
			foreach (KeyValuePair<Transform, TargetInfo> targetRoot in targetRoots)
			{
				RemoveTargetHighlightedRenderers(targetRoot.Key);
			}
			renderersBuffer.Clear();
			renderersBuffer.AddRange(highlightedRenderers);
			RemoveHighlightedRenderers(renderersBuffer);
		}

		private void RemoveTargetHighlightedRenderers(Transform target)
		{
			GetTargetRenderers(target, renderersBuffer);
			RemoveHighlightedRenderers(renderersBuffer);
		}

		private void RemoveHighlightedRenderers(List<Renderer> renderers)
		{
			for (int i = 0; i < renderersBuffer.Count; i++)
			{
				Renderer renderer = renderersBuffer[i];
				if (renderer != null)
				{
					materialsBuffer.Clear();
					materialsBuffer.AddRange(renderer.sharedMaterials);
					if (materialsBuffer.Contains(outlineMaterial))
					{
						materialsBuffer.Remove(outlineMaterial);
						renderer.materials = materialsBuffer.ToArray();
					}
				}
				highlightedRenderers.Remove(renderer);
			}
			renderersBuffer.Clear();
		}

		private void AddTargetRoot(Transform targetRoot)
		{
			targetRoots.Add(targetRoot, new TargetInfo());
			targetRootsOrdered.Add(targetRoot);
			AddAllChildren(targetRoot);
		}

		private void RemoveTargetRoot(Transform targetRoot)
		{
			if (targetRoots.Remove(targetRoot))
			{
				targetRootsOrdered.Remove(targetRoot);
				RemoveAllChildren(targetRoot);
			}
		}

		private void AddAllChildren(Transform target)
		{
			childrenBuffer.Clear();
			target.GetComponentsInChildren(includeInactive: true, childrenBuffer);
			childrenBuffer.Remove(target);
			for (int i = 0; i < childrenBuffer.Count; i++)
			{
				Transform transform = childrenBuffer[i];
				children.Add(transform);
				RemoveTargetRoot(transform);
			}
			childrenBuffer.Clear();
		}

		private void RemoveAllChildren(Transform target)
		{
			childrenBuffer.Clear();
			target.GetComponentsInChildren(includeInactive: true, childrenBuffer);
			childrenBuffer.Remove(target);
			for (int i = 0; i < childrenBuffer.Count; i++)
			{
				children.Remove(childrenBuffer[i]);
			}
			childrenBuffer.Clear();
		}

		public void SetPivotPoint()
		{
			if (!(mainTargetRoot != null))
			{
				return;
			}
			if (pivot == TransformPivot.Pivot)
			{
				pivotPoint = mainTargetRoot.position;
			}
			else if (pivot == TransformPivot.Center)
			{
				totalCenterPivotPoint = Vector3.zero;
				Dictionary<Transform, TargetInfo>.Enumerator enumerator = targetRoots.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Transform key = enumerator.Current.Key;
					TargetInfo value = enumerator.Current.Value;
					value.centerPivotPoint = key.GetCenter(centerType);
					totalCenterPivotPoint += value.centerPivotPoint;
				}
				totalCenterPivotPoint /= (float)targetRoots.Count;
				if (centerType == CenterType.Solo)
				{
					pivotPoint = targetRoots[mainTargetRoot].centerPivotPoint;
				}
				else if (centerType == CenterType.All)
				{
					pivotPoint = totalCenterPivotPoint;
				}
			}
		}

		private void SetPivotPointOffset(Vector3 offset)
		{
			pivotPoint += offset;
			totalCenterPivotPoint += offset;
		}

		private IEnumerator ForceUpdatePivotPointAtEndOfFrame()
		{
			while (base.enabled)
			{
				ForceUpdatePivotPointOnChange();
				yield return waitForEndOFFrame;
			}
		}

		private void ForceUpdatePivotPointOnChange()
		{
			if (!forceUpdatePivotPointOnChange || !(mainTargetRoot != null) || isTransforming)
			{
				return;
			}
			bool flag = false;
			Dictionary<Transform, TargetInfo>.Enumerator enumerator = targetRoots.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!flag && enumerator.Current.Value.previousPosition != Vector3.zero && enumerator.Current.Key.position != enumerator.Current.Value.previousPosition)
				{
					SetPivotPoint();
					flag = true;
				}
				enumerator.Current.Value.previousPosition = enumerator.Current.Key.position;
			}
		}

		public void SetTranslatingAxis(TransformType type, Axis axis, Axis planeAxis = Axis.None)
		{
			translatingType = type;
			nearAxis = axis;
			this.planeAxis = planeAxis;
		}

		public AxisInfo GetAxisInfo()
		{
			AxisInfo result = axisInfo;
			if (isTransforming && GetProperTransformSpace() == TransformSpace.Global && translatingType == TransformType.Rotate)
			{
				result.xDirection = totalRotationAmount * Vector3.right;
				result.yDirection = totalRotationAmount * Vector3.up;
				result.zDirection = totalRotationAmount * Vector3.forward;
			}
			return result;
		}

		private void SetNearAxis()
		{
			if (isTransforming)
			{
				return;
			}
			SetTranslatingAxis(transformType, Axis.None);
			if (mainTargetRoot == null)
			{
				return;
			}
			float distanceMultiplier = GetDistanceMultiplier();
			float num = (minSelectedDistanceCheck + handleWidth) * distanceMultiplier;
			if (nearAxis == Axis.None && (TransformTypeContains(TransformType.Move) || TransformTypeContains(TransformType.Scale)))
			{
				if (nearAxis == Axis.None && TransformTypeContains(TransformType.Move))
				{
					float num2 = (minSelectedDistanceCheck + planeSize) * distanceMultiplier;
					HandleNearestPlanes(TransformType.Move, handlePlanes, num2);
					if (nearAxis != Axis.None)
					{
						planeAxis = nearAxis;
					}
					else
					{
						float num3 = (minSelectedDistanceCheck + triangleSize) * distanceMultiplier;
						HandleNearestLines(TransformType.Move, handleTriangles, num3);
					}
				}
				if (nearAxis == Axis.None)
				{
					TransformType type = ((transformType != TransformType.All) ? transformType : TransformType.Move);
					HandleNearestLines(type, handleLines, num);
				}
			}
			if (nearAxis == Axis.None && TransformTypeContains(TransformType.Rotate))
			{
				HandleNearestLines(TransformType.Rotate, circlesLines, num);
			}
		}

		private void HandleNearestLines(TransformType type, AxisVectors axisVectors, float minSelectedDistanceCheck)
		{
			float xClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.x);
			float yClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.y);
			float zClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.z);
			float allClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.all);
			HandleNearest(type, xClosestDistance, yClosestDistance, zClosestDistance, allClosestDistance, minSelectedDistanceCheck);
		}

		private void HandleNearestPlanes(TransformType type, AxisVectors axisVectors, float minSelectedDistanceCheck)
		{
			float xClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.x);
			float yClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.y);
			float zClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.z);
			float allClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.all);
			HandleNearest(type, xClosestDistance, yClosestDistance, zClosestDistance, allClosestDistance, minSelectedDistanceCheck);
		}

		private void HandleNearest(TransformType type, float xClosestDistance, float yClosestDistance, float zClosestDistance, float allClosestDistance, float minSelectedDistanceCheck)
		{
			if (type == TransformType.Scale && allClosestDistance <= minSelectedDistanceCheck)
			{
				SetTranslatingAxis(type, Axis.Any);
			}
			else if (xClosestDistance <= minSelectedDistanceCheck && xClosestDistance <= yClosestDistance && xClosestDistance <= zClosestDistance)
			{
				SetTranslatingAxis(type, Axis.X);
			}
			else if (yClosestDistance <= minSelectedDistanceCheck && yClosestDistance <= xClosestDistance && yClosestDistance <= zClosestDistance)
			{
				SetTranslatingAxis(type, Axis.Y);
			}
			else if (zClosestDistance <= minSelectedDistanceCheck && zClosestDistance <= xClosestDistance && zClosestDistance <= yClosestDistance)
			{
				SetTranslatingAxis(type, Axis.Z);
			}
			else if (type == TransformType.Rotate && mainTargetRoot != null)
			{
				Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 vector = Geometry.LinePlaneIntersect(ray.origin, ray.direction, pivotPoint, (base.transform.position - pivotPoint).normalized);
				if ((pivotPoint - vector).sqrMagnitude <= GetHandleLength(TransformType.Rotate).Squared())
				{
					SetTranslatingAxis(type, Axis.Any);
				}
			}
		}

		private float ClosestDistanceFromMouseToLines(List<Vector3> lines)
		{
			Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
			float num = float.MaxValue;
			for (int i = 0; i + 1 < lines.Count; i++)
			{
				IntersectPoints intersectPoints = Geometry.ClosestPointsOnSegmentToLine(lines[i], lines[i + 1], ray.origin, ray.direction);
				float num2 = Vector3.Distance(intersectPoints.first, intersectPoints.second);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		private float ClosestDistanceFromMouseToPlanes(List<Vector3> planePoints)
		{
			float num = float.MaxValue;
			if (planePoints.Count >= 4)
			{
				Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
				for (int i = 0; i < planePoints.Count; i += 4)
				{
					if (new Plane(planePoints[i], planePoints[i + 1], planePoints[i + 2]).Raycast(ray, out var enter))
					{
						Vector3 b = ray.origin + ray.direction * enter;
						float num2 = Vector3.Distance((planePoints[0] + planePoints[1] + planePoints[2] + planePoints[3]) / 4f, b);
						if (num2 < num)
						{
							num = num2;
						}
					}
				}
			}
			return num;
		}

		private void SetAxisInfo()
		{
			if (mainTargetRoot != null)
			{
				axisInfo.Set(mainTargetRoot, pivotPoint, GetProperTransformSpace());
			}
		}

		public float GetDistanceMultiplier()
		{
			if (mainTargetRoot == null)
			{
				return 0f;
			}
			if (myCamera.orthographic)
			{
				return Mathf.Max(0.01f, myCamera.orthographicSize * 2f);
			}
			return Mathf.Max(0.01f, Mathf.Abs(ExtVector3.MagnitudeInDirection(pivotPoint - base.transform.position, myCamera.transform.forward)));
		}

		private void SetLines()
		{
			SetHandleLines();
			SetHandlePlanes();
			SetHandleTriangles();
			SetCircles(GetAxisInfo(), circlesLines);
		}

		private void SetHandleLines()
		{
			handleLines.Clear();
			if (TranslatingTypeContains(TransformType.Move) || TranslatingTypeContains(TransformType.Scale))
			{
				float width = handleWidth * GetDistanceMultiplier();
				float length = 0f;
				float length2 = 0f;
				float length3 = 0f;
				if (TranslatingTypeContains(TransformType.Move))
				{
					length = (length2 = (length3 = GetHandleLength(TransformType.Move)));
				}
				else if (TranslatingTypeContains(TransformType.Scale))
				{
					length = GetHandleLength(TransformType.Scale, Axis.X);
					length2 = GetHandleLength(TransformType.Scale, Axis.Y);
					length3 = GetHandleLength(TransformType.Scale, Axis.Z);
				}
				AddQuads(pivotPoint, axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, length, width, handleLines.x);
				AddQuads(pivotPoint, axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, length2, width, handleLines.y);
				AddQuads(pivotPoint, axisInfo.zDirection, axisInfo.xDirection, axisInfo.yDirection, length3, width, handleLines.z);
			}
		}

		private int AxisDirectionMultiplier(Vector3 direction, Vector3 otherDirection)
		{
			if (!ExtVector3.IsInDirection(direction, otherDirection))
			{
				return -1;
			}
			return 1;
		}

		private void SetHandlePlanes()
		{
			handlePlanes.Clear();
			if (TranslatingTypeContains(TransformType.Move))
			{
				Vector3 rhs = myCamera.transform.position - pivotPoint;
				float num = Mathf.Sign(Vector3.Dot(axisInfo.xDirection, rhs));
				float num2 = Mathf.Sign(Vector3.Dot(axisInfo.yDirection, rhs));
				float num3 = Mathf.Sign(Vector3.Dot(axisInfo.zDirection, rhs));
				float num4 = planeSize;
				if (transformType == TransformType.All)
				{
					num4 *= allMoveHandleLengthMultiplier;
				}
				num4 *= GetDistanceMultiplier();
				Vector3 vector = axisInfo.xDirection * num4 * num;
				Vector3 vector2 = axisInfo.yDirection * num4 * num2;
				Vector3 vector3 = axisInfo.zDirection * num4 * num3;
				Vector3 axisStart = pivotPoint + (vector2 + vector3);
				Vector3 axisStart2 = pivotPoint + (vector + vector3);
				Vector3 axisStart3 = pivotPoint + (vector + vector2);
				AddQuad(axisStart, axisInfo.yDirection, axisInfo.zDirection, num4, handlePlanes.x);
				AddQuad(axisStart2, axisInfo.xDirection, axisInfo.zDirection, num4, handlePlanes.y);
				AddQuad(axisStart3, axisInfo.xDirection, axisInfo.yDirection, num4, handlePlanes.z);
			}
		}

		private void SetHandleTriangles()
		{
			handleTriangles.Clear();
			if (TranslatingTypeContains(TransformType.Move))
			{
				float size = triangleSize * GetDistanceMultiplier();
				AddTriangles(axisInfo.GetXAxisEnd(GetHandleLength(TransformType.Move)), axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, size, handleTriangles.x);
				AddTriangles(axisInfo.GetYAxisEnd(GetHandleLength(TransformType.Move)), axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, size, handleTriangles.y);
				AddTriangles(axisInfo.GetZAxisEnd(GetHandleLength(TransformType.Move)), axisInfo.zDirection, axisInfo.yDirection, axisInfo.xDirection, size, handleTriangles.z);
			}
		}

		private void AddTriangles(Vector3 axisEnd, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
		{
			Vector3 item = axisEnd + axisDirection * (size * 2f);
			Square baseSquare = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, size / 2f);
			resultsBuffer.Add(baseSquare.bottomLeft);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.topRight);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.bottomRight);
			resultsBuffer.Add(baseSquare.topRight);
			for (int i = 0; i < 4; i++)
			{
				resultsBuffer.Add(baseSquare[i]);
				resultsBuffer.Add(baseSquare[i + 1]);
				resultsBuffer.Add(item);
			}
		}

		private void AddSquares(Vector3 axisStart, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
		{
			AddQuads(axisStart, axisDirection, axisOtherDirection1, axisOtherDirection2, size, size * 0.5f, resultsBuffer);
		}

		private void AddQuads(Vector3 axisStart, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float length, float width, List<Vector3> resultsBuffer)
		{
			Vector3 axisEnd = axisStart + axisDirection * length;
			AddQuads(axisStart, axisEnd, axisOtherDirection1, axisOtherDirection2, width, resultsBuffer);
		}

		private void AddQuads(Vector3 axisStart, Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float width, List<Vector3> resultsBuffer)
		{
			Square baseSquare = GetBaseSquare(axisStart, axisOtherDirection1, axisOtherDirection2, width);
			Square baseSquare2 = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, width);
			resultsBuffer.Add(baseSquare.bottomLeft);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.topRight);
			resultsBuffer.Add(baseSquare.bottomRight);
			resultsBuffer.Add(baseSquare2.bottomLeft);
			resultsBuffer.Add(baseSquare2.topLeft);
			resultsBuffer.Add(baseSquare2.topRight);
			resultsBuffer.Add(baseSquare2.bottomRight);
			for (int i = 0; i < 4; i++)
			{
				resultsBuffer.Add(baseSquare[i]);
				resultsBuffer.Add(baseSquare2[i]);
				resultsBuffer.Add(baseSquare2[i + 1]);
				resultsBuffer.Add(baseSquare[i + 1]);
			}
		}

		private void AddQuad(Vector3 axisStart, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float width, List<Vector3> resultsBuffer)
		{
			Square baseSquare = GetBaseSquare(axisStart, axisOtherDirection1, axisOtherDirection2, width);
			resultsBuffer.Add(baseSquare.bottomLeft);
			resultsBuffer.Add(baseSquare.topLeft);
			resultsBuffer.Add(baseSquare.topRight);
			resultsBuffer.Add(baseSquare.bottomRight);
		}

		private Square GetBaseSquare(Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size)
		{
			Vector3 vector = axisOtherDirection1 * size + axisOtherDirection2 * size;
			Vector3 vector2 = axisOtherDirection1 * size - axisOtherDirection2 * size;
			Square result = default(Square);
			result.bottomLeft = axisEnd + vector2;
			result.topLeft = axisEnd + vector;
			result.bottomRight = axisEnd - vector;
			result.topRight = axisEnd - vector2;
			return result;
		}

		private void SetCircles(AxisInfo axisInfo, AxisVectors axisVectors)
		{
			axisVectors.Clear();
			if (TranslatingTypeContains(TransformType.Rotate))
			{
				float size = GetHandleLength(TransformType.Rotate);
				AddCircle(pivotPoint, axisInfo.xDirection, size, axisVectors.x);
				AddCircle(pivotPoint, axisInfo.yDirection, size, axisVectors.y);
				AddCircle(pivotPoint, axisInfo.zDirection, size, axisVectors.z);
				AddCircle(pivotPoint, (pivotPoint - base.transform.position).normalized, size, axisVectors.all, depthTest: false);
			}
		}

		private void AddCircle(Vector3 origin, Vector3 axisDirection, float size, List<Vector3> resultsBuffer, bool depthTest = true)
		{
			Vector3 vector = axisDirection.normalized * size;
			Vector3 rhs = Vector3.Slerp(vector, -vector, 0.5f);
			Vector3 vector2 = Vector3.Cross(vector, rhs).normalized * size;
			Matrix4x4 matrix4x = new Matrix4x4
			{
				[0] = vector2.x,
				[1] = vector2.y,
				[2] = vector2.z,
				[4] = vector.x,
				[5] = vector.y,
				[6] = vector.z,
				[8] = rhs.x,
				[9] = rhs.y,
				[10] = rhs.z
			};
			Vector3 vector3 = origin + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
			Vector3 vector4 = Vector3.zero;
			float num = 360f / (float)circleDetail;
			Plane plane = new Plane((base.transform.position - pivotPoint).normalized, pivotPoint);
			float width = handleWidth * GetDistanceMultiplier();
			for (int i = 0; i < circleDetail + 1; i++)
			{
				vector4.x = Mathf.Cos((float)i * num * ((float)Math.PI / 180f));
				vector4.z = Mathf.Sin((float)i * num * ((float)Math.PI / 180f));
				vector4.y = 0f;
				vector4 = origin + matrix4x.MultiplyPoint3x4(vector4);
				if (!depthTest || plane.GetSide(vector3))
				{
					Vector3 normalized = ((vector3 + vector4) * 0.5f - origin).normalized;
					AddQuads(vector3, vector4, normalized, axisDirection, width, resultsBuffer);
				}
				vector3 = vector4;
			}
		}

		private void DrawLines(List<Vector3> lines, Color color)
		{
			if (lines.Count != 0)
			{
				GL.Begin(1);
				GL.Color(color);
				for (int i = 0; i < lines.Count; i += 2)
				{
					GL.Vertex(lines[i]);
					GL.Vertex(lines[i + 1]);
				}
				GL.End();
			}
		}

		private void DrawTriangles(List<Vector3> lines, Color color)
		{
			if (lines.Count != 0)
			{
				GL.Begin(4);
				GL.Color(color);
				for (int i = 0; i < lines.Count; i += 3)
				{
					GL.Vertex(lines[i]);
					GL.Vertex(lines[i + 1]);
					GL.Vertex(lines[i + 2]);
				}
				GL.End();
			}
		}

		private void DrawQuads(List<Vector3> lines, Color color)
		{
			if (lines.Count != 0)
			{
				GL.Begin(7);
				GL.Color(color);
				for (int i = 0; i < lines.Count; i += 4)
				{
					GL.Vertex(lines[i]);
					GL.Vertex(lines[i + 1]);
					GL.Vertex(lines[i + 2]);
					GL.Vertex(lines[i + 3]);
				}
				GL.End();
			}
		}

		private void DrawFilledCircle(List<Vector3> lines, Color color)
		{
			if (lines.Count != 0)
			{
				Vector3 zero = Vector3.zero;
				for (int i = 0; i < lines.Count; i++)
				{
					zero += lines[i];
				}
				zero /= (float)lines.Count;
				GL.Begin(4);
				GL.Color(color);
				for (int j = 0; j + 1 < lines.Count; j++)
				{
					GL.Vertex(lines[j]);
					GL.Vertex(lines[j + 1]);
					GL.Vertex(zero);
				}
				GL.End();
			}
		}

		private bool IsPointerOverUIObject()
		{
			if (EventSystem.current == null)
			{
				return false;
			}
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			return list.Count > 1;
		}

		private void SetMaterial()
		{
			if (lineMaterial == null)
			{
				lineMaterial = new Material(Shader.Find("Custom/Lines"));
			}
		}
	}
}
