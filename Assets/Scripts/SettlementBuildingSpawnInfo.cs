using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SettlementBuildingSpawnInfo
{
	public GameObject buildingPrefab;
	public float spawnChance; //0.2 , 0.6 0.9
}

[Serializable]
public class SettlemenSpawnInfo
{
	public SettlementBuildingSpawnInfo[] buildings;
	public float radius;

	public GameObject SelectRandomBuilding()
	{
		return buildings[Random.Range(0, buildings.Length)].buildingPrefab;
	}
}
