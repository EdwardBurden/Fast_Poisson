using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPoint 
{
	public PoissonPoint poissonPoint;
	public GameObject building;
	public int buildingLevel;
	public float distanceToCentre;

	public BuildingPoint(PoissonPoint poissonPoint, GameObject building)
	{
		this.poissonPoint = poissonPoint;
		this.building = building;
		buildingLevel = 0;
	}

	public BuildingPoint(PoissonPoint poissonPoint, GameObject building , int level)
	{
		this.poissonPoint = poissonPoint;
		this.building = building;
		buildingLevel = level;
	}
}
