using System;
using UnityEngine;

namespace RNModules
{
	public class RNDescentMode : PartModule
	{
		[KSPField]
		public Vector3 DescentModeCoM;

		[KSPField(guiActive = true, guiName = "Descent Mode Active?", isPersistant = true)]
		public bool IsDescentMode;

		private Vector3 _defaultCoM;

		[KSPEvent(guiName = "Toggle Descent Mode", guiActive = true)]
		public void ToggleMode()
		{
			this.IsDescentMode = !this.IsDescentMode;
			this.SetDescentMode(this.IsDescentMode);
		}

		private void SetDescentMode(bool isOn)
		{
			if (isOn)
			{
				base.part.CoMOffset = this.DescentModeCoM;
			}
			else
			{
				base.part.CoMOffset = this._defaultCoM;
			}
		}

		[KSPAction("Toggle Descent Mode")]
		public void Toggle(KSPActionParam param)
		{
			this.ToggleMode();
		}

		public override void OnAwake()
		{
			base.OnAwake();
			if (HighLogic.LoadedSceneIsFlight)
			{
				this._defaultCoM = base.part.CoMOffset;
				this.SetDescentMode(this.IsDescentMode);
			}
		}
	}
}
