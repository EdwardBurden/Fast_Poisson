using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

//Could be mono? will return to later
public class PoissonGrid : MonoBehaviour
{
	private PoissonCell[,] grid;
	public List<PoissonPoint> spawnedPoints; //temp public
	private Vector2 regionSize;
	private float minRadius;
	private float cellSize;

	public void CreateGrid(Vector2 regionSize, float minRadius)
	{
		this.regionSize = regionSize;
		this.minRadius = minRadius;
		cellSize = minRadius / Mathf.Sqrt(2);
		this.grid = CreateGrid();
		this.spawnedPoints = new List<PoissonPoint>();
	}

	public bool CanPlacePoint(Vector2 candidate, float radius)
	{
		if (!(candidate.x >= 0 && candidate.x < regionSize.x && candidate.y >= 0 && candidate.y < regionSize.y))
		{
			return false;
		}
		if (DoesPointOverlapExistingPoint(candidate, radius))
		{
			return false;
		}
		return true;
	}

	public List<PoissonPoint> RemovePointsOverlapping(Vector2 pos, float sampledRadius)
	{
		List<PoissonPoint> removedPoints = new List<PoissonPoint>();
		int cellX = (int)(pos.x / cellSize);
		int cellY = (int)(pos.y / cellSize);
		int cellLength = Mathf.CeilToInt(sampledRadius / cellSize);
		int searchStartX = Mathf.Max(0, cellX - cellLength);
		int searchEndX = Mathf.Min(cellX + cellLength, grid.GetLength(0) - 1);
		int searchStartY = Mathf.Max(0, cellY - cellLength);
		int searchEndY = Mathf.Min(cellY + cellLength, grid.GetLength(1) - 1);

		for (int x = searchStartX; x <= searchEndX; x++)
		{
			for (int y = searchStartY; y <= searchEndY; y++)
			{
				List<PoissonPoint> pushedPoints = grid[x, y].pushedPoints;
				for (int i = 0; i < pushedPoints.Count; i++)
				{
					float sqrDst = (pos - pushedPoints[i].pos).sqrMagnitude;
					if (sqrDst < sampledRadius * sampledRadius || sqrDst < pushedPoints[i].radius * pushedPoints[i].radius)
					{
						removedPoints.Add(pushedPoints[i]);
						RemovePoint(pushedPoints[i]);
					}
					else
					{
						pushedPoints[i].tested = false;
					}
				}
			}
		}
		return removedPoints;
	}

	public void RemovePoint(PoissonPoint point)
	{
		PullPointOutOfGrid(point);
		spawnedPoints.Remove(point);
	}

	public PoissonPoint AddPoint(Vector2 candidate, float sampledRadius)
	{
		PoissonPoint candiatePoint = new PoissonPoint(candidate, sampledRadius);
		PushPointIntoGrid(candiatePoint);
		spawnedPoints.Add(candiatePoint);
		return candiatePoint;
	}

	public List<PoissonPoint> FindUntestedPoints()
	{
		List<PoissonPoint> untestedPoints = new List<PoissonPoint>();
		foreach (PoissonPoint point in spawnedPoints)
		{
			if (!point.tested)
			{
				untestedPoints.Add(point);
			}
		}
		return untestedPoints;
	}

	private bool DoesPointOverlapExistingPoint(Vector2 point, float radius)
	{
		int cellX = (int)(point.x / cellSize);
		int cellY = (int)(point.y / cellSize);
		List<PoissonPoint> pushedPoints = grid[cellX, cellY].pushedPoints;
		for (int i = 0; i < pushedPoints.Count; i++)
		{
			float sqrDst = (point - pushedPoints[i].pos).sqrMagnitude;

			//check agian that points radius too
			if (sqrDst < radius * radius || sqrDst < pushedPoints[i].radius * pushedPoints[i].radius)
			{
				return true;
			}
		}
		return false;
	}

	private void PushPointIntoGrid(PoissonPoint poissonPoint)
	{
		int cellX = (int)(poissonPoint.pos.x / cellSize);
		int cellY = (int)(poissonPoint.pos.y / cellSize);
		int cellLength = Mathf.CeilToInt(poissonPoint.radius / cellSize);
		int searchStartX = Mathf.Max(0, cellX - cellLength);
		int searchEndX = Mathf.Min(cellX + cellLength, grid.GetLength(0) - 1);
		int searchStartY = Mathf.Max(0, cellY - cellLength);
		int searchEndY = Mathf.Min(cellY + cellLength, grid.GetLength(1) - 1);

		grid[cellX, cellY].pointInCell = poissonPoint;
		for (int x = searchStartX; x <= searchEndX; x++)
		{
			for (int y = searchStartY; y <= searchEndY; y++)
			{
				grid[x, y].pushedPoints.Add(poissonPoint);
			}
		}
	}

	private void PullPointOutOfGrid(PoissonPoint poissonPoint)
	{
		int cellX = (int)(poissonPoint.pos.x / cellSize);
		int cellY = (int)(poissonPoint.pos.y / cellSize);
		int cellLength = Mathf.CeilToInt(poissonPoint.radius / cellSize);
		int searchStartX = Mathf.Max(0, cellX - cellLength);
		int searchEndX = Mathf.Min(cellX + cellLength, grid.GetLength(0) - 1);
		int searchStartY = Mathf.Max(0, cellY - cellLength);
		int searchEndY = Mathf.Min(cellY + cellLength, grid.GetLength(1) - 1);

		grid[cellX, cellY].pointInCell = null;
		for (int x = searchStartX; x <= searchEndX; x++)
		{
			for (int y = searchStartY; y <= searchEndY; y++)
			{
				grid[x, y].pushedPoints.Remove(poissonPoint);
			}
		}
	}

	private PoissonCell[,] CreateGrid()
	{
		PoissonCell[,] grid = new PoissonCell[Mathf.CeilToInt(regionSize.x / cellSize), Mathf.CeilToInt(regionSize.y / cellSize)];
		for (int x = 0; x < grid.GetLength(0); x++)
		{
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				grid[x, y] = new PoissonCell(x, y);
			}
		}
		return grid;
	}
}
