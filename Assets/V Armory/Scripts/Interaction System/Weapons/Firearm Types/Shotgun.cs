using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Shotgun : Firearm
    {
        protected override void Start()
        {
            base.Start();

            if (magWell)
            {
                magWell._OnGrab -= RemoveMag;
                magWell._OnLoad -= LoadMag;
                magWell._OnReachedStart -= RemoveMag;

                magWell._OnLoad += magazine.LoadBullet;
                magWell._OnLoad += LimitInternalMag;
                magWell._OnLoad += ClearMagWell;
                magWell._OnLoad += LoadSFX;
            }

            LimitFullInternalMag();
        }

        void ClearMagWell(Item item) { magWell.ClearSlider(); }

        void LimitInternalMag(Item item) { LimitFullInternalMag(); }

        void LimitFullInternalMag() { magWell.minSliderDistance = magazine.Full ? 0.75f : 0; }

        void LoadSFX(Item item)
        {
            if (firearmAudioManager) firearmAudioManager.PlayRandomAudioClip(firearmAudioManager.loadMagazineSounds, magazine.transform.position);
        }

        protected override void ChamberRoundFromMagazine()
        {
            base.ChamberRoundFromMagazine();
            LimitFullInternalMag();
        }
    }
}