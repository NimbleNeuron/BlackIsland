using UnityEngine;

public class TCP2_GetPosOnWater : MonoBehaviour
{
	public Material WaterMaterial;


	[Tooltip("Height scale, for example if the water mesh is scaled along its Y axis")]
	public float heightScale = 1f;


	[Space] [Tooltip("Will make the object stick to the water plane")]
	public bool followWaterHeight = true;


	[Tooltip("Y Position offset")] public float heightOffset;


	[Space] [Tooltip("Will align the object to the wave normal based on its position")]
	public bool followWaterNormal;


	[Tooltip("Determine the object's up axis (when following wave normal)")]
	public Vector3 upAxis = new Vector3(0f, 1f, 0f);


	[Tooltip("Rotation of the object once it's been affected by the water normal")]
	public Vector3 postRotation = new Vector3(0f, 0f, 0f);


	[SerializeField] [HideInInspector] private bool isValid = default;


	[SerializeField] [HideInInspector] private int sineCount = default;


	private readonly float[] sinePhsOffsetsX =
	{
		1f,
		1.3f,
		0.7f,
		1.75f,
		0.2f,
		2.6f,
		0.7f,
		3.1f
	};


	private readonly float[] sinePhsOffsetsZ =
	{
		2.2f,
		0.4f,
		3.3f,
		2.9f,
		0.5f,
		4.8f,
		3.1f,
		2.3f
	};


	private readonly float[] sinePosOffsetsX =
	{
		1f,
		2.2f,
		2.7f,
		3.4f,
		1.4f,
		1.8f,
		4.2f,
		3.6f
	};


	private readonly float[] sinePosOffsetsZ =
	{
		0.6f,
		1.3f,
		3.1f,
		2.4f,
		1.1f,
		2.8f,
		1.7f,
		4.3f
	};


	private void LateUpdate()
	{
		if (followWaterHeight)
		{
			Vector3 positionOnWater = GetPositionOnWater(transform.position);
			positionOnWater.y += heightOffset;
			transform.position = positionOnWater;
		}

		if (followWaterNormal)
		{
			transform.rotation = Quaternion.FromToRotation(upAxis, GetNormalOnWater(transform.position));
			transform.Rotate(postRotation, Space.Self);
		}
	}


	public Vector3 GetPositionOnWater(Vector3 worldPosition)
	{
		if (!isValid)
		{
			Debug.LogWarning("Invalid Water Material, returning the same worldPosition");
			return worldPosition;
		}

		float @float = WaterMaterial.GetFloat("_WaveFrequency");
		float num = WaterMaterial.GetFloat("_WaveHeight") * heightScale;
		float float2 = WaterMaterial.GetFloat("_WaveSpeed");
		float num2 = Time.time * float2;
		float num3 = worldPosition.x * @float;
		float num4 = worldPosition.z * @float;
		float num5 = 0f;
		float num6 = 0f;
		int num7 = sineCount;
		switch (num7)
		{
			case 1:
				num5 = Mathf.Sin(num3 + num2) * num;
				num6 = Mathf.Sin(num4 + num2) * num;
				break;
			case 2:
				num5 = (Mathf.Sin(sinePosOffsetsX[0] * num3 + sinePhsOffsetsX[0] * num2) +
				        Mathf.Sin(sinePosOffsetsX[1] * num3 + sinePhsOffsetsX[1] * num2)) * num / 2f;
				num6 = (Mathf.Sin(sinePosOffsetsZ[0] * num4 + sinePhsOffsetsZ[0] * num2) +
				        Mathf.Sin(sinePosOffsetsZ[1] * num4 + sinePhsOffsetsZ[1] * num2)) * num / 2f;
				break;
			case 3:
				break;
			case 4:
				num5 = (Mathf.Sin(sinePosOffsetsX[0] * num3 + sinePhsOffsetsX[0] * num2) +
				        Mathf.Sin(sinePosOffsetsX[1] * num3 + sinePhsOffsetsX[1] * num2) +
				        Mathf.Sin(sinePosOffsetsX[2] * num3 + sinePhsOffsetsX[2] * num2) +
				        Mathf.Sin(sinePosOffsetsX[3] * num3 + sinePhsOffsetsX[3] * num2)) * num / 4f;
				num6 = (Mathf.Sin(sinePosOffsetsZ[0] * num4 + sinePhsOffsetsZ[0] * num2) +
				        Mathf.Sin(sinePosOffsetsZ[1] * num4 + sinePhsOffsetsZ[1] * num2) +
				        Mathf.Sin(sinePosOffsetsZ[2] * num4 + sinePhsOffsetsZ[2] * num2) +
				        Mathf.Sin(sinePosOffsetsZ[3] * num4 + sinePhsOffsetsZ[3] * num2)) * num / 4f;
				break;
			default:
				if (num7 == 8)
				{
					num5 = (Mathf.Sin(sinePosOffsetsX[0] * num3 + sinePhsOffsetsX[0] * num2) +
					        Mathf.Sin(sinePosOffsetsX[1] * num3 + sinePhsOffsetsX[1] * num2) +
					        Mathf.Sin(sinePosOffsetsX[2] * num3 + sinePhsOffsetsX[2] * num2) +
					        Mathf.Sin(sinePosOffsetsX[3] * num3 + sinePhsOffsetsX[3] * num2) +
					        Mathf.Sin(sinePosOffsetsX[4] * num3 + sinePhsOffsetsX[4] * num2) +
					        Mathf.Sin(sinePosOffsetsX[5] * num3 + sinePhsOffsetsX[5] * num2) +
					        Mathf.Sin(sinePosOffsetsX[6] * num3 + sinePhsOffsetsX[6] * num2) +
					        Mathf.Sin(sinePosOffsetsX[7] * num3 + sinePhsOffsetsX[7] * num2)) * num / 8f;
					num6 = (Mathf.Sin(sinePosOffsetsZ[0] * num4 + sinePhsOffsetsZ[0] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[1] * num4 + sinePhsOffsetsZ[1] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[2] * num4 + sinePhsOffsetsZ[2] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[3] * num4 + sinePhsOffsetsZ[3] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[4] * num4 + sinePhsOffsetsZ[4] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[5] * num4 + sinePhsOffsetsZ[5] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[6] * num4 + sinePhsOffsetsZ[6] * num2) +
					        Mathf.Sin(sinePosOffsetsZ[7] * num4 + sinePhsOffsetsZ[7] * num2)) * num / 8f;
				}

				break;
		}

		worldPosition.y = num5 + num6;
		return worldPosition;
	}


	public Vector3 GetNormalOnWater(Vector3 worldPosition)
	{
		if (!isValid)
		{
			Debug.LogWarning("Invalid Water Material, returning the Vector3.up as the normal");
			return Vector3.up;
		}

		float @float = WaterMaterial.GetFloat("_WaveFrequency");
		float num = WaterMaterial.GetFloat("_WaveHeight") * heightScale;
		float float2 = WaterMaterial.GetFloat("_WaveSpeed");
		float num2 = Time.time * float2;
		float num3 = worldPosition.x * @float;
		float num4 = worldPosition.z * @float;
		float x = 0f;
		float z = 0f;
		int num5 = sineCount;
		switch (num5)
		{
			case 1:
				x = -num * Mathf.Cos(num3 + num2);
				z = -num * Mathf.Cos(num4 + num2);
				break;
			case 2:
				x = -num / 2f * (Mathf.Cos(sinePosOffsetsX[0] * num3 + sinePhsOffsetsX[0] * num2) * sinePosOffsetsX[0] +
				                 Mathf.Cos(sinePosOffsetsX[1] * num3 + sinePhsOffsetsX[1] * num2) * sinePosOffsetsX[1]);
				z = -num / 2f * (Mathf.Cos(sinePosOffsetsZ[0] * num4 + sinePhsOffsetsZ[0] * num2) * sinePosOffsetsZ[0] +
				                 Mathf.Cos(sinePosOffsetsZ[1] * num4 + sinePhsOffsetsZ[1] * num2) * sinePosOffsetsZ[1]);
				break;
			case 3:
				break;
			case 4:
				x = -num / 4f * (Mathf.Cos(sinePosOffsetsX[0] * num3 + sinePhsOffsetsX[0] * num2) * sinePosOffsetsX[0] +
				                 Mathf.Cos(sinePosOffsetsX[1] * num3 + sinePhsOffsetsX[1] * num2) * sinePosOffsetsX[1] +
				                 Mathf.Cos(sinePosOffsetsX[2] * num3 + sinePhsOffsetsX[2] * num2) * sinePosOffsetsX[2] +
				                 Mathf.Cos(sinePosOffsetsX[3] * num3 + sinePhsOffsetsX[3] * num2) * sinePosOffsetsX[3]);
				z = -num / 4f * (Mathf.Cos(sinePosOffsetsZ[0] * num4 + sinePhsOffsetsZ[0] * num2) * sinePosOffsetsZ[0] +
				                 Mathf.Cos(sinePosOffsetsZ[1] * num4 + sinePhsOffsetsZ[1] * num2) * sinePosOffsetsZ[1] +
				                 Mathf.Cos(sinePosOffsetsZ[2] * num4 + sinePhsOffsetsZ[2] * num2) * sinePosOffsetsZ[2] +
				                 Mathf.Cos(sinePosOffsetsZ[3] * num4 + sinePhsOffsetsZ[3] * num2) * sinePosOffsetsZ[3]);
				break;
			default:
				if (num5 == 8)
				{
					x = -num / 8f *
					    (Mathf.Cos(sinePosOffsetsX[0] * num3 + sinePhsOffsetsX[0] * num2) * sinePosOffsetsX[0] +
					     Mathf.Cos(sinePosOffsetsX[1] * num3 + sinePhsOffsetsX[1] * num2) * sinePosOffsetsX[1] +
					     Mathf.Cos(sinePosOffsetsX[2] * num3 + sinePhsOffsetsX[2] * num2) * sinePosOffsetsX[2] +
					     Mathf.Cos(sinePosOffsetsX[3] * num3 + sinePhsOffsetsX[3] * num2) * sinePosOffsetsX[3] +
					     Mathf.Cos(sinePosOffsetsX[4] * num3 + sinePhsOffsetsX[4] * num2) * sinePosOffsetsX[4] +
					     Mathf.Cos(sinePosOffsetsX[5] * num3 + sinePhsOffsetsX[5] * num2) * sinePosOffsetsX[5] +
					     Mathf.Cos(sinePosOffsetsX[6] * num3 + sinePhsOffsetsX[6] * num2) * sinePosOffsetsX[6] +
					     Mathf.Cos(sinePosOffsetsX[7] * num3 + sinePhsOffsetsX[7] * num2) * sinePosOffsetsX[7]);
					z = -num / 8f *
					    (Mathf.Cos(sinePosOffsetsZ[0] * num4 + sinePhsOffsetsZ[0] * num2) * sinePosOffsetsZ[0] +
					     Mathf.Cos(sinePosOffsetsZ[1] * num4 + sinePhsOffsetsZ[1] * num2) * sinePosOffsetsZ[1] +
					     Mathf.Cos(sinePosOffsetsZ[2] * num4 + sinePhsOffsetsZ[2] * num2) * sinePosOffsetsZ[2] +
					     Mathf.Cos(sinePosOffsetsZ[3] * num4 + sinePhsOffsetsZ[3] * num2) * sinePosOffsetsZ[3] +
					     Mathf.Cos(sinePosOffsetsZ[4] * num4 + sinePhsOffsetsZ[4] * num2) * sinePosOffsetsZ[4] +
					     Mathf.Cos(sinePosOffsetsZ[5] * num4 + sinePhsOffsetsZ[5] * num2) * sinePosOffsetsZ[5] +
					     Mathf.Cos(sinePosOffsetsZ[6] * num4 + sinePhsOffsetsZ[6] * num2) * sinePosOffsetsZ[6] +
					     Mathf.Cos(sinePosOffsetsZ[7] * num4 + sinePhsOffsetsZ[7] * num2) * sinePosOffsetsZ[7]);
				}

				break;
		}

		return new Vector3(x, 1f, z).normalized;
	}
}