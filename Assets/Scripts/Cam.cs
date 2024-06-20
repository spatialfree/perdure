using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Cam : MonoBehaviour
{
	public Vector2 zoomStr;
	public float shakeAngle = 5;
	public float shakeIntensity = 10;
	[HideInInspector]
	public float screenShake;

	[Header("References")]
	public Camera cam;
	public Spire spire;
	public Transform mount;
	public Slider slider;
	public Slider layerSlider;
	public TextMeshPro layerInfo;

	float noiseTime;
	float hitStop;
	float zoomTo = 0;
	int blockIndex = 1;
	int layerIndex = 1;
	float layerTime;
	float speedTime;

	private void Update()
	{
		//float zoomTo = Convert.ToInt32(Input.GetKey("z"));

		if (!Input.GetButton("Fire1"))
		{
			if (slider.value > 0.5f)
			{
				slider.value += Time.deltaTime * 3;
			}

			if (slider.value < 0.5f)
			{
				slider.value -= Time.deltaTime * 3;
			}

			if (layerSlider.value > 0 || layerSlider.value < 0)
			{
				layerSlider.value -= Time.deltaTime * 12 * layerSlider.value;
			}
		}

		zoomTo = slider.value;

		layerSlider.gameObject.SetActive(zoomTo == 1);

		transform.localEulerAngles = Vector3.Lerp(new Vector3(5, 45, 0), Vector3.up * 45, zoomTo);

		float zoom = Mathf.Lerp(zoomStr.x, zoomStr.y, zoomTo);

		mount.localPosition = new Vector3(1 * zoomTo, 0, Mathf.Lerp(mount.localPosition.z, zoom, Time.deltaTime * 6));

		float speed = 0.5f;
		Vector3 targetPos = Vector3.up * spire.blockRef.Count;
		if (zoomTo == 1)
		{
			Block block = spire.blockRef[spire.blockRef.Count - blockIndex];
			LayerData layer = block.layerData[block.layerData.Count - layerIndex];
			float yOffset = layer.posY;

			int layerNumber = 0;
			for (int l = 0; l < (spire.blockRef.Count - blockIndex + 1); l++)
			{
				layerNumber += spire.blockRef[l].layerData.Count;
			}

			layerNumber -= (layerIndex - 1);

			layerInfo.text = "-> " + layer.day.Date.ToShortDateString() + " #" + layerNumber;
			
			targetPos = Vector3.up * (yOffset + (spire.blockRef.Count - blockIndex));

			if (layerSlider.value == -1 || layerSlider.value == 1)
			{
				speedTime -= Time.deltaTime;
			}
			else
			{
				speedTime += Time.deltaTime;
			}

			speedTime = Mathf.Clamp(speedTime, 0.1f, 1f);

			if (Time.time > layerTime)
			{
				if (layerSlider.value == -1)
				{
					layerIndex++;
					if (layerIndex > block.layerData.Count)
					{
						layerIndex = 1;
						blockIndex++;
						if (blockIndex > spire.blockRef.Count)
						{
							blockIndex = 1;
						}
					}

				}
				else if (layerSlider.value == 1)
				{
					layerIndex--;
					if (layerIndex < 1)
					{
						blockIndex--;
						if (blockIndex < 1)
						{
							blockIndex = spire.blockRef.Count;
						}
						layerIndex = spire.blockRef[spire.blockRef.Count - blockIndex].layerData.Count;
					}
				}

				layerTime = Time.time + (0.2f * speedTime);
			}
			//layerTarget

			speed = 2;
		}

		layerInfo.transform.position = new Vector3(0.88f, targetPos.y, 0);
		layerInfo.gameObject.SetActive(zoomTo == 1);


		if (transform.position.y < targetPos.y)
		{
			speed = 3;
		}
		transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

		if (hitStop <= 0 && screenShake >= 0.5f)
		{
			hitStop = screenShake / 10;
		}
		screenShake -= Time.deltaTime;
		screenShake = Mathf.Clamp01(screenShake);

		noiseTime += Time.deltaTime * screenShake * screenShake * shakeIntensity;

		Vector3 noise;
		noise.x = 0.5f - Mathf.PerlinNoise(noiseTime, 0.5f);
		noise.x *= noise.x;
		noise.y = 0.5f - Mathf.PerlinNoise(noiseTime, 10.5f);
		noise.y *= noise.y;
		noise.z = 0.5f - Mathf.PerlinNoise(noiseTime, 20.5f);
		noise.z *= noise.z;
		cam.transform.localPosition = noise * screenShake * screenShake * shakeAngle; // exponential


		if (hitStop > 0)
		{
			hitStop -= Time.unscaledDeltaTime;
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}
}