using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class Sight : Attachment
    {
        [SerializeField] protected GameObject reticle;

        public override void Enable() { reticle.SetActive(true); }

        public override void Disable() { reticle.SetActive(false); }
    }
}
