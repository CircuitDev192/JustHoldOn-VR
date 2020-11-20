using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class ItemSFX_Manager : MonoBehaviour
    {
        public List<AudioClip> grabSounds = new List<AudioClip>();
        public List<AudioClip> dropSounds = new List<AudioClip>();

        public virtual void PlayRandomAudioClip(List<AudioClip> list, Vector3 position)
        {
            if (list.Count == 0)
                return;

            AudioClip randomAudioClipClone = list[Random.Range(0, list.Count - 1)];

            if (randomAudioClipClone)
                AudioSource.PlayClipAtPoint(randomAudioClipClone, position);
        }

        public List<AudioSource> lastFX = new List<AudioSource>();

        public int fxIndex;

        public float soundFadeTime = 0.5f;
        public float soundDelayTime = 0.1f;

        public void GetAudioSources(int amount, GameObject audioSourceContainer)
        {
            AudioSource[] tempAudioSources = audioSourceContainer.GetComponents<AudioSource>();

            lastFX.AddRange(tempAudioSources);
        }

        public void RemoveAudioSources(GameObject audioSourceContainer)
        {
            AudioSource[] audioSources = audioSourceContainer.GetComponents<AudioSource>();

            for (int i = 0; i < audioSources.Length; i++)
            {
                if (lastFX.Contains(audioSources[i]))
                {
                    lastFX.Remove(audioSources[i]);

                    IEnumerator fadeRoutine = null;

                    if (fades.ContainsKey(audioSources[i]))
                        fades.TryGetValue(audioSources[i], out fadeRoutine);

                    if (fadeRoutine != null)
                    {
                        StopCoroutine(fadeRoutine);
                        fades.Remove(audioSources[i]);
                        //Debug.Log("Canceled fade");
                    }
                }
            }

            fxIndex = 0;
        }

        public void ClearAudioSources()
        {
            lastFX.Clear();
        }

        Dictionary<AudioSource, IEnumerator> fades = new Dictionary<AudioSource, IEnumerator>();

        protected void PlayRandomAudioClipFade(List<AudioClip> sounds, float delayTime, float fadeTime)
        {
            if (sounds.Count == 0)
                return;

            //Debug.Log("Fade");

            int fadeIndex = fxIndex == 0 ? lastFX.Count - 1 : fxIndex - 1;

            IEnumerator tempFade = FadeOut(lastFX[fadeIndex], delayTime, fadeTime);

            fades.Add(lastFX[fadeIndex], tempFade);

            StartCoroutine(tempFade);

            //Debug.Log("Fade index : " + fadeIndex);
            //Debug.Log("FX index : " + fxIndex);

            AudioClip randomAudioClipClone = sounds[Random.Range(0, sounds.Count - 1)];

            AudioSource tempSource = lastFX[fxIndex];

            if (randomAudioClipClone)
            {
                IEnumerator fadeRoutine = null;

                if (fades.ContainsKey(tempSource))
                    fades.TryGetValue(tempSource, out fadeRoutine);

                if (fadeRoutine != null)
                {
                    StopCoroutine(fadeRoutine);
                    fades.Remove(tempSource);
                    //Debug.Log("Canceled fade");
                }

                tempSource.volume = 1;
                tempSource.spatialBlend = 1;
                tempSource.clip = randomAudioClipClone;
                tempSource.Play();

                fxIndex = fxIndex == lastFX.Count - 1 ? 0 : fxIndex + 1;
            }
        }

        public IEnumerator FadeOut(AudioSource audioSource, float DelayTime, float FadeTime)
        {
            if (audioSource.clip == null)
            {
                //Debug.Log("No Clip To Fade");
                yield break;
            }

            float startVolume = audioSource.volume;

            //Debug.Log("Start fade delay");

            yield return new WaitForSeconds(DelayTime);

            //Debug.Log("Started fade");

            while (audioSource)
            {
                if (audioSource.volume > 0)
                    audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
                else
                    break;

                yield return null;
            }

            if (audioSource)
            {
                audioSource.Stop();
                audioSource.volume = startVolume;
                audioSource.clip = null;

                fades.Remove(audioSource);
                //Debug.Log("Stopped fade");
            }
        }
    }
}