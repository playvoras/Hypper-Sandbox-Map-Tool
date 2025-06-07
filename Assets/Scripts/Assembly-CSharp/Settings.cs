using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	public Toggle shadowsToggle;

	public Dropdown graphicsDropdown;

	public GameObject settingsPanel;

	public Slider fovSlider;

	public Camera myCamera;

	public GameObject[] spawnCubes;

	public Toggle toggleCubes;

	private void Start()
	{
		shadowsToggle.onValueChanged.AddListener(ToggleShadows);
		graphicsDropdown.onValueChanged.AddListener(ChangeGraphics);
		fovSlider.onValueChanged.AddListener(ChangeFOV);
		toggleCubes.onValueChanged.AddListener(ToggleChanged);
		spawnCubes = GameObject.FindGameObjectsWithTag("spawncube");
	}

	public void OpenSett()
	{
		if (!settingsPanel.activeInHierarchy)
		{
			settingsPanel.SetActive(value: true);
		}
		else
		{
			settingsPanel.SetActive(value: false);
		}
	}

	private void ToggleChanged(bool isOn)
	{
		GameObject[] array = spawnCubes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(isOn);
		}
	}

	private void ToggleShadows(bool toggle)
	{
		QualitySettings.shadowCascades = (toggle ? 2 : 0);
	}

	private void ChangeGraphics(int index)
	{
		QualitySettings.SetQualityLevel(index, applyExpensiveChanges: true);
	}

	private void ChangeFOV(float value)
	{
		myCamera.fieldOfView = value;
	}
}
