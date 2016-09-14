using System;
using System.Collections.Generic;
using UnityEngine;


namespace BDAnimationModules
{
	public class BDVTOLDrivenEngineAnimator : PartModule
	{
		public ModuleEngines engine;
		
		[KSPField(isPersistant = false)]
		public string animationName;
		
		public List<BDVTOLAnimator> vtolEngines;
		
		
		public AnimationState[] deployStates;
		
		
		public override void OnStart (PartModule.StartState state)
		{
			deployStates = Utils.SetUpAnimation(animationName, part);
			engine = part.FindModuleImplementing<ModuleEngines>();
			vtolEngines = new List<BDVTOLAnimator>();
			
			engine.Actions["ActivateAction"].active = false;
			engine.Actions["ShutdownAction"].active = false;
		}
		
		void FixedUpdate()
		{
			//todo: check if vtolengines are attached
			if(HighLogic.LoadedSceneIsFlight) vtolEngines = vessel.FindPartModulesImplementing<BDVTOLAnimator>();
			BDVTOLAnimator controllerVTOL = null;
			foreach(var vtol in vtolEngines)
			{
				if(vtol!=null) controllerVTOL = vtol;	
			}
			if(controllerVTOL!=null)
			{
				if(HighLogic.LoadedSceneIsFlight && engine.EngineIgnited != controllerVTOL.part.FindModuleImplementing<ModuleEngines>().EngineIgnited)
				{
					if(controllerVTOL.part.FindModuleImplementing<ModuleEngines>().EngineIgnited)	
					{
						engine.Activate();	
					}
					else
					{
						engine.Shutdown();	
					}
				}
				
				float vtolEnginesTorque = 0;
				foreach(var vtolEngine in vtolEngines)
				{
					if(HighLogic.LoadedSceneIsFlight)
					{
						vtolEnginesTorque += Mathf.Abs(Mathf.Sin(vtolEngine.currentAngle*Mathf.Deg2Rad) * vtolEngine.part.FindModuleImplementing<ModuleEngines>().finalThrust * Vector3.Distance(vtolEngine.thrustPosition, vessel.CoM));
					}
					else
					{
                        //vtolEnginesTorque += Mathf.Sin(vtolEngine.currentAngle * vtolEngine.part.FindModuleImplementing<ModuleEngines>().maxThrust * Vector3.Distance(vtolEngine.thrustPosition, vessel.CoM));
                    }
                }
				
				
				if(HighLogic.LoadedSceneIsFlight) 
				{
					float engineThrust = vtolEnginesTorque / Vector3.Distance(part.FindModelTransform(engine.thrustVectorTransformName).position, vessel.CoM);
					engine.minThrust = engineThrust;
					engine.maxThrust = engineThrust;
				}
				
				
				foreach(var anim in deployStates)
				{
					if(controllerVTOL.vtolModeEnabled && anim.normalizedTime < 1)	
					{
						anim.speed = 1;	
					}
					else if(controllerVTOL.vtolModeEnabled)
					{
						anim.speed = 0;
						anim.normalizedTime = 1;
					}
					else if(anim.normalizedTime > 0)
					{
						anim.speed = -1;	
					}
					else
					{
						anim.speed = 0;
						anim.normalizedTime = 0;
					}
				}
			}
			
		}
		
		
		
		
	}
}

