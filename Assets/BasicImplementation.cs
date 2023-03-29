using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BasicImplementation : MonoBehaviour
{
	public float length;
	public GameObject prefab;
	public GameObject plane;
	public int maxPoints = 100;
	public int addAmount = 10;
	public float waitTime = 0.1f;

	private void Awake()
	{
		Stopwatch stopwatch = new Stopwatch();
		StartCoroutine(Temp());
	}

	private IEnumerator Temp() //TEMP
	{
		int count = 0;
		while (count < maxPoints)
		{
			yield return new WaitForSeconds(waitTime);
			for (int i = 0; i < addAmount; i++)
			{
				PlacePoint();
				count++;
			}
		}
	}

	public void PlacePoint()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		float x = Random.Range(0, length);
		float z = Random.Range(0, length);
		Vector3 pos = new Vector3(x, plane.transform.position.y, z) - (new Vector3(length, 0, length) / 2);
		Instantiate(prefab, pos, Quaternion.identity, this.transform);
		stopwatch.Stop();
		Debug.Log(stopwatch.ElapsedMilliseconds);
	}
}
