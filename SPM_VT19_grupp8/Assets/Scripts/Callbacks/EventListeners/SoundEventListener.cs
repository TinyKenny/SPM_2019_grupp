using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Eventlistener for handling events triggered by sounds like the playersounds, for example that enemies should investigate if they hear the player.
/// </summary>
public class SoundEventListener : EventListenerInterface
{
    void Awake()
    {
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerSoundEventInfo>(PlayerDiegeticSound);
    }

    /// <summary>
    /// Plays a one shot diegetic playersound and checks if any enemies are within range to hear it.
    /// </summary>
    /// <param name="eI"><see cref="PlayerSoundEventInfo"/> representing the player, also needs an audioclip. If the range is more than zero enemies might hear the player.</param>
    public void PlayerDiegeticSound(EventInfo eI)
    {
        PlayerSoundEventInfo playerSound = (PlayerSoundEventInfo)eI;

        if (playerSound.AC != null)
            playerSound.GO.GetComponent<AudioSource>().PlayOneShot(playerSound.AC);

        if (playerSound.Range > 0)
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemySoundEventInfo(playerSound.GO, playerSound.Range));
    }
}
