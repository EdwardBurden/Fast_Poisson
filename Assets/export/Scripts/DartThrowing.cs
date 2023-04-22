using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DartThrowing : MonoBehaviour
{
	public float length;
	public GameObject prefab;
	public GameObject plane;
	public int maxPoints = 100;
	public int addAmount = 10;
	public float waitTime = 0.1f;
	public float minDistance = 5;

	private List<Vector3> points = new List<Vector3>();

	private void Awake()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		int count = 0;
		while (count < maxPoints)
		{
			PlacePoint();
			count++;
		}
		stopwatch.Stop();
		Debug.Log(stopwatch.ElapsedMilliseconds +"  " +points.Count);
	}

	public void PlacePoint()
	{

		float x = Random.Range(0, length);
		float z = Random.Range(0, length);
		Vector3 pos = new Vector3(x, plane.transform.position.y, z) - (new Vector3(length, 0, length) / 2);

		foreach (var item in points)
		{
			if (Vector3.Distance(pos, item) < minDistance)
			{
				return;
			}
		}


		Instantiate(prefab, pos, Quaternion.identity, this.transform);
		points.Add(pos);

	}

}
