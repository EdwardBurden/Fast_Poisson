using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonPoint
{
	public Vector2 pos;
	public float radius;

	public bool usedForStart = false;

	public PoissonPoint(Vector2 pos, float radius)
	{
		this.pos = pos;
		this.radius = radius;
		usedForStart = false;
	}
}
