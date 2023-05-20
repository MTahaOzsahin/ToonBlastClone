using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Slider playerCount;
    public TextMeshProUGUI playerCountText;

    public Button startButton;
    public Button exitButton;
    public Button namesButton;
    public Button namesCloseButton;

    public Button truthButton;
    public Button dareButton;
    

    public GameObject suggestTruth;
    public GameObject suggestDare;

    public GameObject namesPanel;
    public GameObject diffPanel;

    public List<GameObject> playersNameInputField;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        truthButton.onClick.AddListener(OnTruthButtonClick);
        dareButton.onClick.AddListener(OnDareButtonClick);
        namesButton.onClick.AddListener(OnNamesButtonClick);
        namesCloseButton.onClick.AddListener(OnNamesCloseButtonClick);
    }

    private void OnNamesCloseButtonClick()
    {
        namesPanel.SetActive(false);
        foreach (var playerName in playersNameInputField)
        {
            playerName.gameObject.SetActive(false);
        }
    }

    private void OnNamesButtonClick()
    {
        namesPanel.SetActive(true);
        var playerCountSliderValue = playerCount.value;
        playerCountSliderValue = Mathf.RoundToInt(playerCountSliderValue);
        for (int i = 0; i < playerCountSliderValue; i++)
        {
            playersNameInputField[i].gameObject.SetActive(true);
        }
    }

    private void OnTruthButtonClick()
    {
        suggestTruth.gameObject.SetActive(true);
        
    }

    private void OnDareButtonClick()
    {
        suggestDare.gameObject.SetActive(true);
    }

    private void OnStartButtonClick()
    {
        truthButton.gameObject.SetActive(true);
        dareButton.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        diffPanel.gameObject.SetActive(false);
        playerCount.gameObject.SetActive(false);
        namesButton.gameObject.SetActive(false);
    }


    private void Update()
    {
        var playerCountSliderValue = playerCount.value;
        playerCountSliderValue = Mathf.RoundToInt(playerCountSliderValue);


        playerCountText.text = playerCountSliderValue.ToString(CultureInfo.InvariantCulture);
    }
}
