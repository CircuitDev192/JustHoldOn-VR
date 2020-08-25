using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class RPG : Firearm
    {
        protected override void Start()
        {
            base.Start();

            if (magWell)
            {
                magWell._OnGrab -= RemoveMag;
                magWell._OnLoad -= LoadMag;
                magWell._OnReachedStart -= RemoveMag;

                magWell._OnGrab += ClearChamber;
                magWell._OnReachedStart += ClearChamber;
                magWell._OnLoad += LoadBulletIntoChamber;

                magWell._OnReachedStart += DetachMagSFX;
            }
        }

        protected void ClearChamber()
        {
            chambers[0].EjectBulletLight();
        }

        protected void LoadBulletIntoChamber(Item bullet)
        {
            if (bullet.GetType() != typeof(Bullet)) return;

            chambers[0].ChamberBullet(bullet as Bullet);
        }

        protected override bool Fire(int i)
        {
            if (base.Fire(i))
            {
                fireSelector.SwitchFireMode();

                Bullet tempBullet = chambers[0].Bullet;

                chambers[0].EjectBulletLight();

                Destroy(tempBullet.gameObject);

                magWell.ClearSlider();

                return true;
            }

            return false;
        }
    }
}