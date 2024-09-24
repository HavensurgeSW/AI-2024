using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUISetup : MonoBehaviour
{
    [SerializeField] private Button spawnWorkerButton;


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

}
