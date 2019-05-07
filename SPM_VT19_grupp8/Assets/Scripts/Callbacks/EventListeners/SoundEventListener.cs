using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventListener : EventListenerInterface
{
    public static SoundEventListener SoundListener;

    public override void Initialize()
    {
        SoundListener = this;
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<PlayerSoundEventInfo>(PlayerSound);
    }

    public void PlayerSound(EventInfo eI)
    {
        PlayerSoundEventInfo playerSound = (PlayerSoundEventInfo)eI;

        if (playerSound.AC != null)
            playerSound.GO.GetComponent<AudioSource>().PlayOneShot(playerSound.AC);

        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemySoundEventInfo(playerSound.GO, playerSound.Range));
    }
}
