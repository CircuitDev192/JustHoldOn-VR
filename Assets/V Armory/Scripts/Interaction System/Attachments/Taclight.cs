using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

namespace VArmory
{
    public class Taclight : Attachment
    {
        protected Light flashlight;
        protected VolumetricLightBeam volumetric;
        [SerializeField] protected MeshRenderer lightMaterial;

        protected override void Start()
        {
            base.Start();
            flashlight = GetComponentInChildren<Light>();
            volumetric = GetComponentInChildren<VolumetricLightBeam>();
            if(!lightMaterial) lightMaterial = GetComponentInChildren<MeshRenderer>();
        }

        public override void Use()
        {
            base.Use();
            flashlight.enabled = !flashlight.enabled;
            volumetric.enabled = !volumetric.enabled;
            lightMaterial.enabled = !lightMaterial.enabled;
        }
    }
}
