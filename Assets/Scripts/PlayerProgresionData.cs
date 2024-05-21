using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProgressionData
{
    public bool hasUnlockedExperimentals;
    public bool hasUnlockedTrainingHall;
    public bool hasUnlockedDrones;

    public PlayerProgressionData(PlayerProgression progression)
    {
        hasUnlockedExperimentals = progression.HasUnlockedExperimentals;
        hasUnlockedDrones = progression.HasUnlockedDrones;
        hasUnlockedTrainingHall = progression.HasUnlockedTrainingHall;
    }
}
