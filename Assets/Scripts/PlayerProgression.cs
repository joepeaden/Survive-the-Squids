using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance => instance;
    static PlayerProgression instance;

    public bool HasUnlockedExperimentals => hasUnlockedExperimentals;
    private bool hasUnlockedExperimentals;
    public bool HasUnlockedDrones => hasUnlockedDrones;
    private bool hasUnlockedDrones;
    public bool HasUnlockedTrainingHall => hasUnlockedTrainingHall;
    private bool hasUnlockedTrainingHall;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

        PlayerProgressionData p = SaveSystem.LoadProgress();
        hasUnlockedExperimentals = p.hasUnlockedExperimentals;
        hasUnlockedDrones = p.hasUnlockedDrones;
        hasUnlockedTrainingHall = p.hasUnlockedTrainingHall;
    }

    public void UnlockExperimentals()
    {
        hasUnlockedExperimentals = true;
        SaveSystem.SaveProgress(this);
    }

    public void UnlockDrones()
    {
        hasUnlockedDrones = true;
        SaveSystem.SaveProgress(this);
    }

    public void UnlockTrainingHall()
    {
        hasUnlockedTrainingHall = true;
        SaveSystem.SaveProgress(this);
    }
}
