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
        WorkerManager.OnInit += BindToTown;
    }

    private void OnDisable()
    {
        WorkerManager.OnInit -= BindToTown;
    }

    public void BindToTown(WorkerManager WM) {
        spawnWorkerButton.onClick.AddListener(WM.CreateWorker);
    }

    public void SoundAlarm() { 
        AlarmRing?.Invoke();
    }

    public void CancelAlarm() { 
        AlarmCancel?.Invoke();
    }

}
