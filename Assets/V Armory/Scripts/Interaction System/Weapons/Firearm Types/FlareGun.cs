using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class FlareGun : HammerActionPistol
    {
        protected FreeSwingHinge hinge;
        protected Collider hingeCollider;

        [SerializeField] protected SteamVR_Action_Boolean openBreakInput;

        protected override void Start()
        {
            base.Start();
            hinge = GetComponentInChildren<FreeSwingHinge>();
            hingeCollider = hinge.GetComponent<Collider>();
            Suppressed = true;
        }

        protected override void Update()
        {
            base.Update();
            LocalInputDown(UnlockHingeWrapper, openBreakInput);
        }

        protected override bool FirePreconditions(int i)
        {
            if (!hinge.locked)
                return false;

            return base.FirePreconditions(i);
        }

        protected override void ChamberBullet(Chamber chamber)
        {
            if (!hinge.locked)
                base.ChamberBullet(chamber);
        }

        protected override void EjectCartridge()
        {
            if (!hinge.locked)
                base.EjectCartridge();
        }

        protected override void TouchpadDownInput()
        {
            TouchPadInput(UnlockHingeWrapper, VArmoryInput.TouchPadDirection.up);
        }

        protected void UnlockHingeWrapper()
        {
            if (!hinge)
                return;

            hinge.Unlock();

            if (chambers[0].Bullet)
                if (chambers[0].Bullet.Spent)
                {
                    Physics.IgnoreCollision(chambers[0].Bullet.Col, hingeCollider);

                    Invoke("EjectCartridge", 0.2f);
                }
        }

    }
}
