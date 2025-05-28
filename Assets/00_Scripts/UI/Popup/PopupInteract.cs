using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupInteract : PopupUI
{
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetText(string key, string description)
    {
        keyText.text = key;
        descriptionText.text = description;
    }
}
