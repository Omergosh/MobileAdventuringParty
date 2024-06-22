using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerVisual : MonoBehaviour
{
    [SerializeField] Transform selectedUnitIndicator;
    [SerializeField] Transform targetedUnitIndicator;

    [SerializeField] AdventurerScript thisAdventurer;

    // Start is called before the first frame update
    void Start()
    {
        BattleManager.Instance.OnSelectedAdventurerChanged += BattleManager_OnSelectedAdventurerChanged;

        if (BattleManager.Instance.GetSelectedAdventurer == thisAdventurer)
        { ShowSelectedUI(); }
        else { HideSelectedUI(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnitIndicator.gameObject.activeSelf)
        {
            selectedUnitIndicator.Rotate(Vector3.forward, 40f * Time.deltaTime);
        }
    }

    private void BattleManager_OnSelectedAdventurerChanged(object sender, BattleManager.OnSelectedAdventurerChangedEventArgs e)
    {
        if (e.adventurer == thisAdventurer)
        { ShowSelectedUI(); }
        else { HideSelectedUI(); }
    }

    void ShowSelectedUI()
    {
        selectedUnitIndicator.gameObject.SetActive(true);
    }

    void HideSelectedUI()
    {
        selectedUnitIndicator.gameObject.SetActive(false);
    }

}
