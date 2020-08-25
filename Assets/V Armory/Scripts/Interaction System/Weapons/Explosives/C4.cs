using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class C4 : ExplosiveItem
    {
        [SerializeField] protected AudioClip arming;
        [SerializeField] protected AudioClip disarming;

        [SerializeField] protected SteamVR_Action_Boolean armInput;

        [SerializeField] protected Transform armingSwitch;
        [SerializeField] protected Vector3 armedSwitchRotation;
        [SerializeField] protected Vector3 unarmedSwitchRotation;

        [SerializeField] protected MeshRenderer armingLight;
        [SerializeField] protected Material armedMaterial;
        [SerializeField] protected Material unarmedMaterial;


        protected override void Start()
        {
            base.Start();
            explosive = GetComponent<Explosive>();
        }

        protected override void Update()
        {
            base.Update();

            if (!PrimaryHand) return;

            LocalInputDown(Arm, armInput);
        }

        Detonator detonator;

        public override void Arm()
        {
            Debug.Log("Arm");
            if (!armed && !detonator)
            {
                Item tempItem = PrimaryHand.Sibling.StoredItem;

                if (tempItem)
                    if (tempItem.GetType() == typeof(Detonator))
                    {
                        Detonator tempDetonator = PrimaryHand.Sibling.StoredItem as Detonator;

                        detonator = tempDetonator;
                        detonator._Detonate += Explode;

                        armed = true;

                        if(arming) AudioSource.PlayClipAtPoint(arming, transform.position);
                    }
            }
            else if(armed && detonator)
            {
                detonator._Detonate -= Explode;
                detonator = null;

                armed = false;

                if (disarming) AudioSource.PlayClipAtPoint(disarming, transform.position);
            }

            if (armingSwitch)
                armingSwitch.localRotation = Quaternion.Euler(armed ? armedSwitchRotation : unarmedSwitchRotation);

            if (armingLight)
                armingLight.sharedMaterial = armed ? armedMaterial : unarmedMaterial;

        }

        public override void Explode()
        {
            detonator._Detonate -= Explode;
            base.Explode();
        }
    }
}