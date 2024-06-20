using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
	public bool clearSaves;

	[Header("References")]
	public Spire spire;

	public GameObject mainMenu;
	public GameObject newMenu;
	public TMP_InputField spireName;
	public GameObject saveSelect;
	public GameObject saveList;
	public GameObject editMenu;
	public TextMeshProUGUI editTitle;
	public Toggle simTime;

	Save newSave = new Save();

	private void Awake()
	{
		UnityEngine.Debug.Log(Application.persistentDataPath);
		if (clearSaves)
		{
			foreach (string filePath in System.IO.Directory.GetFiles(Application.persistentDataPath + "/"))
			{
				File.Delete(filePath);
			}
		}
		spire.enabled = false;
		mainMenu.SetActive(true);
		LoadSaveList();
		newMenu.SetActive(false);
		editMenu.SetActive(false);
	}

	private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
			if (false)
			{
				// exit layerview mode and return to the spire
			}
			else
			{
            SceneManager.LoadScene(0);
				
			}
        }
    }

	public void NewSave()
	{
		mainMenu.SetActive(false);
		newMenu.SetActive(true);
	}

	public void NewSpire()
	{
		if (spireName.text=="")
		{
			Debug.Log("Field is empty");
			return;
		}

		newSave.dateFounded = System.DateTime.Today;
		newSave.name = spireName.text;
		newSave.simTime = simTime.isOn;

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + spireName.text + ".spire");
		bf.Serialize(file, newSave);
		file.Close();

		newMenu.SetActive(false);

		LoadSave(spireName.text);
	}

	public void LoadSaveList()
	{
		// Pull daved data and instantiate Chain Save UI
		Vector3 pos = Vector3.zero;

		foreach (string filePath in System.IO.Directory.GetFiles(Application.persistentDataPath + "/"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(filePath, FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			pos += Vector3.down * 50;
			GameObject chainSaveInst = Instantiate(saveSelect, pos, Quaternion.identity, saveList.transform);
			chainSaveInst.transform.localPosition = pos;
			chainSaveInst.GetComponent<SaveSelect>().saveData = save;
		}
	}

	public void LoadSave(string saveName)
	{
		if (File.Exists(Application.persistentDataPath + "/" + saveName + ".spire"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + saveName + ".spire", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			// pass the save data onto the runtime components
			spire.saveData = save;
			spire.enabled = true;
			mainMenu.SetActive(false);
			editTitle.text = saveName;
			editMenu.SetActive(true);
		}
		else
		{
			Debug.Log("No chain saved!");
		}

	}

	public void StartWindow(TextMeshProUGUI valueText)
	{
		// convert to 12 hr time (am pm)
		int hr = (int)valueText.GetComponentInParent<Slider>().value;
		newSave.startWindow = hr;
		if (hr > 12)
			hr -= 12;

		if (hr == 0)
			hr = 12;
		
		valueText.text = hr + " am";
		if (valueText.GetComponentInParent<Slider>().value > 11)
		valueText.text = hr + " pm";
	}

	public void TimeWindow(TextMeshProUGUI valueText)
	{
		int hrs = (int)valueText.GetComponentInParent<Slider>().value;
		newSave.windowDur = hrs;
		valueText.text = hrs + " hrs";
	}

	public void ColorSlider(Slider slider)
	{
		Color color = Color.HSVToRGB(slider.value, 0.5f, 0.5f);
		newSave.hue = slider.value;
		newMenu.GetComponent<Image>().color = color;
	}
}