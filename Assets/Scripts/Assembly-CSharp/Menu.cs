using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public GameObject LoadPanel;

	public void GoToMenu()
	{
		SceneManager.LoadScene("Menu");
	}

	public void GoToMars()
	{
		SceneManager.LoadScene("Mars");
	}

	public void GoToBase()
	{
		SceneManager.LoadScene("Base");
	}

	public void GoToFlatGrass()
	{
		SceneManager.LoadScene("FlatGrass");
	}

	public void GoToSpace()
	{
		SceneManager.LoadScene("Space");
	}

	public void GoToDesert()
	{
		SceneManager.LoadScene("Desert");
	}

	public void GoToCity()
	{
		SceneManager.LoadScene("City");
	}

	public void GoToBaseV2()
	{
		SceneManager.LoadScene("BaseV2");
	}

	public void LoadSavePanel()
	{
		LoadPanel.SetActive(value: true);
	}

	public void CloseSavePanel()
	{
		LoadPanel.SetActive(value: false);
	}
}
