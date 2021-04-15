using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VArmory
{
    public class FireSelector : MonoBehaviour
    {
        public enum _FireMode
        {
            safety,
            semi,
            full,
            burst
        }

        [Header("Fire Selector Settings")]

        [SerializeField] protected _FireMode fireMode;
        public _FireMode FireMode { get { return fireMode; } }
        [SerializeField] protected List<_FireMode> availableFireModes = new List<_FireMode>();
        [SerializeField] protected List<Vector3> fireSelectorRotations;

        [SerializeField] protected float safetySlideLimit = -1;
        public float SafetySlideLimit { get { return safetySlideLimit; } }

        [SerializeField] protected VArmoryInput.TouchPadDirection touchpadDirection;
        public VArmoryInput.TouchPadDirection _TouchPadDirection { get { return touchpadDirection; } }

        protected int selectedFireModeIndex;

        [SerializeField] protected GameObject[] safetyIndicators;

        [SerializeField] protected List<AudioClip> switchFireModeSounds = new List<AudioClip>();

        [SerializeField] protected SteamVR_Action_Boolean fireSelectorInput;
        public SteamVR_Action_Boolean FireSelectorInput { get { return fireSelectorInput; } }

        void Start()
        {
            selectedFireModeIndex = availableFireModes.IndexOf(fireMode);

            if (availableFireModes.Count != fireSelectorRotations.Count)
                fireSelectorRotations.AddRange(new Vector3[availableFireModes.Count - fireSelectorRotations.Count]);
        }

        public virtual void SwitchFireMode()
        {
            if (availableFireModes.Count - 1 <= 0)
                return;

            selectedFireModeIndex = selectedFireModeIndex < availableFireModes.Count - 1 ? selectedFireModeIndex + 1 : 0;

            //if selected = 0 set safety indicator to green. else, red.
            if (selectedFireModeIndex == 0)
            {
                safetyIndicators[0].SetActive(true);
                safetyIndicators[1].SetActive(false);
            }
            else
            {
                safetyIndicators[0].SetActive(false);
                safetyIndicators[1].SetActive(true);
            }

            fireMode = availableFireModes[selectedFireModeIndex];

            transform.localEulerAngles = fireSelectorRotations[selectedFireModeIndex];

            TwitchExtension.PlayRandomAudioClip(switchFireModeSounds, transform.position);
        }

        public _FireMode NextFireMode()
        {
            return availableFireModes[selectedFireModeIndex < availableFireModes.Count - 1 ? selectedFireModeIndex + 1 : 0];
        }
    }
}