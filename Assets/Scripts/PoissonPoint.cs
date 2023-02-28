using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonPoint
{
	public Vector2 pos;
	public float radius;

	public bool tested = false;

	public PoissonPoint(Vector2 pos, float radius)
	{
		this.pos = pos;
		this.radius = radius;
		tested = false;
	}
}
