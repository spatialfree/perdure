using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
	public float dur = 0.5f;

	float t = 0;

	private void Start()
	{
		transform.localScale = Vector3.zero;
	}

	private void Update()
	{
		transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

		t += Time.deltaTime / 0.5f;
		transform.localScale = Vector3.one * t * t;// exponential

		if (t >= 1)
		{
			Destroy(this.gameObject);
		}
	}
}