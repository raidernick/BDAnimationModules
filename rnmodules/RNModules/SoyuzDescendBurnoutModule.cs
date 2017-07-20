using System;
using UnityEngine;

namespace RNModules
{
	public class RNDescentBurn : PartModule
	{
		[KSPField]
		public float TemperatureThreshold;

		[KSPField]
		public float BurnIntensity;

		[KSPField(isPersistant = true, guiActive = true, guiName = "Burnt State")]
		public float BurnStatus = 1f;

		[KSPField(guiActive = true, guiName = "Temperature", guiUnits = "°", guiFormat = "f2")]
		public float CurrentTemp;

		private MeshRenderer[] _meshRenderers;

		public override void OnAwake()
		{
			Debug.Log(string.Format("RNDescentBurn::OnAwake", new object[0]));
			OnAwake();
			if (HighLogic.LoadedSceneIsFlight)
			{
                _meshRenderers = part.FindModelComponents<MeshRenderer>().ToArray();
				if (_meshRenderers != null)
				{
					MeshRenderer[] meshRenderers = _meshRenderers;
					for (int i = 0; i < meshRenderers.Length; i++)
					{
						MeshRenderer meshRenderer = meshRenderers[i];
						meshRenderer.material.SetFloat("_Factor", BurnStatus);
					}
				}
			}
		}

		public void FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				CurrentTemp = Convert.ToSingle(part.temperature);
                if (float.IsPositiveInfinity(CurrentTemp))
                {
                    CurrentTemp = float.MaxValue;
                }
                else if (float.IsNegativeInfinity(CurrentTemp))
                {
                    CurrentTemp = float.MinValue;
                }
                if (vessel.situation == (Vessel.Situations)3 && BurnStatus > 0f)
				{
					if (part.temperature > TemperatureThreshold && BurnStatus > 0f)
					{
						float tempDelta = Convert.ToSingle(part.temperature - TemperatureThreshold);
                        if (float.IsPositiveInfinity(tempDelta))
                        {
                            tempDelta = float.MaxValue;
                        }
                        else if (float.IsNegativeInfinity(tempDelta))
                        {
                            tempDelta = float.MinValue;
                        }
                        float delta = tempDelta * tempDelta * BurnIntensity * TimeWarp.fixedDeltaTime;
						BurnStatus -= delta;
						if (BurnStatus < 0f)
						{
							BurnStatus = 0f;
						}
						MeshRenderer[] meshRenderers = _meshRenderers;
						for (int i = 0; i < meshRenderers.Length; i++)
						{
							MeshRenderer meshRenderer = meshRenderers[i];
							meshRenderer.material.SetFloat("_Factor", BurnStatus);
						}
					}
				}
			}
		}

		[KSPEvent(guiActive = true, guiName = "Toggle Burnt State")]
		public void ToggleState()
		{
			if (BurnStatus > 0.9f)
			{
				BurnStatus = 0.75f;
			}
			else if (BurnStatus > 0.6f)
			{
				BurnStatus = 0.5f;
			}
			else if (BurnStatus > 0.3f)
			{
				BurnStatus = 0.25f;
			}
			else if (BurnStatus > 0.1f)
			{
				BurnStatus = 0f;
			}
			else
			{
				BurnStatus = 1f;
			}
			MeshRenderer[] meshRenderers = _meshRenderers;
			for (int i = 0; i < meshRenderers.Length; i++)
			{
				MeshRenderer meshRenderer = meshRenderers[i];
				meshRenderer.material.SetFloat("_Factor", BurnStatus);
			}
		}
	}
}
