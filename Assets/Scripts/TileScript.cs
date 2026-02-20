using UnityEngine;

public class Tile : MonoBehaviour
{

    public int x,y;
    public bool isHighlighted = false;
    public BoardManager boardManager;
    public Unit unitScript;

    public Unit unitOnTile = null;
    public bool isOccupied = false;

    public enum TileOccupiedBy
    {
        Unit,
        Wall,
        Nothing
    }
    public TileOccupiedBy occupiedBy;

    private CombatManager combatManager;
    void Start()
    {
        boardManager = FindFirstObjectByType<BoardManager>();
        combatManager = FindFirstObjectByType<CombatManager>();

    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        if(!isHighlighted)
        {
            boardManager.ClearTileHighlights();
            boardManager.DeselectUnit();
            
        }
        else
        {
            if(combatManager.currentState == PlayerActionState.Move)
            {
                boardManager.MoveSelectedUnit(x, y);
            }
            else if (combatManager.currentState == PlayerActionState.WaitingForPull)
            {
                if (combatManager.unitToPull != null)
                {
                    boardManager.MoveTargetUnit(combatManager.unitToPull, x, y);

                    combatManager.unitToPull.ResetCombatSelection();
                    boardManager.ClearTileHighlights();
                }
            }

        }
    }
}
