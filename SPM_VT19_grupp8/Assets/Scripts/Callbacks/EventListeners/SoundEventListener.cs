using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventListener : EventListenerInterface
{
    public static SoundEventListener SoundListener;
    private List<SoundEventInfo> enemyList = new List<SoundEventInfo>();

    public override void Initialize()
    {
        SoundListener = this;
    }

    public void RegisterPlayerSoundListener(SoundEventInfo sEI)
    {
        enemyList.Add(sEI);
    }

    public void PlayerSound(EventInfo eI)
    {
        SoundEventInfo playerSound = (SoundEventInfo)eI;

    }
}
