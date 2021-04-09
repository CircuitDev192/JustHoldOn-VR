using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class RespawnItemManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] items;

        private List<Transform> itemStartPos = new List<Transform>();

        private float timeToCheckDistance = 30f;
        private float timer = 0;
        private bool shouldTeleItems = false;
         
        InteractionVolume iv;

        private void Start()
        {
            iv = GetComponent<InteractionVolume>();

            if (!iv)
            {
                Debug.LogError("Interaction Volume on Magazine Spawner is invalid");
            }

            iv._StartInteraction += TeleItems;

            for (int i = 0; i < items.Length; i++)
            {
                itemStartPos.Add(items[i].transform);
            }

            timer = timeToCheckDistance;
            shouldTeleItems = true;
        }

        private void LateUpdate()
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                if (shouldTeleItems)
                {
                    Debug.Log("Teleporting lost items back to base.");
                    TeleItems();
                    //shouldTeleItems = false;
                }
                timer = timeToCheckDistance;
            }
        }

        public void TeleItems()
        {
            iv.StopInteraction();
            Vector3 playerPos = PlayerManager.instance.player.transform.position;
            for (int i = 0; i < items.Length; i++)
            {
                if(Vector3.Distance(playerPos, items[i].transform.position) > 2f)
                {
                    items[i].transform.position = itemStartPos[i].position;
                    items[i].transform.rotation = itemStartPos[i].rotation;
                }
            }

        }
    }
}
