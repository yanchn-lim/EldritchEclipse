using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelHandler : MonoBehaviour
{
    GameObject[] UpgradePanels = new GameObject[3];

    [SerializeField]
    Upgrade[] UpgradeList;
    Dictionary<Upgrade, float> UpgradeWeightList = new();

    private void Awake()
    {
        UpgradePanels[0] = GameObject.Find("Upgrade_1");
        UpgradePanels[1] = GameObject.Find("Upgrade_2");
        UpgradePanels[2] = GameObject.Find("Upgrade_3");

        foreach (var item in UpgradeList)
        {
            UpgradeWeightList.Add(item, item.RarityWeightValue);
        }
    }

    void DisplayUpgrade()
    {
        foreach (var panel in UpgradePanels)
        {
            //get a random upgrade
            var upgrade = Probability.SelectWeightedItem(UpgradeWeightList);
            panel.GetComponent<UpgradeHandler>().DisplayUpgrade(upgrade);
            
        }
    }
}
