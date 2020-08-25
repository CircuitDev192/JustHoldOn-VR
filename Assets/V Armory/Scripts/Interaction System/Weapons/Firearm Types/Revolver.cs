using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class Revolver : HammerActionPistol
    {
        protected FreeSwingHinge cylHinge;
        [SerializeField] protected Transform cylinder;

        protected RevolverHammer revolverHammer;
        [SerializeField] protected InteractionVolume extractorRod;

        [SerializeField] protected float ejectBulletsVelocityThreshold = 1;

        [SerializeField] protected TwitchExtension.DotAxis pullToEjectDirection;

        [SerializeField] protected SteamVR_Action_Boolean openCylinderInput;
        [SerializeField] protected VArmoryInput.TouchPadDirection openCylinderDirection;

        protected override void Start()
        {
            base.Start();

            if (!cylHinge) cylHinge = GetComponentInChildren<FreeSwingHinge>();
            if (!revolverHammer) revolverHammer = GetComponentInChildren<RevolverHammer>();

            hammer = revolverHammer;

            revolverHammer.Chambers = chambers.Count;

            revolverHammer._OnCockHammer += AdvanceChamberIndex;
            revolverHammer.Hinge = cylHinge;
            revolverHammer.Cyl = cylinder;

            if (extractorRod)
            {
                cylHinge._Close += RestrainExtractorRod;
                cylHinge._Open += FreeExtractorRod;

                extractorRod._StartInteraction += ExtractorRod;
            }
        }

        protected virtual void RestrainExtractorRod()
        {
            extractorRod.restrained = true;
            cylHinge.Restrained = true;
        }

        protected virtual void FreeExtractorRod()
        {
            extractorRod.restrained = false;
            cylHinge.Restrained = false;
        }

        protected virtual void ExtractorRod()
        {
            if (!cylHinge.locked)
            {
                bool hasSpentShell = false;

                foreach (Chamber chamber in chambers)
                {
                    if (chamber.Bullet != null)
                        if (chamber.Bullet.Spent)
                        {
                            hasSpentShell = true;
                            break;
                        }
                }

                if (hasSpentShell)
                    EjectShells();
                else
                {
                    Hand tempHand = extractorRod.Hand;
                    extractorRod.StopInteraction();
                    cylHinge.ForceStart(tempHand);
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!PrimaryHand)
                return;

            Vector3 tempVelocity = velocityHistory._ReleaseVelocity;
            Vector3 tempCharVelocity = PrimaryHand.CharControllerVelocity;

            //If the revolver is being pulled back fast enough eject the shells
            if ((tempVelocity - tempCharVelocity).magnitude > ejectBulletsVelocityThreshold &&
                Vector3.Dot(TwitchExtension.ReturnAxis(pullToEjectDirection, transform), tempVelocity.normalized) > 0.75f)
                EjectShells();

            if (!hammer.Cocked && !hammer.Firing)
                LocalInputDown(cylHinge.Unlock, openCylinderInput);
        }

        protected override void TouchpadDownInput()
        {
            base.TouchpadDownInput();

            if (!hammer.Cocked && !hammer.Firing)
                TouchPadInput(cylHinge.Unlock, openCylinderDirection);
        }

        protected void EjectShells()
        {
            if (cylHinge.locked)
                return;

            for (int i = 0; i < chambers.Count; i++)
                if (chambers[i].Bullet)
                    if (chambers[i].Bullet.Spent)
                        chambers[i].EjectBullet(velocityHistory._ReleaseVelocity);
        }

        protected override bool FirePreconditions()
        {
            if (hammer)
                if (hammer.Firing)
                    pulledTriggerWhileSlideWasBack = true;

            if (chambers[selectedChamberIndex].Bullet)
                if (!chambers[selectedChamberIndex].Bullet.Spent)
                    if (cylHinge.locked)
                        return true;

            return false;
        }

        protected override void ChamberBullet(Chamber chamber)
        {
            if (!cylHinge.locked)
                base.ChamberBullet(chamber);
        }
    }
}