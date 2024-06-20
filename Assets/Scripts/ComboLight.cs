using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboLight : MonoBehaviour
{
	public float dur = 1;

	float time;

	private void Start()
	{
		time = dur;
		transform.localScale = Vector3.one * 1.1f;
	}

	private void Update()
	{
		time -= Time.deltaTime;
		transform.localScale = Vector3.one * (dur - time) * (dur - time);

		if (time <= 0)
		{
			Destroy(this.gameObject);
		}
	}
}