using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MenuUISetup : MonoBehaviour
{

    [SerializeField] TMP_Text wTextbox;
    [SerializeField] TMP_Text hTextbox;
    [SerializeField] TMP_Text mineTextbox;
    [SerializeField] TMP_Text sepText;
    [SerializeField] Slider wSlider;
    [SerializeField] Slider hSlider;
    [SerializeField] Slider mineSlider;
    [SerializeField] Slider sepSlider;

    private void Update()
    {
        wTextbox.text = "Map Width: "+wSlider.value.ToString();
        hTextbox.text = "Map Height: " + hSlider.value.ToString();
        mineTextbox.text = "Amount of gold mines: " + mineSlider.value.ToString(); 
        sepText.text = "Grid separation: " + sepSlider.value.ToString();

    }


}
