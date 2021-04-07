using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSurvive : MonoBehaviour
{
    [SerializeField] private string missionTitle;
    [SerializeField] private string missionDescription;
    [SerializeField] private string returnToBaseDescription; //Shows after the player completes the objective
    [SerializeField] public string npcDialog; //What the NPC tells the player at the start of the mission
    [SerializeField] public string npcDialog2; //Second half of dialog. Specific to the last mission..
    [SerializeField] private Transform missionObjectiveLocation;
    [SerializeField] private Transform missionObjective2Location;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private int zombiesToSpawn;
    [SerializeField] private bool shouldSpawnZombiesAtMissionArea;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private Transform[] flareSpawnLocations; // must have 4 total
    [SerializeField] private Transform chargeTransform; // will lure the zombies inside the police station

    // Start is called before the first frame update
    void Start()
    {
        EventManager.EndMission += EndMission;
        EventManager.PlayerEnteredMissionVehicle += PlayerEnteredMissionVehicle;

        EventManager.TriggerMissionChanged(missionTitle, missionDescription);
        EventManager.TriggerMissionWaypointChanged(missionObjectiveLocation.position);
        EventManager.TriggerStartMission();
        EventManager.TriggerFinalMissionInstantiated();
    }

    private void PlayerEnteredMissionVehicle()
    {
        ZombieSpawnManager.instance.SetMissionZombieSpawns(spawnPoints, zombiesToSpawn, shouldSpawnZombiesAtMissionArea, true);
        EventManager.TriggerMissionWaypointChanged(missionObjective2Location.position);

        StartCoroutine(FireFlares());

        StartCoroutine(StartZombieCharge());

        EventManager.TriggerStartSurvivalCountdown();
    }

    private void EndMission()
    {
        EventManager.TriggerMissionChanged("", "");
        this.gameObject.SetActive(false);
        EventManager.EndMission -= EndMission;
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
        //Destroy(this.gameObject);
    }

    IEnumerator StartZombieCharge()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            EventManager.TriggerZombieCharge(chargeTransform);
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator FireFlares()
    {
        yield return new WaitForSeconds(5f);
        GameObject flare = Instantiate(flarePrefab, flareSpawnLocations[0].position, Quaternion.identity);
        flare.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0.05f, 1f, 0.1f) * 3000f);

        yield return new WaitForSeconds(1f);
        flare = Instantiate(flarePrefab, flareSpawnLocations[1].position, Quaternion.identity);
        flare.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-0.05f, 0.9f, 0.15f) * 3000f);

        yield return new WaitForSeconds(0.3f);
        flare = Instantiate(flarePrefab, flareSpawnLocations[2].position, Quaternion.identity);
        flare.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-0.05f, 0.9f, 0.05f) * 3000f);

        yield return new WaitForSeconds(1f);
        flare = Instantiate(flarePrefab, flareSpawnLocations[3].position, Quaternion.identity);
        flare.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-0.05f, 0.8f, 0.05f) * 3000f);
    }

    private void OnDestroy()
    {
        EventManager.EndMission -= EndMission;
        EventManager.PlayerEnteredMissionVehicle -= PlayerEnteredMissionVehicle;
    }
}
