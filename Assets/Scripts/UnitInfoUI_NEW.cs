using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unit;

public class UnitInfoUI_NEW : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject buttonPrefab;
    public Transform buttonContainer; //InfoPanel



    //Stats text
    public GameObject statsTextPrefab;
    private GameObject instantiatedStats;

    private List<GameObject> activeButtons = new List<GameObject>();


    private CombatManager combatManager;
    private BoardManager boardManager;

    public Text ability1Text;
    public Text ability2Text;
    private void Start()
    {

        combatManager = FindFirstObjectByType<CombatManager>();
        boardManager = FindFirstObjectByType<BoardManager>();

        HideInfoPanel();

        //moveButton.onClick.AddListener(OnMoveButtonClicked);
    }

    public void Show(Unit unit)
    {
        infoPanel.SetActive(true);
        ClearUI();

        string[] names = unit.GetAbilityNames();
        for (int i = 0; i < names.Length; i++)
        {
            int index = i;
            GameObject newBtn = Instantiate(buttonPrefab, buttonContainer);
            activeButtons.Add(newBtn);

            Text txt = newBtn.GetComponentInChildren<Text>();
            if (txt != null) txt.text = names[i];

            Button b = newBtn.GetComponent<Button>();
            b.onClick.AddListener(() => OnAbilityClicked(unit, index));
        }

        if (statsTextPrefab != null)
        {
            instantiatedStats = Instantiate(statsTextPrefab, buttonContainer);

            Text t = instantiatedStats.GetComponent<Text>();
            if (t != null)
            {
                t.text = "HP: " + unit.HitPoints + "\n" +
                         "Move speed: " + unit.MoveSpeed + "\n" +
                         "Defense: " + unit.Defense + "\n" +
                         "Armor: " + unit.Armor.ToString();
            }
        }

    }

    

    private void ClearUI()
    {
        foreach (GameObject btn in activeButtons) Destroy(btn);
        activeButtons.Clear();

        if (instantiatedStats != null) Destroy(instantiatedStats);
    }
    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);

    }
    

    void OnAbilityClicked(Unit unit, int abilityIndex)
    {
        string abilityName = unit.GetAbilityNames()[abilityIndex];

        if (abilityName == "Move")
        {
            if (combatManager.currentState != PlayerActionState.Move)
            {
                combatManager.SetCurrentState(PlayerActionState.Move);
                boardManager.HighlightTiles(boardManager.selectedUnit.x, boardManager.selectedUnit.y, boardManager.selectedUnit.MoveSpeed);
            }
        }


        combatManager.selectedAbilityIndex = abilityIndex;
        combatManager.targetsRequired = unit.GetRequiredTargets(abilityIndex);
        combatManager.AbilityRange = unit.GetAbilityRange(abilityIndex);

        TargetType type = unit.GetTargetType(abilityIndex);
        if (type == TargetType.Unit)
        {
            combatManager.SetCurrentState(PlayerActionState.WaitingForUnit);

        }

    }

    //DISPLAY ABILITY ROLL

    public Transform rollInfoContainer;
    public GameObject rollTextPrefab;
    public void DisplayRoll(int roll)
    {
        if (rollInfoContainer == null || rollTextPrefab == null) return;

        foreach (Transform child in rollInfoContainer)
        {
            Destroy(child.gameObject);
        }

        GameObject rollObj = Instantiate(rollTextPrefab, rollInfoContainer);
        Text t = rollObj.GetComponent<Text>();
        if (t != null)
        {
            t.text = "Rolled a: " + roll.ToString();
        }
    }
}


