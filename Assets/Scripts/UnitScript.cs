using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/* TO DO:
-Doom mechanic

-thrall abilities
-add ability data as necesary

-All deadsouls units
*/



public class Unit : MonoBehaviour
{

    public struct AbilityData
    {
        public Unit targetUnit;
        public List<Unit> multiTargets;
        public Tile targetTile;
        // more to be added
    }
    public enum TargetType { Self, Unit, Tile, MoreUnits, MoreTiles, Line, SplashSelf, SplashTarget }

    public bool isSelected = false;
    public int x, y;
    public BoardManager boardManager;
    public CombatManager combatManager;

    //Stats
    public enum ArmorType { None, Ward, Arm, Super }

    public int MoveSpeed;
    public int HitPoints;
    public int Defense;
    public ArmorType Armor;

    //Flags
    public bool isIsolated = true;
    public bool isHighlighted = false;

    //TOKENS
    public int WeakTokens = 0;
    public int DoomTokens = 0;


    //Adjacent Units
    public List<Unit> adjacentUnits = new List<Unit>();


    //INFO UI
    public UnitInfoUI_NEW infoUI;
    void Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();
        combatManager = FindFirstObjectByType<CombatManager>();
        infoUI = FindFirstObjectByType<UnitInfoUI_NEW>();
    }

    
    void Update()
    {
        
    }
    void OnMouseDown()
    {

        

        if (combatManager.currentState == PlayerActionState.WaitingForUnit /* || combatManager.currentState == PlayerActionState.WaitingForPull*/)
        {
            if (combatManager.UnitsInRange.Contains(this)){

            
                if (!combatManager.collectedUnits.Contains(this) && boardManager.selectedUnit != this)
                {
                    combatManager.collectedUnits.Add(this);
                    Debug.Log($"Țintă selectată: {this.name}. ({combatManager.collectedUnits.Count}/{combatManager.targetsRequired})");
                }

                if (combatManager.collectedUnits.Count == combatManager.targetsRequired)
                {
                    AbilityData data = new AbilityData();

                    data.targetUnit = combatManager.collectedUnits[0];

                    data.multiTargets = new List<Unit>(combatManager.collectedUnits);

                    boardManager.selectedUnit.ExecuteAbility(combatManager.selectedAbilityIndex, data);
                    Debug.Log("Se executa abilitatea, am selectat toate target-urile");


                }
            }
            else
            {
                Debug.Log("Unitatea nu se afla in range");

            }
            return;
        }

        if (!isSelected)
        {
            if(!(combatManager.currentState==PlayerActionState.WaitingForPull))
            {
                boardManager.SelectUnit(this);
                isSelected = true;
            }
            
        }
        else
        {
            boardManager.ClearTileHighlights();
            boardManager.DeselectUnit();
        }
    }

    public void ResetCombatSelection()
    {
        combatManager.collectedUnits.Clear();
        combatManager.targetsRequired = 0;
        combatManager.SetCurrentState(PlayerActionState.None);
        
        infoUI.HideInfoPanel();

    }

    public virtual void ExecuteAbility(int abilityIndex, AbilityData data) { }

    public virtual string[] GetAbilityNames() { return new string[] { "Ability 1", "Ability 2", "Move" }; }
    public virtual int GetRequiredTargets(int index) { return 0; }
    public virtual (int rangeLow, int rangeHigh) GetAbilityRange(int index) { return (0,0); }

    public virtual TargetType GetTargetType(int index) { return TargetType.Self; }




}


