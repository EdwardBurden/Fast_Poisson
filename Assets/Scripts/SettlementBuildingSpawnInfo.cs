using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SettlemetBuildingSpawnData
{
	public GameObject buildingPrefab;
	public float spawnChance; //0.2 , 0.6 0.9
}

[Serializable]
public class SettlementBuildings
{
	public SettlemetBuildingSpawnData[] buildings;
	public float minRadius;
	public float maxRadius;


	public SettlemetBuildingSpawnData SelectRandomBuilding()
	{
		float totalChance = 0;
		for (int i = 0; i < buildings.Length; i++)
		{
			totalChance += buildings[i].spawnChance;
		}
		while (true)
		{
			int buildingIndex = Random.Range(0, buildings.Length);
			float chance = buildings[buildingIndex].spawnChance / totalChance;
			Random.Range(0, 1);
			if (chance > Random.Range(0.0f, 1.0f))
			{
				return buildings[buildingIndex];
			}
		}
	}
}
