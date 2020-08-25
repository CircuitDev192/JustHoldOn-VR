using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class Attachment : Item
    {
        [SerializeField] protected InteractionVolume useAttachmentIV;

        [SerializeField] protected List<AudioClip> useSounds = new List<AudioClip>();

        [SerializeField] protected SteamVR_Action_Boolean useAttachmentInput;

        public enum Type
        {
            other,
            sight,
            scope,
            foregrip,
            suppressor
        }

        public Type type;

        public Vector3 AttachmentPosOffset;
        public Vector3 AttachmentRotOffset;

        public TwitchExtension.DotAxis primaryAttachDotAxis = TwitchExtension.DotAxis.forward;
        public TwitchExtension.DotAxis secondaryAttachDotAxis = TwitchExtension.DotAxis.right;

        public virtual void Use() { TwitchExtension.PlayRandomAudioClip(useSounds, transform.position); }

        public virtual void Enable() { }

        public virtual void Disable() { }

        protected override void Start()
        {
            base.Start();
            if(useAttachmentIV) useAttachmentIV._StartInteraction += Use;
        }

        protected override void Update()
        {
            base.Update();

            LocalInputDown(Use, useAttachmentInput);
        }
    }
}