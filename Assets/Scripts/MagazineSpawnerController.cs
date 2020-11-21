using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VArmory
{
    public class MagazineSpawnerController : MonoBehaviour
    {
        [SerializeField] private GameObject magazineToSpawn;
        [SerializeField] private Transform spawnPoint;

        InteractionVolume iv;

        private void Start()
        {
            iv = GetComponent<InteractionVolume>();

            if (!iv)
            {
                Debug.LogError("Interaction Volume on Magazine Spawner is invalid");
            }

            iv._StartInteraction += SpawnMagazine;
        }

        public void SpawnMagazine()
        {
            iv.StopInteraction();
            Instantiate(magazineToSpawn, spawnPoint.position, Quaternion.identity);
        }
    }
}
