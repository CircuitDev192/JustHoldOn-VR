using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class CombinationOptic : Attachment
    {
        private Camera scopeCamera;
        [SerializeField] protected GameObject scopeTexture;

        [SerializeField] protected GameObject reticle;

        protected override void Start() { base.Start(); scopeCamera = GetComponentInChildren<Camera>(); }

        public override void Enable()
        {
            scopeCamera.enabled = true;
            scopeTexture.SetActive(true);
            reticle.SetActive(true);
        }

        public override void Disable()
        {
            scopeCamera.enabled = false;
            scopeTexture.SetActive(false);
            reticle.SetActive(false);
        }
    }
}
