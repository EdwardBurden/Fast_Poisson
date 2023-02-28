using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public static class PoissonSampling
	{
		public static bool DoesPointOverlapExistingPoint(PoissonCell[,] cells, Vector2 point, float minRadius, float radius)
		{
			float cellSize = minRadius / Mathf.Sqrt(2);
			int cellX = (int)(point.x / cellSize);
			int cellY = (int)(point.y / cellSize);
			List<PoissonPoint> pushedPoints = cells[cellX, cellY].pushedPoints;
			for (int i = 0; i < pushedPoints.Count; i++)
			{
				float sqrDst = (point - pushedPoints[i].pos).sqrMagnitude;

				//check agian that points radius too?
				if (sqrDst < radius * radius || sqrDst < pushedPoints[i].radius * pushedPoints[i].radius)
				{
					return true;
				}
			}
			return false;
		}


		public static void PushPointIntoGrid(PoissonCell[,] cells, PoissonPoint poissonPoint ,float minRadius)
		{
			float cellSize = minRadius / Mathf.Sqrt(2);
			int cellX = (int)(poissonPoint.pos.x / cellSize);
			int cellY = (int)(poissonPoint.pos.y / cellSize);
			int cellLength = Mathf.CeilToInt(poissonPoint.radius / cellSize);
			int searchStartX = Mathf.Max(0, cellX - cellLength);
			int searchEndX = Mathf.Min(cellX + cellLength, cells.GetLength(0) - 1);
			int searchStartY = Mathf.Max(0, cellY - cellLength);
			int searchEndY = Mathf.Min(cellY + cellLength, cells.GetLength(1) - 1);

			cells[cellX, cellY].pointInCell = poissonPoint;
			for (int x = searchStartX; x <= searchEndX; x++)
			{
				for (int y = searchStartY; y <= searchEndY; y++)
				{
					cells[x, y].pushedPoints.Add(poissonPoint);
				}
			}
		}


		public static PoissonCell[,] CreateGrid(Vector2 sampleRegionSize, float minRadius)
		{
			float cellSize = minRadius / Mathf.Sqrt(2);
			PoissonCell[,] grid = new PoissonCell[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
			for (int x = 0; x < grid.GetLength(0); x++)
			{
				for (int y = 0; y < grid.GetLength(1); y++)
				{
					grid[x, y] = new PoissonCell(x, y);
				}
			}
			
			//intilise grid here
			
			return grid;
		}

		private static List<Vector2> ConvertPointsToWorld(List<Vector2> points, Vector3 offset)
		{
			for (int i = 0; i < points.Count; i++)
			{
				points[i] += new Vector2(offset.x, offset.z);
			}
			return points;
		}

		private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
		{
			if (!(candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y))
			{
				return false;
			}

			int cellX = (int)(candidate.x / cellSize);
			int cellY = (int)(candidate.y / cellSize);
			int searchStartX = Mathf.Max(0, cellX - 2);
			int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
			int searchStartY = Mathf.Max(0, cellY - 2);
			int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

			for (int x = searchStartX; x <= searchEndX; x++)
			{
				for (int y = searchStartY; y <= searchEndY; y++)
				{
					int pointIndex = grid[x, y] - 1;
					if (pointIndex != -1)
					{
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
						if (sqrDst < radius * radius)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public static List<Vector2> CreateRandomPoints(Vector2 sampleRegionSize, float radius, Vector3 offset, int rejectionSamples = 30)
		{
			float cellSize = radius / Mathf.Sqrt(2);

			int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
			List<Vector2> points = new List<Vector2>();
			List<Vector2> spawnPoints = new List<Vector2>();

			spawnPoints.Add(sampleRegionSize / 2);
			while (spawnPoints.Count > 0)
			{
				int spawnIndex = Random.Range(0, spawnPoints.Count);
				Vector2 spawnCentre = spawnPoints[spawnIndex];
				bool candidateAccepted = false;

				for (int i = 0; i < rejectionSamples; i++)
				{
					float angle = Random.value * Mathf.PI * 2;
					Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2f * radius);
					if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
					{
						points.Add(candidate);
						spawnPoints.Add(candidate);
						grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
						candidateAccepted = true;
						break;
					}
				}
				if (!candidateAccepted)
				{
					spawnPoints.RemoveAt(spawnIndex);
				}

			}

			return ConvertPointsToWorld(points, offset);
		}
	
}