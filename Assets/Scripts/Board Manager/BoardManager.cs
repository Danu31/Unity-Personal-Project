using JetBrains.Annotations;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class BoardManager : MonoBehaviour
{
    //Tiles, matrix and Board
    public float width = 10.0f;
    public float height = 10.0f;
    public GameObject tile;

    //Tile Script
    private Tile tileScript;

    //Unit script
    private Unit unitScript;

    //Tile Matrix
    [SerializeField] public Tile[,] tileMatrix;
    public GameObject Board;

    //Units LIST
    public List<Unit> unitList = new List<Unit>();

    //Temporary for testing
    public GameObject thrall;
    public GameObject DeadsoulsScion;
    public GameObject wall;

    //Selected unit
    public Unit selectedUnit;

    //UI
    public UnitInfoUI_NEW infoUI;

    //Combat Manager
    public CombatManager combatManager;

    //Highlight
    public GameObject highlightTileAsset;
    public GameObject highlightUnitAsset;


    void Start()
    {
        Board = new GameObject("Board");
        GenerateBoard();

        infoUI = FindFirstObjectByType<UnitInfoUI_NEW>();
        combatManager = FindFirstObjectByType<CombatManager>();




        //Generate Thrall at [3,1] - FOR TESTING
        Vector3 unit_position = tileMatrix[3, 1].transform.position;
        unit_position.z = -1f;
        Quaternion rotation = Quaternion.identity;
        GameObject thrall_1 = Instantiate(thrall, unit_position, rotation);
        thrall_1.transform.parent = Board.transform;

        unitScript = thrall_1.GetComponent<Unit>();
        unitScript.x = 3; unitScript.y = 1;
        tileMatrix[3, 1].isOccupied = true;
        tileMatrix[3, 1].occupiedBy = Tile.TileOccupiedBy.Unit;
        tileMatrix[3, 1].unitOnTile = unitScript;
        unitList.Add(unitScript);




        //Generate Thrall at [4,5] - FOR TESTING
        unit_position = tileMatrix[4, 5].transform.position;
        unit_position.z = -1f;
        GameObject thrall_2 = Instantiate(thrall, unit_position, rotation);
        thrall_2.transform.parent = Board.transform;

        unitScript = thrall_2.GetComponent<Unit>();
        unitScript.x = 4; unitScript.y = 5;
        tileMatrix[4, 5].isOccupied = true;
        tileMatrix[4, 5].occupiedBy = Tile.TileOccupiedBy.Unit;
        tileMatrix[4, 5].unitOnTile = unitScript;
        unitList.Add(unitScript);

        //Generate Scion at [6,5] - FOR TESTING
        unit_position = tileMatrix[6, 5].transform.position;
        unit_position.z = -1f;
        GameObject DeadsoulsScionObject = Instantiate(DeadsoulsScion, unit_position, rotation);
        DeadsoulsScionObject.transform.parent = Board.transform;

        unitScript = DeadsoulsScionObject.GetComponent<Unit>();
        unitScript.x = 6; unitScript.y = 5;
        tileMatrix[6, 5].isOccupied = true;
        tileMatrix[6, 5].occupiedBy = Tile.TileOccupiedBy.Unit;
        tileMatrix[6, 5].unitOnTile = unitScript;
        unitList.Add(unitScript);

        //Generate Thrall at [1,1] - FOR TESTING
        unit_position = tileMatrix[1, 1].transform.position;
        unit_position.z = -1f;
        GameObject thrall_3 = Instantiate(thrall, unit_position, rotation);
        thrall_3.transform.parent = Board.transform;

        unitScript = thrall_3.GetComponent<Unit>();
        unitScript.x = 1; unitScript.y = 1;
        tileMatrix[1, 1].isOccupied = true;
        tileMatrix[1, 1].occupiedBy = Tile.TileOccupiedBy.Unit;
        tileMatrix[1, 1].unitOnTile = unitScript;
        unitList.Add(unitScript);


        //Generate wall at [3,2] _ FOR TESTING
        Vector3 wall_position = tileMatrix[3, 2].transform.position;
        wall_position.z = -1f;
        GameObject wall_1 = Instantiate(wall, wall_position, rotation);
        wall_1.transform.parent = Board.transform;
        tileMatrix[3,2].isOccupied = true;
        tileMatrix[3, 2].occupiedBy = Tile.TileOccupiedBy.Wall;




    }

    void Update()
    {
        
    }
    //Generate Board
    public void GenerateBoard()
    {
        tileMatrix = new Tile[(int)width, (int)height];

        for (int x = 0; x < width; ++x)
        {

            for (int y = 0; y < height; ++y)
            {

                Vector2 position = new Vector2(x * 0.7f, y * 0.7f);
                Quaternion rotation = Quaternion.identity;
                GameObject obj = Instantiate(tile, position, rotation);



                obj.transform.parent = Board.transform;

                tileScript = obj.GetComponent<Tile>();
                tileScript.x = x; tileScript.y = y; tileMatrix[x, y] = tileScript;

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if ((x + y) % 2 == 1)
                {
                    sr.color = new Color32(169, 169, 169, 255);
                }
            }
        }

        float boardX = (height / 2.0f) * 0.7f;
        float boardY = (width / 2.0f) * 0.7f;

        Board.transform.position = new Vector2(-boardX + 0.35f, -boardY + 0.35f);


    }

    //Highlight possible moves
    List<Tile> highlightedTiles = new List<Tile>();
    public void HighlightTiles(int x, int y, int MoveSpeed)
    {
        ClearTileHighlights();

        Queue<Tile> queue = new Queue<Tile>();

        Dictionary<Tile, int> costToReach = new Dictionary<Tile, int>();

        Tile startTile = tileMatrix[x, y];
        queue.Enqueue(startTile);
        costToReach[startTile] = 0;

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            if (current.x + 1 < width)
            {
                Tile right = tileMatrix[current.x + 1, current.y];
                if (!highlightedTiles.Contains(right) && costToReach[current] + 1 <= MoveSpeed && !right.isOccupied)
                {
                    queue.Enqueue(right);
                    costToReach[right] = costToReach[current] + 1;
                    highlightedTiles.Add(right);
                }
            }


            if (current.x - 1 >= 0)
            {
                Tile left = tileMatrix[current.x - 1, current.y];
                if (!highlightedTiles.Contains(left) && costToReach[current] + 1 <= MoveSpeed && !left.isOccupied)
                {
                    queue.Enqueue(left);
                    costToReach[left] = costToReach[current] + 1;
                    highlightedTiles.Add(left);
                }
            }

            if (current.y + 1 < height)
            {
                Tile up = tileMatrix[current.x, current.y + 1];
                if (!highlightedTiles.Contains(up) && costToReach[current] + 1 <= MoveSpeed && !up.isOccupied)
                {
                    queue.Enqueue(up);
                    costToReach[up] = costToReach[current] + 1;
                    highlightedTiles.Add(up);

                }
            }

            if (current.y - 1 >= 0)
            {
                Tile down = tileMatrix[current.x, current.y - 1];
                if (!highlightedTiles.Contains(down) && costToReach[current] + 1 <= MoveSpeed && !down.isOccupied)
                {
                    queue.Enqueue(down);
                    costToReach[down] = costToReach[current] + 1;
                    highlightedTiles.Add(down);

                }

            }








        }


        highlightedTiles.Remove(startTile);
        foreach (var i in highlightedTiles)
        {
            i.isHighlighted = true;
            Vector3 highlightPosition = i.transform.position;
            highlightPosition.z = -1f;
            Quaternion rotation = Quaternion.identity;
            GameObject obj = Instantiate(highlightTileAsset, highlightPosition, rotation);
            obj.transform.parent = Board.transform;
            obj.tag = "TileHighlight";
        }

    }


    //Delete Highlights
    public void ClearTileHighlights()
    {
        GameObject[] highlights = GameObject.FindGameObjectsWithTag("TileHighlight");

        foreach (GameObject h in highlights)
        {
            
            Destroy(h);
        }

        //for (int i = highlightedTiles.Count - 1; i >= 0; i--)
        //{
        //    highlightedTiles[i].isHighlighted = false;
        //    highlightedTiles.RemoveAt(i);
        //}

        foreach (Tile t in highlightedTiles)
        {
            t.isHighlighted = false;
        }

        highlightedTiles.Clear();
    }

    //deselect unit
    public void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            GameObject[] highlights = GameObject.FindGameObjectsWithTag("SelectedUnitHighlight");
            foreach (GameObject h in highlights)
            {
                Destroy(h);
            }

            infoUI.HideInfoPanel();
            selectedUnit.isSelected = false;
            selectedUnit = null;



            if (combatManager.currentState != PlayerActionState.None)
            {
                combatManager.SetCurrentState(PlayerActionState.None);
            }
        }
    }

    //select unit
    public void SelectUnit(Unit unit)
    {
        DeselectUnit();
        ClearTileHighlights();
        selectedUnit = unit;
        infoUI.Show(unit);

        Vector3 highlightPosition = unit.transform.position;
        highlightPosition.z = -0.9f;
        Quaternion rotation = Quaternion.identity;
        GameObject obj = Instantiate(highlightUnitAsset, highlightPosition, rotation);
        //obj.transform.parent = Board.transform;
        obj.tag = "SelectedUnitHighlight";
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.color = new Color32(255, 0, 0, 255);

    }

    //Move Unit
    public void MoveSelectedUnit(int x, int y)
    {
        if(selectedUnit != null)
        {

            if(selectedUnit.adjacentUnits.Count > 0)
            {
                foreach (var i in selectedUnit.adjacentUnits)
                {
                    i.adjacentUnits.Remove(selectedUnit);
                    if(i.adjacentUnits.Count == 0)
                    {
                        i.isIsolated = true;
                    }
                }
            }

            selectedUnit.adjacentUnits.Clear();

            tileMatrix[selectedUnit.x, selectedUnit.y].isOccupied = false;
            tileMatrix[selectedUnit.x, selectedUnit.y].occupiedBy = Tile.TileOccupiedBy.Nothing;
            tileMatrix[selectedUnit.x, selectedUnit.y].unitOnTile = null;

            Vector3 position = tileMatrix[x, y].transform.position;
            position.z = -1f;
            selectedUnit.transform.position = position;
            selectedUnit.x = x;
            selectedUnit.y = y;
            tileMatrix[x, y].isOccupied = true;
            tileMatrix[x, y].occupiedBy = Tile.TileOccupiedBy.Unit;
            tileMatrix[x, y].unitOnTile = selectedUnit;

            List<Unit> adjacentUnits = ExistsAdjacentUnit(selectedUnit);

            if (adjacentUnits.Count > 0)
            {
                selectedUnit.isIsolated = false;
                foreach (var i in adjacentUnits)
                {
                    selectedUnit.adjacentUnits.Add(i);
                    i.adjacentUnits.Add(selectedUnit);
                    i.isIsolated = false;
                }
            }
            else
            {
                selectedUnit.isIsolated = true;
            }

            DeselectUnit();
            ClearTileHighlights();
        }
    }

    public void MoveTargetUnit(Unit target, int x, int y)
    {
        if (target == null) return;

        if (target.adjacentUnits.Count > 0)
        {
            foreach (var i in target.adjacentUnits)
            {
                i.adjacentUnits.Remove(target);
                if (i.adjacentUnits.Count == 0)
                {
                    i.isIsolated = true;
                }
            }
        }



        target.adjacentUnits.Clear();


        tileMatrix[target.x, target.y].isOccupied = false;
        tileMatrix[target.x, target.y].occupiedBy = Tile.TileOccupiedBy.Nothing;
        tileMatrix[target.x, target.y].unitOnTile = null;



        Vector3 position = tileMatrix[x, y].transform.position;
        position.z = -1f;
        target.transform.position = position;

        target.x = x;
        target.y = y;

        tileMatrix[x, y].isOccupied = true;
        tileMatrix[x, y].occupiedBy = Tile.TileOccupiedBy.Unit;
        tileMatrix[x, y].unitOnTile = target;

        List<Unit> adjacentUnits = ExistsAdjacentUnit(target);
        if (adjacentUnits.Count > 0)
        {
            target.isIsolated = false;
            foreach(var i in adjacentUnits)
            {
                target.adjacentUnits.Add(i);
                i.adjacentUnits.Add(target);
                i.isIsolated = false;
            }
        }
        else
        {
            target.isIsolated = true;
        }

        DeselectUnit();
        ClearTileHighlights();
    }

    public List<Unit> ExistsAdjacentUnit(Unit target)
    {
        List<Unit> adjacentUnits = new List<Unit>();
        if (target)
        {
            if (target.x + 1 < width && tileMatrix[target.x + 1, target.y].isOccupied && tileMatrix[target.x + 1, target.y].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x + 1, target.y].unitOnTile);

            if (target.x - 1 >= 0 && tileMatrix[target.x - 1, target.y].isOccupied && tileMatrix[target.x - 1, target.y].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x - 1, target.y].unitOnTile);

            if (target.y + 1 < height && tileMatrix[target.x, target.y + 1].isOccupied && tileMatrix[target.x, target.y + 1].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x, target.y + 1].unitOnTile);

            if (target.y - 1 >= 0 && tileMatrix[target.x, target.y - 1].isOccupied && tileMatrix[target.x, target.y - 1].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x, target.y - 1].unitOnTile);


            if (target.x + 1 < width && target.y + 1 < height && tileMatrix[target.x + 1, target.y + 1].isOccupied && tileMatrix[target.x + 1, target.y + 1].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x + 1, target.y + 1].unitOnTile);

            if (target.x + 1 < width && target.y - 1 >= 0 && tileMatrix[target.x + 1, target.y - 1].isOccupied && tileMatrix[target.x + 1, target.y - 1].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x + 1, target.y - 1].unitOnTile);

            if (target.x - 1 >= 0 && target.y - 1 >= 0 && tileMatrix[target.x - 1, target.y - 1].isOccupied && tileMatrix[target.x - 1, target.y - 1].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x - 1, target.y - 1].unitOnTile);

            if (target.x - 1 >= 0 && target.y + 1 < height && tileMatrix[target.x - 1, target.y + 1].isOccupied && tileMatrix[target.x - 1, target.y + 1].occupiedBy == Tile.TileOccupiedBy.Unit)
                adjacentUnits.Add(tileMatrix[target.x - 1, target.y + 1].unitOnTile);
        }
        return adjacentUnits;
    }

}