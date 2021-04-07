using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class HandheldUsableItem : Item
    {

        [SerializeField] protected List<AudioClip> useSounds = new List<AudioClip>();

        [SerializeField] protected SteamVR_Action_Boolean useItemInput;

        public virtual void Use() { TwitchExtension.PlayRandomAudioClip(useSounds, transform.position); }

        public virtual void Enable() { }

        public virtual void Disable() { }

        protected override void Start()
        {
            base.Start();
            primaryGrip._StartInteraction += Use;
        }

        protected override void Update()
        {
            base.Update();

            LocalInputDown(Use, useItemInput);
        }
    }
}