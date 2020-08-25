﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class FirearmSFX_Manager : ItemSFX_Manager
    {

        [Header("Sound Effects")]
        public List<AudioClip> actionBackSounds = new List<AudioClip>();
        public List<AudioClip> actionFowardSounds = new List<AudioClip>();
        public List<AudioClip> fireSounds = new List<AudioClip>();
        public List<AudioClip> suppressedFireSounds = new List<AudioClip>();
        public List<AudioClip> dryFireSounds = new List<AudioClip>();
        public List<AudioClip> loadMagazineSounds = new List<AudioClip>();
        public List<AudioClip> unloadMagazineSounds = new List<AudioClip>();
        public List<AudioClip> restOnSlideStopSounds = new List<AudioClip>();

        public void FireFX(bool suppressed)
        {
            List<AudioClip> sounds = suppressed ? suppressedFireSounds : fireSounds;
            PlayRandomAudioClipFade(sounds, soundFadeTime, soundDelayTime);
        }
    }
}