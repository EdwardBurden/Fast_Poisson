using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "SettlementGenerationSettings")]
public class SettlementGenerationSettings : ScriptableObject
{
	[SerializeField] public AnimationCurve radiusCurve; //0-1, 
	[SerializeField] public AnimationCurve overideRadiusCurve; //TEMP
	[SerializeField] public AnimationCurve heightCurve; //0-1
	[SerializeField][Min(1)] public float regionExtent = 2;
	[SerializeField][Min(0.05f)] public float minBuildingRadius = 1;
	[SerializeField][Min(0.25f)] public float maxBuildingRadius = 1.5f;

	[SerializeField][Min(0.05f)] public float minHeight = 1;
	[SerializeField][Min(0.25f)] public float maxheight = 1.5f;
	[SerializeField] public int rejectionSamples = 30;
	[SerializeField] public SettlementBuildings[] settlemenSpawnInfos;
	[SerializeField][Range(0, 1)] public float radiusNoisePercentage = 0.1f; //the amount of points effected 
	[SerializeField] public float radiusNoiseModifier = 1f; //the severity of the difference  form the noise
	[SerializeField] public int maxPoints = 1000; //the severity of the difference  form the noise
	[SerializeField] public GameObject centerBuilding;
	public Vector2 regionSize => new Vector2(regionExtent, regionExtent);
	public Vector3 offset => new Vector3(regionSize.x, 0, regionSize.y) / 2.0f;

	public float CenterRadius = 3;

	public float SampleRadius(Vector2 point, Transform parentTransform, float perlinSeed)
	{
		Vector3 worldPos = (point.ToVector3() - offset) + parentTransform.position;
		Vector2 noisePos = new Vector2((perlinSeed + worldPos.x) * 100f, (perlinSeed + worldPos.z) * 100f);
		float evaluationPos = 1f - Vector3.Distance(worldPos, parentTransform.position) / (regionExtent / 2);
		float noise = ((Mathf.PerlinNoise(noisePos.x, noisePos.y) - 0.5f) * 2) * radiusNoisePercentage * radiusNoiseModifier; //range -1 to 1 multiplied by noiseModifier
		float curvePercentage = radiusCurve.Evaluate(evaluationPos);
		float curveRadius = minBuildingRadius + ((maxBuildingRadius - minBuildingRadius) * curvePercentage);
		return Mathf.Clamp(curveRadius + noise, minBuildingRadius, maxBuildingRadius);
	}

	public float SampleHeight(Vector2 point, Transform parentTransform, float perlinSeed)
	{
		Vector3 worldPos = (point.ToVector3() - offset) + parentTransform.position;
		Vector2 noisePos = new Vector2((perlinSeed + worldPos.x) * 100f, (perlinSeed + worldPos.z) * 100f);
		float evaluationPos = 1f - Vector3.Distance(worldPos, parentTransform.position) / (regionExtent / 2);
		float noise = ((Mathf.PerlinNoise(noisePos.x, noisePos.y) - 0.5f) * 2) * radiusNoisePercentage * radiusNoiseModifier; //range -1 to 1 multiplied by noiseModifier
		float curvePercentage = heightCurve.Evaluate(evaluationPos);
		float curveRadius = minHeight + ((maxheight - minHeight) * curvePercentage);
		return Mathf.Clamp(curveRadius + noise, minHeight, maxheight);
	}

	public float SampleOverrideRadius(Vector2 point, Transform parentTransform, float perlinSeed) //TEMP
	{
		Vector3 worldPos = (point.ToVector3() - offset) + parentTransform.position;
		Vector2 noisePos = new Vector2((perlinSeed + worldPos.x) * 100f, (perlinSeed + worldPos.z) * 100f);
		float evaluationPos = 1f - Vector3.Distance(worldPos, parentTransform.position) / (regionExtent / 2);
		float noise = ((Mathf.PerlinNoise(noisePos.x, noisePos.y) - 0.5f) * 2) * radiusNoisePercentage * radiusNoiseModifier; //range -1 to 1 multiplied by noiseModifier
		float curvePercentage = overideRadiusCurve.Evaluate(evaluationPos);
		float curveRadius = minBuildingRadius + ((maxBuildingRadius - minBuildingRadius) * curvePercentage);
		return Mathf.Clamp(curveRadius + noise, minBuildingRadius, maxBuildingRadius);
	}

	internal float SampleRadiusAtLevel(PoissonPoint poissonPoint, Transform transform, float perlinSeed, int level)
	{
		return SampleOverrideRadius(poissonPoint.pos, transform, perlinSeed);
		//throw new NotImplementedException();
	}
}
