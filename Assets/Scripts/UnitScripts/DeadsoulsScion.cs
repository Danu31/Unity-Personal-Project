using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class DeadsoulsScion : Unit
{
    public override string[] GetAbilityNames() { return new string[] { "Move", "Tombraiser", "Kidnap", "Serpent's Kiss" }; }

    public override TargetType GetTargetType(int index)
    {
        string name = GetAbilityNames()[index];
        if (name == "Tombraiser") return TargetType.Tile;
        if (name == "Kidnap") return TargetType.Unit;
        if (name == "Serpent's Kiss") return TargetType.Unit;

        return TargetType.Self;
    }

    //Targets
    public override int GetRequiredTargets(int index)
    {
        string name = GetAbilityNames()[index];
        if (name == "Tombraiser") return 1;
        if (name == "Kidnap") return 1;
        if (name == "Serpent's Kiss") return 1;

        return 0;
    }

    //Range
    public override (int rangeLow, int rangeHigh) GetAbilityRange(int index)
    {
        string name = GetAbilityNames()[index];
        if (name == "Tombraiser") return (0, 1);
        if (name == "Kidnap") return (1, 4);
        if (name == "Serpent's Kiss") return (0, 1);
        return (0, 0);
    }
    public override void ExecuteAbility(int abilityIndex, AbilityData data)
    {
        if (abilityIndex == 1)
        { // Tombraiser 11111111
            Tombraiser(data.targetTile);
        }
        else if (abilityIndex == 2)
        { // Kidnap 22222222
            Kidnap(data.targetUnit);
        }
        else if (abilityIndex == 3)
        { // Serpent's kiss 33333
            SerpentsKiss(data.targetUnit);
        }
    }

    void Tombraiser(Tile target)
    {
       //

    }
    void Kidnap(Unit target)
    {

        //
    }

    void SerpentsKiss(Unit target)
    {

        int damage = 2;
        int roll = Random.Range(1, 7);

        infoUI.DisplayRoll(roll);

        if (roll >= target.Defense)
        {
            if ((target.Armor == ArmorType.Arm || target.Armor == ArmorType.Super))
            {
                damage--;
            }
            if(damage > 0 && this.WeakTokens >= 1)
            {
                WeakTokens--;
                damage--;
            }
            target.HitPoints -= damage;

            target.adjacentUnits.Remove(this);
            if (target.isIsolated)
            {
                target.WeakTokens++;
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
            target.adjacentUnits.Add(this);

        }
        else
        {
            Debug.Log("Missed the target's defense");

        }


        ResetCombatSelection();
    }

}