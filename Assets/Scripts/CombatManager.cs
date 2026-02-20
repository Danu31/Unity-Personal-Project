using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum PlayerActionState
{
    None,
    Move,
    WaitingForUnit,
    WaitingForTile,
    WaitingForPull
}
public class CombatManager : MonoBehaviour
{
    public PlayerActionState currentState = PlayerActionState.None;

    public GameObject highlightUnitAsset;



    //Referinte SCRIPT-uri
    public BoardManager BoardManager;
    public UnitInfoUI_NEW infoUI;


    //Actions MANAGER
    public int selectedAbilityIndex;
    public int targetsRequired = 0;
    public (int rangeLow, int rangeHigh) AbilityRange;
    public List<Unit> collectedUnits = new List<Unit>();
    public List<Tile> collectedTiles = new List<Tile>();
    void Start()
    {
        //REFERINTE SCRIPT-uri
        BoardManager = FindFirstObjectByType<BoardManager>();
        infoUI = FindFirstObjectByType<UnitInfoUI_NEW>();

    }

    void Update()
    {
        
    }

    public List<Unit> UnitsInRange = new List<Unit>();
    public void getUnitsInRange(int x, int y, int rangeLow, int rangeHigh)
    {
        clearUnitHighlights();

        foreach (Unit unit in BoardManager.unitList)
        {

            if (unit.x == x && unit.y == y)
                continue;

            bool inOuterSquare =
            unit.x >= x - rangeHigh && unit.x <= x + rangeHigh &&
            unit.y >= y - rangeHigh && unit.y <= y + rangeHigh;

            bool inInnerSquare =
                unit.x >= x - rangeLow && unit.x <= x + rangeLow &&
                unit.y >= y - rangeLow && unit.y <= y + rangeLow;

            if (inOuterSquare && !inInnerSquare)
            {
                UnitsInRange.Add(unit);
                Debug.Log(unit.name + " is in square range at (" + unit.x + ", " + unit.y + ")");
            }
        }
    }

    //public void dealDamage(Unit unit, int dmg)
    //{
    //    unit.HitPoints -= dmg;
    //}

    public void clearUnitHighlights()
    {
        GameObject[] highlights = GameObject.FindGameObjectsWithTag("UnitHighlight");

        foreach (GameObject h in highlights)
        {

            Destroy(h);
        }

        foreach (Unit t in UnitsInRange)
        {
            t.isHighlighted = false;
        }

        UnitsInRange.Clear();
    }

    public void SetCurrentState(PlayerActionState newState)
    {
        currentState = newState;
        if(newState == PlayerActionState.None)
        {
            BoardManager.DeselectUnit();
            clearUnitHighlights();
        }

        if(newState == PlayerActionState.WaitingForUnit)
        {     
            BoardManager.ClearTileHighlights();
             getUnitsInRange(BoardManager.selectedUnit.x, BoardManager.selectedUnit.y, AbilityRange.rangeLow, AbilityRange.rangeHigh);
             foreach (Unit unit in UnitsInRange)
             {
                    unit.isHighlighted = true;
                    Vector3 highlightPosition = unit.transform.position;
                    highlightPosition.z = -0.9f;
                    Quaternion rotation = Quaternion.identity;
                    GameObject obj = Instantiate(highlightUnitAsset, highlightPosition, rotation);
                    //obj.transform.parent = Board.transform;
                    obj.tag = "UnitHighlight";
             }
        }
        if(newState == PlayerActionState.Move)
        {
            clearUnitHighlights();
        }
       
        Debug.Log("State changed to: " + currentState);
    }


    //Highlight possible moves
    List<Tile> highlightedTiles = new List<Tile>();
    public Unit unitToPull;
    public void pull(Unit source, Unit target, int distance)
    {
        unitToPull = target;
        BoardManager.ClearTileHighlights();
        highlightedTiles.Clear();

        int dirX = 0;
        if (source.x > target.x) dirX = 1;
        else if (source.x < target.x) dirX = -1;

        int dirY = 0;
        if (source.y > target.y) dirY = 1;
        else if (source.y < target.y) dirY = -1;

        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Tile, int> costToReach = new Dictionary<Tile, int>();

        Tile startTile = BoardManager.tileMatrix[target.x, target.y];
        queue.Enqueue(startTile);
        costToReach[startTile] = 0;

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            if (costToReach[current] >= distance) continue;

            if (dirX == 0 || dirY == 0)
            {
                if (dirX == 0 && dirY != 0)
                {
                    if (dirY == 1 && current.y + 1 < BoardManager.height)
                    {
                        Tile up = BoardManager.tileMatrix[current.x, current.y + 1];
                        if (!up.isOccupied && !costToReach.ContainsKey(up))
                        {
                            queue.Enqueue(up);
                            costToReach[up] = costToReach[current] + 1;
                            if (costToReach[up] <= distance) highlightedTiles.Add(up);
                        }
                    }
                    if (dirY == -1 && current.y - 1 >= 0)
                    {
                        Tile down = BoardManager.tileMatrix[current.x, current.y - 1];
                        if (!down.isOccupied && !costToReach.ContainsKey(down))
                        {
                            queue.Enqueue(down);
                            costToReach[down] = costToReach[current] + 1;
                            if (costToReach[down] <= distance) highlightedTiles.Add(down);
                        }
                    }
                }
                else if (dirY == 0 && dirX != 0)
                {
                    if (dirX == 1 && current.x + 1 < BoardManager.width)
                    {
                        Tile right = BoardManager.tileMatrix[current.x + 1, current.y];
                        if (!right.isOccupied && !costToReach.ContainsKey(right))
                        {
                            queue.Enqueue(right);
                            costToReach[right] = costToReach[current] + 1;
                            if (costToReach[right] <= distance) highlightedTiles.Add(right);
                        }
                    }
                    if (dirX == -1 && current.x - 1 >= 0)
                    {
                        Tile left = BoardManager.tileMatrix[current.x - 1, current.y];
                        if (!left.isOccupied && !costToReach.ContainsKey(left))
                        {
                            queue.Enqueue(left);
                            costToReach[left] = costToReach[current] + 1;
                            if (costToReach[left] <= distance) highlightedTiles.Add(left);
                        }
                    }
                }
            }
            else
            {
                if (dirX == 1 && current.x + 1 < BoardManager.width)
                {
                    Tile right = BoardManager.tileMatrix[current.x + 1, current.y];
                    if (!right.isOccupied && !costToReach.ContainsKey(right))
                    {
                        queue.Enqueue(right);
                        costToReach[right] = costToReach[current] + 1;
                        if (costToReach[right] <= distance) highlightedTiles.Add(right);
                    }
                }
                if (dirX == -1 && current.x - 1 >= 0)
                {
                    Tile left = BoardManager.tileMatrix[current.x - 1, current.y];
                    if (!left.isOccupied && !costToReach.ContainsKey(left))
                    {
                        queue.Enqueue(left);
                        costToReach[left] = costToReach[current] + 1;
                        if (costToReach[left] <= distance) highlightedTiles.Add(left);
                    }
                }
                if (dirY == 1 && current.y + 1 < BoardManager.height)
                {
                    Tile up = BoardManager.tileMatrix[current.x, current.y + 1];
                    if (!up.isOccupied && !costToReach.ContainsKey(up))
                    {
                        queue.Enqueue(up);
                        costToReach[up] = costToReach[current] + 1;
                        if (costToReach[up] <= distance) highlightedTiles.Add(up);
                    }
                }
                if (dirY == -1 && current.y - 1 >= 0)
                {
                    Tile down = BoardManager.tileMatrix[current.x, current.y - 1];
                    if (!down.isOccupied && !costToReach.ContainsKey(down))
                    {
                        queue.Enqueue(down);
                        costToReach[down] = costToReach[current] + 1;
                        if (costToReach[down] <= distance) highlightedTiles.Add(down);
                    }
                }
            }
        }

        int maxDistance = costToReach.Values.Max();
        Debug.Log("Cea mai mare distanță atinsă a fost: " + maxDistance);

        for (int j = highlightedTiles.Count - 1; j >= 0; j--)
        {
            Tile t = highlightedTiles[j];

            if (dirX == 1 && dirY == 1)
            {
                if (t.y > source.y)
                {
                    highlightedTiles.RemoveAt(j);
                }
                else if (t.x > source.x)
                {
                    highlightedTiles.RemoveAt(j);
                }
            }
            else if(dirX == 1 && dirY == -1)
            {
                if (t.y < source.y)
                {
                    highlightedTiles.RemoveAt(j);
                }
                else if (t.x > source.x)
                {
                    highlightedTiles.RemoveAt(j);
                }
            }
            else if( dirX == -1 && dirY == 1)
            {
                if (t.y > source.y)
                {
                    highlightedTiles.RemoveAt(j);
                }
                else if (t.x < source.x)
                {
                    highlightedTiles.RemoveAt(j);
                }
            }
            else if( dirY == -1 && dirX == -1)
            {
                if (t.y < source.y)
                {
                    highlightedTiles.RemoveAt(j);
                }
                else if (t.x < source.x)
                {
                    highlightedTiles.RemoveAt(j);
                }
            }

            if (!(costToReach[t] == maxDistance))
            {
                highlightedTiles.RemoveAt(j);
            }
        }

        bool existPossiblePull = false;
        foreach (var i in highlightedTiles)
        {
            if (costToReach[i] == maxDistance)
            {
                SetCurrentState(PlayerActionState.WaitingForPull);
                existPossiblePull = true;
                i.isHighlighted = true;
                Vector3 pos = i.transform.position;
                pos.z = -1f;
                GameObject obj = Instantiate(BoardManager.highlightTileAsset, pos, Quaternion.identity);
                obj.tag = "TileHighlight";
            }
        }
        if (existPossiblePull == false)
        {
            SetCurrentState(PlayerActionState.None);

        }
    }
}


