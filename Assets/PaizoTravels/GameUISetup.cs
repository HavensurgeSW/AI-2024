using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameUISetup : MonoBehaviour
{
    [SerializeField] private Button spawnWorkerButton;
    public static Action AlarmRing;
    public static Action AlarmCancel;

    private void OnEnable()
    {
        TownImplement.OnInit += BindToTown;
    }

    private void OnDisable()
    {
        TownImplement.OnInit -= BindToTown;
    }

    public void BindToTown(TownImplement TI) {
        spawnWorkerButton.onClick.AddListener(TI.CreateWorker);
    }

    public void SoundAlarm() { 
        AlarmRing?.Invoke();
    }

    public void CancelAlarm() { 
        AlarmCancel?.Invoke();
    }

}
