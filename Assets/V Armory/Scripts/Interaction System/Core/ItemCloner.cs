using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class ItemCloner : MonoBehaviour
    {
        InteractionVolume iv;

        [SerializeField] protected Item prefab;

        void Start()
        {
            iv = GetComponent<InteractionVolume>();
            iv._StartInteraction += GrabCloneWrapper;
        }

        void GrabCloneWrapper()
        {
            StartCoroutine(GrabClone());
        }

        IEnumerator GrabClone()
        {
            Hand tempHand = iv.Hand;

            iv.StopInteraction();

            Item clone = Instantiate(prefab, transform.position, transform.rotation);

            yield return new WaitForFixedUpdate();

            clone.Attach(tempHand);
        }
    }
}