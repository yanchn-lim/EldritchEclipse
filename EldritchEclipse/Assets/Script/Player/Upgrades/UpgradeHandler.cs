using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UpgradeHandler : MonoBehaviour
{
    public TMP_Text NameText;
    public TMP_Text DescriptionText;
    public Image Icon;

    public void DisplayUpgrade(Upgrade u)
    {
        NameText.text = u.Name;
        DescriptionText.text = u.Description;
        Icon.sprite = u.Icon;
    }
}
