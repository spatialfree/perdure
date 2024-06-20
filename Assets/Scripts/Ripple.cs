using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : MonoBehaviour
{
	public float dur;
	public float maxScale = 10;

	[Header("References")]
	public LineRenderer line;

	float delay;

	private void Start()
	{
		delay = Time.time + dur;
	}

	private void Update()
	{
		float t = (delay - Time.time) / dur;
		transform.localScale = Vector3.one * Mathf.Lerp(maxScale, 0.5f, t * t);
		line.widthMultiplier = t * 0.14f;

		if (t <= 0)
			Destroy(this.gameObject);

	}
}