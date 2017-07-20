using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNModules
{
	public class RNLandingEngine : PartModule
	{
		[KSPField(isPersistant = true, guiName = "Auto-Ignition Altitude", guiActive = true)]
		public float IgnitionAltitude;

		private bool engineActivated;

		private IList<ModuleEngines> numEngines;

		public override void OnAwake()
		{
			base.OnAwake();
			if (HighLogic.LoadedSceneIsFlight)
			{
				Debug.Log(string.Format("RNLandingEngine::OnAwake, IgnitionAltitude = {0}", IgnitionAltitude));
                numEngines = part.FindModulesImplementing<ModuleEngines>();
				Debug.Log(string.Format("Found {0} engine modules", (numEngines != null) ? numEngines.Count : -1));
			}
		}

		public void FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight && vessel.situation == Vessel.Situations.FLYING)
			{
				if (!engineActivated)
				{
                    Vector3 srfVelocity = part.vessel.GetSrfVelocity();
                    Vector3 normalized = srfVelocity.normalized;
                    if (srfVelocity.magnitude >= 2f && normalized.z <= -0.2)
					{
						double num = part.vessel.terrainAltitude;
						if (num < 0.0)
						{
							num = 0.0;
						}
						double num2 = part.vessel.altitude - num;
						if (num2 <= IgnitionAltitude)
						{
							foreach (ModuleEngines current in numEngines)
							{
								current.OnActive();
								Debug.Log("Activating engine.");
							}
                            engineActivated = true;
						}
					}
				}
			}
		}
	}
}
