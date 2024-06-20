using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLine : MonoBehaviour
{
	public Vector2 scale;

	[Header("References")]
	public GameObject line;

	Vector3 targetPos;
	float xR;
	float zR;

	private void Start()
	{
		SetPos(transform.position);
		xR = 0.1f + Random.value;
		zR = 0.1f + Random.value;
	}

	private void Update()
	{
		// WaterLine
		float x = Mathf.LerpUnclamped(scale.x, scale.y, 1 + (Mathf.Sin(Time.time * xR) / 2));
		float z = Mathf.LerpUnclamped(scale.x, scale.y, 1 + (Mathf.Sin(Time.time * zR) / 2));
		transform.localScale = new Vector3(x, 0, z);
		transform.position = targetPos;
	}

	public void SetPos(Vector3 pos)
	{
		targetPos = pos;
	}
}