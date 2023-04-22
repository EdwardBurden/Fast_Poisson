using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonCell
{
	public int x;
	public int z;
	public PoissonPoint pointInCell; //point currently in cell
	public List<PoissonPoint> pushedPoints; //points that have test distance within this cell

	public PoissonCell(int x, int z)
	{
		this.x = x;
		this.z = z;
		pushedPoints = new List<PoissonPoint>();
		pointInCell = null;

	}
}
