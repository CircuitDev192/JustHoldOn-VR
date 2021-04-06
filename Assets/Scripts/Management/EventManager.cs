using System;
using UnityEngine;

public static class EventManager
{
    #region UIEvents

    // Game should resume
    public static Action UIResumeClicked;
    public static void TriggerUIResumeClicked() { UIResumeClicked?.Invoke(); }

    // Game should quit
    public static Action UIQuitClicked;
    public static void TriggerUIQuitClicked() { UIQuitClicked?.Invoke(); }

    // Show Controls Menu
    public static Action UIControlsClicked;
    public static void TriggerUIControlsClicked() { UIControlsClicked?.Invoke(); }

    // Show Settings Menu
    public static Action UISettingsClicked;
    public static void TriggerUISettingsClicked() { UISettingsClicked?.Invoke(); }

    // Leave Controls Menu
    public static Action UIControlsBackClicked;
    public static void TriggerUIControlsBackClicked() { UIControlsBackClicked?.Invoke(); }

    // Leave Settings Menu
    public static Action UISettingsBackClicked;
    public static void TriggerUISettingsBackClicked() { UISettingsBackClicked?.Invoke(); }

    // Should mouse be hidden/locked
    public static Action<bool> MouseShouldHide;
    public static void TriggerMouseShouldHide(bool shouldHide) { MouseShouldHide?.Invoke(shouldHide); }

    #endregion

    #region Game Manager Events

    // Game State Changed Event
    public static Action<GameState> GameStateChanged;
    public static void TriggerGameStateChanged(GameState gameState) { GameStateChanged?.Invoke(gameState); }

    // Scene Loaded Event
    public static Action<string> SceneLoaded;
    public static void TriggerSceneLoaded(string sceneName) { SceneLoaded?.Invoke(sceneName); }

    // Scene UnLoadedEvent
    public static Action<string> SceneUnLoaded;
    public static void TriggerSceneUnLoaded(string sceneName) { SceneUnLoaded?.Invoke(sceneName); }

    #endregion

    #region Mission Events

    // Mission info changed event
    public static Action<string, string> MissionChanged;
    public static void TriggerMissionChanged(string missionTitle, string missionDescription) { MissionChanged?.Invoke(missionTitle, missionDescription); }

    // Mission waypoint changed
    public static Action<Vector3> MissionWaypointChanged;
    public static void TriggerMissionWaypointChanged(Vector3 missionWaypointPosition) { MissionWaypointChanged?.Invoke(missionWaypointPosition); }

    // Player at mission giver
    public static Action PlayerAtMissionGiver;
    public static void TriggerPlayerAtMissionGiver() { PlayerAtMissionGiver?.Invoke(); }

    // Player left mission giver
    public static Action PlayerLeftMissionGiver;
    public static void TriggerPlayerLeftMissionGiver() { PlayerLeftMissionGiver?.Invoke(); }

    // Player spoke to mission giver NPC
    public static Action<string> PlayerSpokeToMissionGiver;
    public static void TriggerPlayerSpokeToMissionGiver(string npcDialog) { PlayerSpokeToMissionGiver?.Invoke(npcDialog); }

    // Mission Completed Event
    public static Action MissionCompleted;
    public static void TriggerMissionCompleted() { MissionCompleted?.Invoke(); }

    // Mission Started
    public static Action StartMission;
    public static void TriggerStartMission() { StartMission?.Invoke(); }

    // Instantiate Next Mission
    public static Action InstantiateNextMission;
    public static void TriggerInstantiateNextMission() { InstantiateNextMission?.Invoke(); }

    // Player at mission area
    public static Action PlayerAtMissionArea;
    public static void TriggerPlayerAtMissionArea() { PlayerAtMissionArea?.Invoke(); }

    // Player left mission area
    public static Action PlayerLeftMissionArea;
    public static void TriggerPlayerLeftMissionArea() { PlayerLeftMissionArea?.Invoke(); }

    // Player cleared zombie area
    public static Action PlayerClearedArea;
    public static void TriggerPlayerClearedArea() { PlayerClearedArea?.Invoke(); }

    // Mission Ended
    public static Action EndMission;
    public static void TriggerEndMission() { EndMission?.Invoke(); }

    // Player collided with mission item
    public static Action<string> PlayerCollidedWithMissionItem;
    public static void TriggerPlayerCollidedWithMissionItem(string itemName) { PlayerCollidedWithMissionItem?.Invoke(itemName); }

    // Player picked up mission item
    public static Action PlayerPickedUpMissionItem;
    public static void TriggerPlayerPickedUpMissionItem() { PlayerPickedUpMissionItem?.Invoke(); }

    // Player left mission item
    public static Action PlayerLeftMissionItem;
    public static void TriggerPlayerLeftMissionItem() { PlayerLeftMissionItem?.Invoke(); }

    // Player collided with mission vehicle
    public static Action<string> PlayerCollidedWithMissionVehicle;
    public static void TriggerPlayerCollidedWithMissionVehicle(string vehicleName) { PlayerCollidedWithMissionVehicle?.Invoke(vehicleName); }

    // Player left mission vehicle
    public static Action PlayerLeftMissionVehicle;
    public static void TriggerPlayerLeftMissionVehicle() { PlayerLeftMissionVehicle?.Invoke(); }

    // Player Entered mission vehicle
    public static Action PlayerEnteredMissionVehicle;
    public static void TriggerPlayerEnteredMissionVehicle() { PlayerEnteredMissionVehicle?.Invoke(); }

    // Final Mission Instantiated
    public static Action FinalMissionInstantiated;
    public static void TriggerFinalMissionInstantiated() { FinalMissionInstantiated?.Invoke(); }

    // Survival Mission Failed
    public static Action SurvivalMissionFailed;
    public static void TriggerSurvivalMissionFailed() { SurvivalMissionFailed?.Invoke(); }

    // Disable flood light generator sounds 
    public static Action DisableFloodLightSounds;
    public static void TriggerDisableFloodLightSounds() { DisableFloodLightSounds?.Invoke(); }

    // Start Survival Countdown
    public static Action StartSurvivalCountdown;
    public static void TriggerStartSurvivalCountdown() { StartSurvivalCountdown?.Invoke(); }

    // Start Helicopter Move
    public static Action StartHelicopterMove;
    public static void TriggerStartHelicopterMove() { StartHelicopterMove?.Invoke(); }

    // Trigger Fade to black anim
    public static Action FadeToBlack;
    public static void TriggerFadeToBlack() { FadeToBlack?.Invoke(); }

    // Start next credits sequence
    public static Action StartNextCreditsSequence;
    public static void TriggerStartNextCreditsSequence() { StartNextCreditsSequence?.Invoke(); }

    // Start credits UI
    public static Action CreditsUI;
    public static void TriggerCreditsUI() { CreditsUI?.Invoke(); }

    // Game Ended
    public static Action GameEnded;
    public static void TriggerGameEnded() { GameEnded?.Invoke(); }

    #endregion

    #region Player Info Events

    // Player damaged event
    // DIFFERENT FROM HEALTH CHANGED
    //  This one triggers on damage (or healing),
    //    the other triggers once the player's new health has been calculated
    public static Action<float> PlayerDamaged;
    public static void TriggerPlayerDamaged(float damage) { PlayerDamaged?.Invoke(damage); }

    // Player health changed event
    public static Action<float> PlayerHealthChanged;
    public static void TriggerPlayerHealthChanged(float health) { PlayerHealthChanged?.Invoke(health); }

    // Player killed event
    public static Action PlayerKilled;
    public static void TriggerPlayerKilled() { PlayerKilled?.Invoke(); }

    // NPC killed event
    public static Action NPCKilled;
    public static void TriggerNPCKilled() { NPCKilled?.Invoke(); }

    // Flashbang Detonated
    public static Action<Vector3, float> FlashbangDetonated;
    public static void TriggerFlashbangDetonated(Vector3 flashbangPosition, float stunDistance) { FlashbangDetonated?.Invoke(flashbangPosition, stunDistance); }

    // Weapon ammo count changed event
    public static Action<int> AmmoCountChanged;
    public static void TriggerAmmoCountChanged(int ammoCount) { AmmoCountChanged?.Invoke(ammoCount); }

    // Weapon changed event
    public static Action<string> WeaponChanged;
    public static void TriggerWeaponChanged(string weapon) { WeaponChanged?.Invoke(weapon); }

    // Flashlight power changed event
    public static Action<float> FlashLightPowerChanged;
    public static void TriggerFlashLightPowerChanged(float power) { FlashLightPowerChanged?.Invoke(power); }

    // Player picked up a suppressor
    public static Action PlayerPickedUpSuppressor;
    public static void TriggerPlayerPickedUpSuppressor() { PlayerPickedUpSuppressor?.Invoke(); }

    // Suppressor durability changed event
    public static Action<float> SuppressorDurabilityChanged;
    public static void TriggerSuppressorDurabilityChanged(float durability) { SuppressorDurabilityChanged?.Invoke(durability); }

    // Suppressor Broken
    public static Action SuppressorBroken;
    public static void TriggerSuppressorBroken() { SuppressorBroken?.Invoke(); }

    // Player walks into weapon pickup
    public static Action<string> PlayerCollidedWithPickup;
    public static void TriggerPlayerCollidedWithPickup(string weaponName) { PlayerCollidedWithPickup?.Invoke(weaponName); }

    // Player walks away from weapon pickup
    public static Action PlayerLeftPickup;
    public static void TriggerPlayerLeftPickup() { PlayerLeftPickup?.Invoke(); }

    // Player picked up a weapon
    public static Action<string> PlayerPickedUpWeapon;
    public static void TriggerPlayerPickedUpWeapon(string previousWeaponName) { PlayerPickedUpWeapon?.Invoke(previousWeaponName); }

    // Player changed the equipped consumable
    public static Action<string> PlayerChangedConsumable;
    public static void TriggerPlayerChangedConsumable(string consumableName) { PlayerChangedConsumable?.Invoke(consumableName); }

    // Player camera changed
    public static Action<Camera> PlayerCameraChanged;
    public static void TriggerPlayerCameraChanged(Camera newCamera) { PlayerCameraChanged?.Invoke(newCamera); }
    
    // Update consumable count value
    public static Action<string, int> UpdateItemCountUI;
    public static void TriggerUpdateItemCountUI(string consumableName, int newValue) { UpdateItemCountUI?.Invoke(consumableName, newValue); }

    #endregion

    // Zombie killed event
    public static Action<GameObject> ZombieKilled;
    public static void TriggerZombieKilled(GameObject zombie) { ZombieKilled?.Invoke(zombie); }

    // Mission Zombie Spawned
    public static Action<GameObject> MissionZombieSpawned;
    public static void TriggerMissionZombieSpawned(GameObject zombie) { MissionZombieSpawned?.Invoke(zombie); }

    // Zombie should despawn event
    public static Action<GameObject> zombieShouldDespawn;
    public static void TriggerZombieShouldDespawn(GameObject zombie) { zombieShouldDespawn?.Invoke(zombie); }

    // Zombie charge event
    public static Action<Transform> ZombieCharge;
    public static void TriggerZombieCharge(Transform chargeTransform) { ZombieCharge?.Invoke(chargeTransform); }

    // Sound Generated Event
    public static Action<Vector3, float> SoundGenerated;
    public static void TriggerSoundGenerated(Vector3 location, float audibleDistance) { SoundGenerated?.Invoke(location, audibleDistance); }
}
