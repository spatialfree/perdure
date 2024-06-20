using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.SimpleAndroidNotifications;
using System;

public class Spire : MonoBehaviour
{
    public List<int> blocks;

    [Header("References")]
    public GameObject block;
    public Material blockMat;
    public GameObject addBlockButton;
    public Slider zoomSlider;
    public GameObject spawnLight;
    public GameObject comboLight;
    public GameObject shockwave;
    public ParticleSystem gravel;
    public AudioSource gravelSFX;
    public GameObject ripple;
    public Cam cam;
    public MeshFilter cap;
    public Save saveData;

    public List<Block> blockRef = new List<Block>();

    IEnumerator coroutine;
    Transform smoosh;
    float t;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        foreach(Block block in blockRef)
        {
            Destroy(block.gameObject);
        }
        blockRef.Clear();

        blockMat.color = Color.HSVToRGB(saveData.hue, 1, 1);
        if (saveData.hue == 0.5f)
            blockMat.color = Color.HSVToRGB(saveData.hue, 0, 1);

        foreach (List<LayerData> layer in saveData.spireData)
        {
            GameObject newBlock = Instantiate(block, Vector3.up * blockRef.Count, Quaternion.identity, this.transform);
            blockRef.Add(newBlock.GetComponent<Block>());
            TopBlock().layerData = layer;
            TopBlock().BlockMesh();
        }


        // bleach current top layer
        double fullDay = 24 * 60 * 60;
        if (blockRef.Count > 0)
        {
            double exposedTime = (NowSim() - saveData.lastPlaced).TotalSeconds;
            double maxTime = fullDay * 3;
            TopBlock().layerData[TopBlock().layerData.Count - 1].value = (float)exposedTime / (float)maxTime;
            TopBlock().BlockMesh();
        }
        
        Cap();

        // Locking System
        // currentTime > timeWindow = locked; < worry about this later

        
        if (saveData.locked)
        {
            // if now is later than daylate lastopened 
            DateTime dayLater = saveData.lastWindow.AddSeconds(fullDay);
            if (DateTime.Compare(NowSim(), dayLater) > 0)
            {
                saveData.locked = false;
                saveData.notifSent = false;
            }
        }

        addBlockButton.SetActive(!saveData.locked);

        // Notification
        if (!saveData.notifSent)
        {
            double startSeconds = saveData.startWindow * 60 * 60;
            double currentSeconds = NowSim().TimeOfDay.TotalSeconds;
            double secondsTillOpen = fullDay - (currentSeconds - startSeconds);
        
            string message = saveData.windowDur + "hr window to update your spire";
            NotificationIcon icon = NotificationIcon.Bell;
            NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(secondsTillOpen), saveData.name, message, Color.HSVToRGB(saveData.hue, 0.5f, 0.5f), icon);
            saveData.notifSent = true;
        }
    }

    DateTime NowSim()
    {
        return DateTime.Now.AddSeconds(saveData.simSeconds);
    }

    private void Update()
    {
        if (smoosh != null)
        {
            t += Time.deltaTime * 2;
            smoosh.localScale = Vector3.Lerp(smoosh.localScale, Vector3.one, t * t);
            smoosh.position = Vector3.up * (blockRef.Count + (smoosh.localScale.y / 2) - 1.5f);

            cap.transform.position = smoosh.transform.position + (Vector3.up * (smoosh.localScale.y / 2));

            if (t >= 1)
            {
                t = 0;
                smoosh = null;
            }
        }

        if (Input.GetKey("space"))
        {
            AddBlock();
        }
    }

    public void AddBlock()
    {
        if (coroutine == null && !saveData.locked)
        {
            coroutine = BlockTransition();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator BlockTransition()
    {
        saveData.locked = true;
        addBlockButton.SetActive(!saveData.locked);

        Instantiate(spawnLight, Vector3.up * blockRef.Count, Quaternion.identity, this.transform);
        yield return new WaitForSeconds(0.5f);
        GameObject newBlock = Instantiate(block, Vector3.up * blockRef.Count, Quaternion.identity, this.transform);
        blockRef.Add(newBlock.GetComponent<Block>());
        TopBlock().layerData.Add(new LayerData());
        TopBlock().layerData[0].day = System.DateTime.Today;
        TopBlock().layerData[0].value = 0; // 0-1 for convenience remapped for visuals
        TopBlock().BlockMesh();
        Juice();
        Cap();
        
        yield return new WaitForSeconds(0.5f);

        // check for each type of combo
        for (int t = 0; t < 5; t++)
        {
            List<Block> comboThese = new List<Block>();
            foreach (Block b in blockRef)
            {
                if (b.layerData.Count == blocks[t])
                {
                    comboThese.Add(b);
                }
            }

            if (comboThese.Count >= 4)
            {
                // Juice!
                // highlight all 4 and smoosh them together
                // sfx screenshake and don't forget the kitchen sink ^-^
                while (comboThese.Count > 1)
                {
                    foreach (LayerData data in comboThese[1].layerData)
                    {
                        comboThese[0].layerData.Add(data);
                    }
                    Destroy(comboThese[1].gameObject);
                    blockRef.Remove(comboThese[1]);
                    comboThese.Remove(comboThese[1]);
                }

                comboThese[0].BlockMesh();
                comboThese[0].transform.localScale = new Vector3(1, 4, 1);
                comboThese[0].transform.position += Vector3.up * 1.5f;
                t = 0;
                smoosh = comboThese[0].transform;
                // move and scale back down (exponential)
                yield return new WaitForSeconds(0.5f);
                Juice();
            }
        }

        double startSeconds = saveData.startWindow * 60 * 60;
        double currentSeconds = NowSim().TimeOfDay.TotalSeconds;
        saveData.lastWindow = NowSim().AddSeconds(-(currentSeconds - startSeconds));
        saveData.lastPlaced = NowSim();

        Cap();
        QuickSave();

        coroutine = null;

        if (saveData.simTime)
        {
            while (saveData.locked)
            {
                saveData.simSeconds += (24 * 60 * 60) + ((0.5f - UnityEngine.Random.value) * 60 * 60 * 2);
                Setup();
                QuickSave();
            }
        }
    }

    private void Juice()
    {
        zoomSlider.value = 0;
        gravelSFX.pitch = 0.75f + (UnityEngine.Random.value / 2);
        gravelSFX.volume = 0.75f + UnityEngine.Random.value;
        gravelSFX.Play();
        cam.screenShake += 0.5f;

        Instantiate(ripple, Vector3.down / 2, Quaternion.identity, this.transform);
        Instantiate(shockwave, Vector3.up * blockRef.Count, Quaternion.identity, this.transform);

        gravel.transform.position = Vector3.up * ((float)blockRef.Count - 1.5f);
        gravel.Play();
    }

    private void Cap()
    {
        if (blockRef.Count == 0)
        {
            cap.transform.position = Vector3.down;
            return;
        }
        // set cap colors based on top layer 
        List<Color> capColors = new List<Color>();

        for (int c = 0; c < 4; c++)
		{
            int topLayerIndex = TopBlock().layerData.Count - 1;
            float value = TopBlock().layerData[topLayerIndex].value;
			capColors.Add(Color.white * (value + (((1 - value) * 0.25f))));
		}

        cap.mesh.colors = capColors.ToArray();
        cap.transform.position = Vector3.up * ((float)blockRef.Count - 0.5f);
    }

    public void QuickSave()
	{
        saveData.spireData.Clear();
        foreach (Block block in blockRef)
        {
            saveData.spireData.Add(block.layerData);
        }

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + saveData.name + ".spire");
		bf.Serialize(file, saveData);
		file.Close();
	}

    Block TopBlock()
    {
        Block topBlock = blockRef[blockRef.Count - 1];
        return topBlock;
    }
}