using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{

    [SerializeField] Image fullscreenOverlay;
    [SerializeField] TMP_Text countdownText;

    [SerializeField] Transform moveTargetIndicatorPrefab;

    private bool countdownStarted = false;
    private bool countdownFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        fullscreenOverlay.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        BattleManager.Instance.OnCountdownStart += BattleManager_OnCountdownStart;
        BattleManager.Instance.OnCommandAdventurerMove += BattleManager_OnCommandAdventurerMove;
        //BattleManager.Instance.OnSelectedAdventurerChanged += BattleManager_OnSelectedAdventurerChanged;
    }

    private void BattleManager_OnCommandAdventurerMove(object sender, BattleManager.OnCommandAdventurerMoveEventArgs e)
    {
        Transform moveTargetIndicatorGO = Instantiate(moveTargetIndicatorPrefab, e.targetPosition, Quaternion.identity);
        //Transform moveTargetIndicatorScript = moveTargetIndicatorGO.GetComponent<>
    }

    //private void BattleManager_OnSelectedAdventurerChanged(object sender, BattleManager.OnSelectedAdventurerChangedEventArgs e)
    //{
    //    throw new System.NotImplementedException();
    //}

    private void BattleManager_OnCountdownStart(object sender, System.EventArgs e)
    {
        countdownStarted = true;
        countdownText.text = "3";
    }

    // Update is called once per frame
    void Update()
    {
        if (countdownStarted && !countdownFinished)
        {
            countdownText.text = Mathf.CeilToInt(BattleManager.Instance.countdownTimer).ToString();
            if (BattleManager.Instance.countdownTimer <= 0f)
            {
                countdownFinished = false;
                fullscreenOverlay.gameObject.SetActive(false);
                countdownText.gameObject.SetActive(false);
            }
        }
    }

}
