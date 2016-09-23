using System;
using System.Linq;
using System.Collections;
using KSP;
using UnityEngine;

/*Code by Starwaster
License: Released under Creative Commons Share-alike attribution license
http://creativecommons.org/licenses/by-sa/4.0/
*/
namespace BDAnimationModules
{
    public class BDAnimatedDec : ModuleDecouple, IScalarModule
    {
        [KSPField]
        public string animationName = "";

        [KSPField()]
        public bool waitForAnimation = false;

        [KSPField(isPersistant = true)]
        public bool animationComplete = false;

        [KSPField]
        public int layer = 0;

        [KSPField]
        public string moduleID = "bdanimatedDecoupler";

        protected Animation anim;

        protected bool isDecoupling;

        protected bool isResetting;

        protected bool decoupleAfterAnimation = false;

        protected ModuleCargoBay cargoBay;

        public BDAnimatedDec() :
        base()
        {
        }

        //[KSPAction("Decouple")]
        public new void DecoupleAction(KSPActionParam param)
        {
            if ((object)anim != null)
            {
                anim.Play(animationName);
                isDecoupling = true;
                OnMoving.Fire(0f, 1f);
                if (waitForAnimation)
                    StartCoroutine(DelayedDecouple());
                else
                    OnDecouple();
            }
            else
                OnDecouple();
        }

        public new void Decouple()
        {
            if ((object)anim != null)
            {
                anim.Play(animationName);
                isDecoupling = true;
                OnMoving.Fire(0f, 1f);
                if (waitForAnimation)
                    StartCoroutine(DelayedDecouple());
                else
                    OnDecouple();
            }
            else
                OnDecouple();
        }


        public override void OnAwake()
        {
            this.OnMovingEvent = new EventData<float, float>("BDAnimatedDec.OnMovingEvent");
            this.OnStoppedEvent = new EventData<float>("BDAnimatedDec.OnStoppedEvent");
            base.OnAwake();
        }

        public override void OnStart(StartState state)
        {
            GameEvents.onVesselWasModified.Add(OnVesselWasModified);

            cargoBay = part.FindModuleImplementing<ModuleCargoBay>();
            base.OnStart(state);
            Debug.Log("BDAnimatedDec.OnStart(), isDecoupled = " + this.isDecoupled.ToString());
            if (animationName != "")
            {
                anim = part.FindModelAnimators(animationName).FirstOrDefault();
                if ((object)this.anim == null)
                {
                    Debug.Log("BDAnimatedDec: Animations not found");
                }
                else
                {
                    Debug.Log("BDAnimatedDec.OnStart() - Animation found named " + animationName);
                    // If Decoupled or animation already played then set animation to end.
                    this.anim[animationName].layer = layer;
                    if (this.animationComplete || this.isDecoupled)
                    {
                        this.anim[animationName].normalizedTime = 1f;
                    }
                }
            }
        }

        public override void OnActive()
        {
            if (staged)
            {
                if ((object)anim != null)
                {
                    anim.Play(animationName);
                    isDecoupling = true;
                    OnMoving.Fire(0f, 1f);
                    if (waitForAnimation)
                        StartCoroutine(DelayedDecouple());
                    else
                        OnDecouple();
                }
                else
                    OnDecouple();
            }
        }

        private void OnVesselWasModified(Vessel v)
        {
            if ((object)v != null && v == vessel)
            {
                if (!(isDecoupling || isDecoupled))
                {
                    if ((object)part.FindAttachNode(this.explosiveNodeID).attachedPart == null)
                    {
                        isDecoupling = true;
                        OnMoving.Fire(0f, 1f);
                        OnStop.Fire(1f);
                    }
                }
            }
        }

        IEnumerator DelayedDecouple()
        {
            yield return new WaitForSeconds(EventTime);
            this.animationComplete = true;
            this.OnStop.Fire(1f);
            OnDecouple();
        }

        private void OnDestroy()
        {
            GameEvents.onVesselWasModified.Remove(OnVesselWasModified);
        }

        //
        // Properties
        //
        private EventData<float, float> OnMovingEvent;
        private EventData<float> OnStoppedEvent;

        float EventTime
        {
            get
            {
                return anim[animationName].length / anim[animationName].speed;
            }
        }

        public bool CanMove
        {
            get
            {
                return true;
            }
        }

        public float GetScalar
        {
            get
            {
                return (object)part.FindAttachNode(this.explosiveNodeID).attachedPart == null || isResetting || isDecoupling ? 1f : 0f;
            }
        }

        public string ScalarModuleID
        {
            get
            {
                return this.moduleID;
            }
        }


        public EventData<float, float> OnMoving
        {
            get
            {
                return OnMovingEvent;
            }
        }

        public EventData<float> OnStop
        {
            get
            {
                return OnStoppedEvent;
            }
        }

        //
        // Methods
        //
        public bool IsMoving()
        {
            if (animationName != "")
            {
                if ((object)anim != null)
                {
                    return anim.IsPlaying(animationName);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void SetScalar(float t)
        {
        }

        public void SetUIRead(bool state)
        {
        }

        public void SetUIWrite(bool state)
        {
        }
    }
}
