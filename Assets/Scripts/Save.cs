using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public System.DateTime dateFounded;
    public string name;
    public List<List<LayerData>> spireData = new List<List<LayerData>>(); // Spire data
    public float hue = 0.5f;
    public int startWindow;
    public int windowDur;
    public System.DateTime lastPlaced;
    public System.DateTime lastWindow;
    public bool locked = false;
    public bool notifSent = false;
    // dev tools
    public bool simTime = false;
    public double simSeconds = 0;
}