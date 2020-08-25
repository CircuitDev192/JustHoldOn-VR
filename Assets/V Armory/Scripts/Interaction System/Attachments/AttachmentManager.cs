using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class AttachmentManager : MonoBehaviour
    {
        [SerializeField] protected AttachmentSocket[] sockets = new AttachmentSocket[0];

        public delegate void OnAttach(Attachment attachment);
        public OnAttach _OnAttach;

        public delegate void OnDetach(Attachment attachment);
        public OnDetach _OnDetach;

        void Awake()
        {
            sockets = GetComponentsInChildren<AttachmentSocket>();

            if (sockets == null)
                Destroy(this);

            foreach (AttachmentSocket socket in sockets)
            {
                socket._OnAttach += AttachWrapper;
                socket._OnDetach += DetachWrapper;
            }

            ToggleAttachments(true);
        }

        void AttachWrapper(Attachment attachment) { if (_OnAttach != null) _OnAttach(attachment); }
        void DetachWrapper(Attachment attachment) { if (_OnDetach != null) _OnDetach(attachment); }

        void ToggleAttachments(bool enable)
        {
            foreach (AttachmentSocket socket in sockets)
            {
                if (socket.HasItem)
                {
                    if (enable)
                        socket.EnableAttachment();
                    else
                        socket.DisableAttachment();
                }
            }
        }
    }
}