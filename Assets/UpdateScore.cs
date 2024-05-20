using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UpdateScore : MonoBehaviour
{
    private void OnEnable()
    {
        NewScore(2);
    }

    public string team;
    public TextMeshProUGUI scoreText;
    public void NewScore(int newValue)
    {
        scoreText.text = $"{team}: {newValue}";
    }
}
