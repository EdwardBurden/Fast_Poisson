using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
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
	private Dictionary<PoissonPoint, GameObject> buildingsSpawned = new Dictionary<PoissonPoint, GameObject>();
	private int generationlevel = 0;

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
		buildingsSpawned.Add(start, AddBuildingPrefab(start));

		int count = 0;
		while (count < generationSettings.maxPoints)
		{
			yield return new WaitForSeconds(waitTime);
			for (int i = 0; i < addAmount; i++)
			{
				TryAddBuilding();
				count++;
			}
			yield return new WaitForSeconds(waitTime);
			for (int i = 0; i < overrideAmount; i++)
			{
				TryReplaceBuilding();
			}
			//StaticBatchingUtility.Combine(this.transform.gameObject);
		}
		StaticBatchingUtility.Combine(this.transform.gameObject);
	}

	private void TryReplaceBuilding() //TODO
	{
		//Select point , do last point for now
		KeyValuePair<PoissonPoint, GameObject> keyValuePair = buildingsSpawned.TakeLast(addAmount).FirstOrDefault();
		//tell grid to remove point
		poissonGrid.RemovePoint(keyValuePair.Key);
		Destroy(buildingsSpawned[keyValuePair.Key]);
		buildingsSpawned.Remove(keyValuePair.Key);
		//sample radius at postion , default to maxbuildingradius for now
		float sampledRadius = generationSettings.maxBuildingRadius/2; //generationSettings.SampleRadius(keyValuePair.Key.pos, transform, perlinSeed);
																	//remove any points overlapping
		List<PoissonPoint> removed = poissonGrid.RemovePointsOverlapping(keyValuePair.Key.pos, sampledRadius);
		foreach (var item in removed)
		{
			Destroy(buildingsSpawned[item]);
			buildingsSpawned.Remove(item);
		}
		//place point as new untested point
		PoissonPoint poissonPoint = poissonGrid.AddPoint(keyValuePair.Key.pos, sampledRadius);
		buildingsSpawned.Add(poissonPoint, AddBuildingPrefab(poissonPoint));
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
		buildingsSpawned.Add(point, AddBuildingPrefab(point));
	}
	private GameObject AddBuildingPrefab(PoissonPoint point)
	{
		GameObject building = null;
		Vector3 world = point.pos.ToVector3() - generationSettings.offset + this.transform.position;
		if (Physics.Raycast(world + (Vector3.up * 10), Vector3.down, out RaycastHit hit, 100))
		{
			int angleamount = 16;
			float y = Random.Range(0, angleamount + 1) * (360 / angleamount);
			Quaternion rotation = Quaternion.Euler(0, y, 0);
			building = Instantiate(FindBuildingPrefab(point), hit.point, rotation, this.transform);
		}
		return building;
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
		if (poissonGrid == null)
		{
			return;
		}
		List<PoissonPoint> spawnedPoints = poissonGrid.spawnedPoints;
		if (spawnedPoints == null)
		{
			return;
		}
		foreach (PoissonPoint localPoint in spawnedPoints)
		{
			Gizmos.color = Color.red;
			//	Gizmos.DrawSphere(new Vector3(localPoint.pos.x, 0, localPoint.pos.y), localPoint.radius);
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(new Vector3(localPoint.pos.x - generationSettings.offset.x + this.transform.position.x, 0, localPoint.pos.y - generationSettings.offset.z + this.transform.position.z), localPoint.radius / 2);
		}
	}

}
