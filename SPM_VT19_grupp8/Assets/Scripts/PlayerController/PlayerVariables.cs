using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerVariables
{
    public PositionInfo SpawnPosition { get; set; }
    public float SpawnRotationY { get; set; }
    public int AmmoAmount { get; set; } = 0;
    public float ShieldAmount { get; set; }
    public float TimeSlowEnergy { get; set; }
    public float ShieldCooldown { get; set; }

    public PlayerVariables(Vector3 playerPosition, float playerRotationY, int playerAmmo, float playerShieldAmount, float playerTimeSlowEnergy, float shieldCooldown)
    {
        SpawnPosition = new PositionInfo(playerPosition);
        SpawnRotationY = playerRotationY;
        AmmoAmount = playerAmmo;
        ShieldAmount = playerShieldAmount;
        TimeSlowEnergy = playerTimeSlowEnergy;
        ShieldCooldown = shieldCooldown;
    }
}
