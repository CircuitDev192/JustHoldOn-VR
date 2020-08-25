using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Taclight : Attachment
    {
        protected Light flashlight;
        [SerializeField] protected MeshRenderer lightMaterial;

        protected override void Start()
        {
            base.Start();
            flashlight = GetComponentInChildren<Light>();
            if(!lightMaterial) lightMaterial = GetComponentInChildren<MeshRenderer>();
        }

        public override void Use()
        {
            base.Use();
            flashlight.enabled = !flashlight.enabled;
            lightMaterial.enabled = !lightMaterial.enabled;
        }
    }
}
