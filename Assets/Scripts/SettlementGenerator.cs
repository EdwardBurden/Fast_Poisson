using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class SettlementGenerator : MonoBehaviour
{
	[SerializeField] private SettlementGenerationSettings generationSettings;
	public float waitTime = 0.05f; //debug
	public int addAmount = 5; //debug
	public int overrideAmount = 1; //debug

	private PoissonGrid poissonGrid;
	private float perlinSeed;
	//HANDLE GAMEOBJECT SIDE THINGS HERE


	private void Awake()
	{
		poissonGrid = GetComponent<PoissonGrid>();
		perlinSeed = Random.Range(0.0f, 10000f);
		poissonGrid.CreateGrid(generationSettings.regionSize, generationSettings.minBuildingRadius);
		StartCoroutine(Temp());
	}

	private IEnumerator Temp() //TEMP
	{
		float sampledRadius = generationSettings.SampleRadius((generationSettings.regionSize / 2), transform, perlinSeed);
		PoissonPoint start = poissonGrid.AddPoint(generationSettings.regionSize / 2, sampledRadius);
		AddBuildingPrefab(start);
		while (true)
		{
			yield return new WaitForSeconds(waitTime);
			for (int i = 0; i < addAmount; i++)
			{
				TryAddBuilding();
			}
			for (int i = 0; i < overrideAmount; i++)
			{
				TryReplaceBuilding();
			}
		}

	}

	private void TryReplaceBuilding() //TODO
	{


	}

	private void TryAddBuilding() //Clean
	{

		int attempts = 0;
		PoissonPoint point = TryCreatePoint();
		while (point == null && attempts < 10)
		{
			point = TryCreatePoint();
			attempts++;
		}
		if (point == null)
		{
			return;
		}
		AddBuildingPrefab(point);

	}
	private void AddBuildingPrefab(PoissonPoint point)
	{
		Vector3 world = point.pos.ToVector3() - generationSettings.offset + this.transform.position;
		int angleamount = 16;
		float y = Random.Range(0, angleamount + 1) * (360 / angleamount);
		Quaternion rotation = Quaternion.Euler(0, y, 0);
		GameObject gg = Instantiate(FindBuildingPrefab(point), world, rotation, this.transform);
		gg.transform.localScale += new Vector3(0, point.radius, 0);

	}


	private PoissonPoint TryCreatePoint()
	{
		PoissonPoint start = GetNextStartingPoint();
		for (int i = 0; i < generationSettings.rejectionSamples; i++)
		{
			float angle = Random.value * Mathf.PI * 2;
			Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
			Vector2 candidate = start.pos + direction * Random.Range(start.radius, 2 * start.radius);
			float sampledRadius = generationSettings.SampleRadius(candidate, this.transform, perlinSeed);
			if (!poissonGrid.CanPlacePoint(candidate, sampledRadius))
			{
				continue;
			}
			return poissonGrid.AddPoint(candidate, sampledRadius);
		}
		start.tested = true;
		return null;
	}

	private PoissonPoint GetNextStartingPoint()
	{
		PoissonPoint startPoint;
		List<PoissonPoint> untested = poissonGrid.FindUntestedPoints();
		if (untested.Count > 0)
		{
			startPoint = untested[0];
			//startPoint = untested[Random.Range(0, untested.Count)];
		}
		else
		{
			Debug.Log("Try From Start");
			float sampledRadius = generationSettings.SampleRadius((generationSettings.regionSize / 2), transform, perlinSeed);
			startPoint = new PoissonPoint(generationSettings.regionSize / 2, sampledRadius);
		}
		return startPoint;

	}

	private void Clear() //TODO
	{
	}

	private GameObject FindBuildingPrefab(PoissonPoint poissonPoint) //TEMP
	{
		return generationSettings.settlemenSpawnInfos[0].buildings[0].buildingPrefab;
		SettlemenSpawnInfo buildingsFiltered = generationSettings.settlemenSpawnInfos.OrderBy(x => Mathf.Abs(x.radius - poissonPoint.radius)).FirstOrDefault();
		return buildingsFiltered.SelectRandomBuilding();
	}



	private void OnDrawGizmos()
	{
		/*	Gizmos.color = Color.yellow;
			Gizmos.DrawCube(this.transform.position, new Vector3(regionExtent, 0, regionExtent));

			if (poissonCells != null)
			{
				for (int i = 0; i < poissonCells.GetLength(0); i++)
				{
					for (int j = 0; j < poissonCells.GetLength(1); j++)
					{
						if (poissonCells[i, j].pointInCell != null)
						{
							Gizmos.color = Color.red;
							Gizmos.DrawCube(new Vector3(poissonCells[i, j].x, poissonCells[i, j].pushedPoints.Count, poissonCells[i, j].z), Vector3.one * 0.4f);
							//Gizmos.DrawSphere(new Vector3(poissonCells[i, j].pointInCell.pos.x, 0, poissonCells[i, j].pointInCell.pos.y), 0.1f);
							continue;
						}

						Gizmos.color = Color.yellow;
						if (poissonCells[i, j].pushedPoints.Count > 0)
						{
							Gizmos.color = Color.green;
						}
						Gizmos.DrawCube(new Vector3(poissonCells[i, j].x, poissonCells[i, j].pushedPoints.Count, poissonCells[i, j].z), Vector3.one * 1f);
					}
				}
			}

			//Gizmos.color = Color.yellow;
			//Gizmos.DrawCube(this.transform.position, new Vector3(settlementRadius, 0, settlementRadius));


			if (spawnedPoints == null)
			{
				return;
			}
			foreach (PoissonPoint localPoint in spawnedPoints)
			{
				Gizmos.color = Color.red;
				//	Gizmos.DrawSphere(new Vector3(localPoint.pos.x, 0, localPoint.pos.y), localPoint.radius);
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(new Vector3(localPoint.pos.x, 0, localPoint.pos.y), localPoint.radius / 2);
			}*/
	}

}
