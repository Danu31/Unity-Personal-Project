using Unity.VisualScripting;
using UnityEngine;

public class DeadsoulsThrall : Unit
{
    public override string[] GetAbilityNames() { return new string[] {"Move", "Beckon", "Shudder" }; }

    public override TargetType GetTargetType(int index)
    {
        string name = GetAbilityNames()[index];
        if (name == "Shudder") return TargetType.Unit;
        if (name == "Beckon") return TargetType.Unit; 
        return TargetType.Self;
    }

    //Targets
    public override int GetRequiredTargets(int index)
    {
        string name = GetAbilityNames()[index];
        if (name == "Beckon") return 1;
        if (name == "Shudder") return 1;
        return 0;
    }

    //Range
    public override (int rangeLow, int rangeHigh) GetAbilityRange(int index)
    {
        string name = GetAbilityNames()[index];
        if (name == "Beckon") return (1,4);
        if (name == "Shudder") return (0,1);
        return (0,0);
    }
    public override void ExecuteAbility(int abilityIndex, AbilityData data)
    {
        if (abilityIndex == 1)
        { // Beckon 11111111
            Beckon(data.targetUnit);
        }
        else if (abilityIndex == 2)
        { // Shudder 22222222
            Shudder(data.targetUnit);
        }
    }

    void Beckon(Unit target)
    {
        combatManager.pull(this, target, 1);

        //cleanup
        combatManager.collectedUnits.Clear();
        combatManager.targetsRequired = 0;

        if (boardManager.selectedUnit != null)
        {
            GameObject[] highlights = GameObject.FindGameObjectsWithTag("SelectedUnitHighlight");
            foreach (GameObject h in highlights)
            {
                Destroy(h);
            }

            infoUI.HideInfoPanel();
            boardManager.selectedUnit.isSelected = false;
            boardManager.selectedUnit = null;


        }
        combatManager.clearUnitHighlights();

        infoUI.HideInfoPanel();

    }
    void Shudder(Unit target)
    {

        int damage = 1;
        int roll = Random.Range(1, 7);

        infoUI.DisplayRoll(roll);

        if(roll >= target.Defense)
        {
            if ((target.Armor == ArmorType.Arm || target.Armor == ArmorType.Super))
            {
                damage--;
            }
            if (damage > 0 && this.WeakTokens >= 1)
            {
                WeakTokens--;
                damage--;
            }
            target.HitPoints -= damage;

            int EffectRoll = Random.Range(1, 7);
            if (EffectRoll >= 4)
            {
                Debug.Log("rolled a: " + EffectRoll.ToString() + " - Doomed the target");
                target.DoomTokens++;
            }
            else
            {
                Debug.Log("rolled a: " + EffectRoll.ToString() + " - Didnt doom the target");
            }
        }
        else
        {
            Debug.Log("Missed the target's defense");

        }


        ResetCombatSelection();
    }


}