using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory{

    public class RandomAudioClipPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] clips;

        InteractionVolume iv;

        private void Start()
        {
            iv = GetComponent<InteractionVolume>();

            if (!iv)
            {
                Debug.LogError("Interaction Volume on Magazine Spawner is invalid");
            }

            iv._StartInteraction += PlayClip;
        }

        public void PlayClip()
        {
            iv.StopInteraction();

            AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length - 1)], this.transform.position);
        }
    }
}
