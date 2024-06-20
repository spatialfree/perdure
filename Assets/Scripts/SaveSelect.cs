using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSelect : MonoBehaviour
{
    public Save saveData;
    [HideInInspector]
    public Menu menu;

    [Header("References")]
    public Button load;
    public TextMeshProUGUI saveName;

    private void Start()
    {
        saveName.text = saveData.name;
        menu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>();

        load.onClick.AddListener(delegate {menu.LoadSave(saveData.name); });
    }

    public void DeleteSave()
    {
		File.Delete(Application.persistentDataPath + "/" + saveData.name + ".spire");

        RefreshEditorProjectWindow();

        Destroy(this.gameObject);
    }

    void RefreshEditorProjectWindow() 
    {
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
} 