using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class RespawnItemManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] items;

        private List<Vector3> itemStartPos = new List<Vector3>();
        private List<Quaternion> itemStartRot = new List<Quaternion>();

        private float timeToCheckDistance = 15f;
        private float timer = 0;
         
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
                itemStartPos.Add(items[i].transform.position);
                itemStartRot.Add(items[i].transform.rotation);
            }

            timer = timeToCheckDistance;
        }

        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                TeleItems();            
                timer = timeToCheckDistance;
            }
        }

        public void TeleItems()
        {
            iv.StopInteraction();
            Vector3 playerPos = PlayerManager.instance.player.transform.position;
            for (int i = 0; i < items.Length; i++)
            {
                if(Vector3.Distance(playerPos, items[i].transform.position) >= 3f && Vector3.Distance(items[i].transform.position, itemStartPos[i]) >= 3f)
                {
                    items[i].transform.position = itemStartPos[i];
                    items[i].transform.rotation = itemStartRot[i];
                }
            }

        }
    }
}
