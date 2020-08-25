using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    [System.Serializable]
    public class RecoilManager : MonoBehaviour
    {
        public Transform recoilPositionOffset;
        public Transform recoilRotationOffset;

        [System.Serializable]
        public struct Recoil
        {
            public RecoilAxis x;
            public RecoilAxis y;
            public RecoilAxis z;
        }

        [SerializeField] protected Recoil translation;
        [SerializeField] protected Recoil rotation;

        float sustainedFireTimeStamp;
        float increaseRecoilTimeStamp;
        [SerializeField] protected float firstShotDelay;

        public delegate void FirstShotEvent();

        public FirstShotEvent _OnFirstShot;
        public FirstShotEvent _OnSustainedFire;

        public void ClearRecoil()
        {
            translation.x.ResetAxis();
            translation.y.ResetAxis();
            translation.z.ResetAxis();
            rotation.x.ResetAxis();
            rotation.y.ResetAxis();
            rotation.z.ResetAxis();
        }

        public void SetSelf(Item self)
        {
            SetRecoilSelf(ref translation, self);
            SetRecoilSelf(ref rotation, self);
        }

        public void ScaleRotation(float maxDamp, float incrementDamp, float decreaseTargetDamp, float increaseDamp, float decreaseDamp)
        {
            UnscaleRotation();
            ScaleRecoil(maxDamp, incrementDamp, decreaseTargetDamp, increaseDamp, decreaseDamp, ref rotation);
        }

        public void ScaleTranslation(float maxDamp, float incrementDamp, float decreaseTargetDamp, float increaseDamp, float decreaseDamp)
        {
            UnscaleTranslation();
            ScaleRecoil(maxDamp, incrementDamp, decreaseTargetDamp, increaseDamp, decreaseDamp, ref translation);
        }

        public void ScaleRecoil(float maxScale, float incrementScale, float decreaseTargetScale, float increaseScale, float decreaseScale,
                                       ref Recoil recoil)
        {
            ScaleRecoilAxis(maxScale, incrementScale, decreaseTargetScale, increaseScale, decreaseScale, ref recoil.x);
            ScaleRecoilAxis(maxScale, incrementScale, decreaseTargetScale, increaseScale, decreaseScale, ref recoil.y);
            ScaleRecoilAxis(maxScale, incrementScale, decreaseTargetScale, increaseScale, decreaseScale, ref recoil.z);
        }

        void ScaleRecoilAxis(float maxScale, float incrementScale, float decreaseTargetScale, float increaseScale, float decreaseScale,
                         ref RecoilAxis recoil)
        {
            recoil.twoHandMaxScale *= maxScale;
            recoil.twoHandIncrementScale *= incrementScale;
            recoil.twoHandDecreaseTargetScale *= decreaseTargetScale;
            recoil.twoHandIncreaseScale *= increaseScale;
            recoil.twoHandDecreaseScale *= decreaseScale;
        }

        public void UnscaleRotation() { UnscaleRecoil(ref rotation); }

        public void UnscaleTranslation() { UnscaleRecoil(ref translation); }

        public void UnscaleRecoil(ref Recoil recoil)
        {
            UnscaleRecoilAxis(ref recoil.x);
            UnscaleRecoilAxis(ref recoil.y);
            UnscaleRecoilAxis(ref recoil.z);
        }

        public void UnscaleRecoilAxis(ref RecoilAxis recoil)
        {
            recoil.SetInitial();

            //ScaleRecoilAxis(
            //	recoil.initial.max / (recoil.twoHandMaxScale != 0 ? recoil.twoHandMaxScale : 1),
            //	recoil.initial.increment / (recoil.twoHandIncrementScale != 0 ? recoil.twoHandIncrementScale : 1),
            //	recoil.initial.decreaseTarget / (recoil.twoHandDecreaseTargetScale != 0 ? recoil.twoHandMaxScale : 1),
            //	recoil.initial.increase / (recoil.twoHandIncreaseScale != 0 ? recoil.twoHandIncreaseScale : 1),
            //	recoil.initial.decrease / (recoil.twoHandDecreaseScale != 0 ? recoil.twoHandDecreaseScale : 1),
            //	ref recoil);
        }

        public void ApplyRecoil()
        {
            if (!recoilPositionOffset)
                return;

            ApplyRecoil(ref translation);
            ApplyRecoil(ref rotation);

            recoilPositionOffset.localPosition = new Vector3(translation.x.Current, translation.y.Current, translation.z.Current);
            recoilRotationOffset.localRotation = Quaternion.Euler(new Vector3(rotation.x.Current, rotation.y.Current, rotation.z.Current));

            DecreaseRecoil(ref translation);
            DecreaseRecoil(ref rotation);
        }

        void ApplyRecoil(ref Recoil recoil)
        {
            recoil.x.ApplyRecoil();
            recoil.y.ApplyRecoil();
            recoil.z.ApplyRecoil();
        }

        public void IncreaseAllRecoil()
        {
            float timeSinceLastRecoilIncrease = Time.time - increaseRecoilTimeStamp;

            if (timeSinceLastRecoilIncrease > firstShotDelay)
            {
                if (_OnFirstShot != null)
                    _OnFirstShot();

                sustainedFireTimeStamp = Time.time;
            }
            else
            {
                if (_OnSustainedFire != null)
                    _OnSustainedFire();
            }

            IncreaseRecoil(ref translation, Random.Range(0.0f, 1.0f));
            IncreaseRecoil(ref rotation, Random.Range(0.0f, 1.0f));

            increaseRecoilTimeStamp = Time.time;
        }

        void IncreaseRecoil(ref Recoil recoil, float syncedPolarity)
        {
            recoil.x.IncreaseRecoil(syncedPolarity <= rotation.x.IncreaseChance ? 1 : -1, sustainedFireTimeStamp);
            recoil.y.IncreaseRecoil(syncedPolarity <= rotation.y.IncreaseChance ? 1 : -1, sustainedFireTimeStamp);
            recoil.z.IncreaseRecoil(syncedPolarity <= rotation.z.IncreaseChance ? 1 : -1, sustainedFireTimeStamp);
        }

        void DecreaseRecoil(ref Recoil recoil)
        {
            recoil.x.DecreaseRecoil();
            recoil.y.DecreaseRecoil();
            recoil.z.DecreaseRecoil();
        }

        void SetRecoilSelf(ref Recoil recoil, Item self)
        {
            recoil.x.self = self;
            recoil.y.self = self;
            recoil.z.self = self;

            recoil.x.Initialize();
            recoil.y.Initialize();
            recoil.z.Initialize();
        }
    }
}