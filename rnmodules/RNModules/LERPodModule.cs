using System;
using UnityEngine;

namespace RNModules
{
	public class LERPodModule : PartModule
	{
		private Renderer[] _suitMeshes;

		public override void OnAwake()
		{
			base.OnAwake();
			if (HighLogic.LoadedSceneIsFlight)
			{
				Debug.Log("LERPodModule::OnAwake");
				int crewCount = base.part.CrewCapacity;
				Debug.Log(string.Format("Crew Capacity: {0}", crewCount));
				this._suitMeshes = new Renderer[crewCount];
				for (int i = 0; i < crewCount; i++)
				{
					string meshName = string.Format("suit{0}", i + 1);
					this._suitMeshes[i] = base.part.FindModelComponent<Renderer>(meshName);
					Debug.Log(string.Format("Mesh '{0}': {1}", meshName, this._suitMeshes[i] != null));
				}
			}
		}

		public void FixedUpdate()
		{
			if (this._suitMeshes != null)
			{
				int actualCrewCount = base.part.protoModuleCrew.Count;
				for (int i = 0; i < this._suitMeshes.Length; i++)
				{
					this._suitMeshes[i].enabled = (i < actualCrewCount);
				}
			}
		}

		public void Destroy()
		{
			Debug.Log("LERPodModule::Destroy");
			for (int i = 0; i < this._suitMeshes.Length; i++)
			{
				this._suitMeshes[i].enabled = true;
			}
		}
	}
}
