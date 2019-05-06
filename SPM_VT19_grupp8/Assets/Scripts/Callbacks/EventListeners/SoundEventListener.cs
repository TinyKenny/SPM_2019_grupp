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

        EventCoordinator.CurrentEventCoordinator.ActivateEvent(new EnemySoundEventInfo(playerSound.GO, playerSound.Range));
    }
}
