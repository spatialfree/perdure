using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLight : MonoBehaviour
{
	public float duration = 0.5f;

	float dur = 1;
	float time;

	private void Start()
	{
		time = dur;
		transform.localScale = Vector3.zero;
	}

	private void Update()
	{
		time -= Time.deltaTime / duration;
		transform.localScale = Vector3.one * (dur - time) * (dur - time);

		if (time <= 0)
		{
			Destroy(this.gameObject);
		}
	}
}