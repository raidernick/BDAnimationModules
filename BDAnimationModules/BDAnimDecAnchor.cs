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
    public class BDAnimDecAnchor : ModuleAnchoredDecoupler
    {
        [KSPField]
        public string animationName = "";

        protected Animation anim;

        protected bool isDecoupling;

        protected bool isResetting;

        ModuleCargoBay cargoBay;

        [KSPField()]
        public bool waitForAnimation = false;

        [KSPField(isPersistant = true)]
        public bool animationComplete = false;

        [KSPField]
        public int layer = 0;

        public BDAnimDecAnchor() :
        base()
        {
        }

        public new void DecoupleAction(KSPActionParam param)
        {
            if (waitForAnimation && (object)anim != null)
            {
                anim.Play(animationName);
                isDecoupling = true;
                OnMoving.Fire(0f, 1f);
                StartCoroutine(DelayedDecouple());
            }
            else
                OnDecouple();
        }

        public new void Decouple()
        {
            if (waitForAnimation && (object)anim != null)
            {
                anim.Play(animationName);
                isDecoupling = true;
                OnMoving.Fire(0f, 1f);
                StartCoroutine(DelayedDecouple());
            }
            else
                OnDecouple();
        }


        public override void OnAwake()
        {
            this.OnMovingEvent = new EventData<float, float>("BDAnimDecAnchor.OnMovingEvent");
            this.OnStoppedEvent = new EventData<float>("BDAnimDecAnchor.OnStoppedEvent");
            base.OnAwake();
        }

        public override void OnStart(StartState state)
        {
            GameEvents.onVesselWasModified.Add(OnVesselWasModified);
            cargoBay = part.FindModuleImplementing<ModuleCargoBay>();
            base.OnStart(state);
            Debug.Log("BDAnimDecAnchor.OnStart(), isDecoupled = " + isDecoupled.ToString());
            if (animationName != "")
            {
                anim = part.FindModelAnimators(animationName).FirstOrDefault();
                if ((object)anim == null)
                {
                    Debug.Log("BDAnimDecAnchor: Animations not found");
                }
                else
                {
                    Debug.Log("BDAnimDecAnchor.OnStart() - Animation found named " + animationName);
                    if (this.animationComplete || this.isDecoupled)
                    // If Decoupled or animation already played then set animation to end.
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
                if (waitForAnimation && (object)anim != null)
                {
                    anim.Play(animationName);
                    isDecoupling = true;
                    OnMoving.Fire(0f, 1f);
                    StartCoroutine(DelayedDecouple());
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

                    Part p;
                    if (this.explosiveNodeID == "")
                        p = part.srfAttachNode.attachedPart;
                    else
                        p = part.FindAttachNode(this.explosiveNodeID).attachedPart;
                    if (p = null)
                    {
                        isDecoupling = true;
                        OnMoving.Fire(0f, 1f);
                        OnStop.Fire(1f);
                    }
                }
            }
        }

        // TODO Consider deprecating checkForDecoupling; it should no longer be necessary
        private void checkForDecoupling(EventReport separationData)
        {
            if (separationData.eventType == FlightEvents.STAGESEPARATION && separationData.origin == this.part)
            {
                OnMoving.Fire(0f, 1f);
                if (animationName != "" && (object)anim != null && (!this.animationComplete || !this.anim.IsPlaying(animationName)))
                {
                    this.anim.Play(animationName);
                    this.animationComplete = true;
                    Debug.Log("BDAnimDecAnchor.onStageSeparation() triggered animation " + this.animationName);
                }
                this.isDecoupling = true;
                this.OnStop.Fire(1f);
            }
            return;
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
                return (isResetting || isDecoupling) ? 1f : 0f;
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
            return false;
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
