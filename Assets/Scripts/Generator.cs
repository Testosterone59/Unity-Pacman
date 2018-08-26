using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

    public static List<CellData> cells;

    public const float sizeCell = 1;
    public const float height = 1;
    public const float emptyCellChance = .01f;

    public static List<GameObject> instances = new List<GameObject>();

    public static void GenerateLevel(int sizeLevel)
    {
        cells = new List<CellData>();
        CreateGrid(sizeLevel);
        ConstructLevel();
    }
    /// <summary>
    /// Генерация сетки
    /// </summary>
    /// <param name="size">Колличество ячеек в длину и ширину</param>
    static void CreateGrid(int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if (x == 0 || z == 0 || x == size - 1 || z == size - 1) /// Если граница, создаем стены
                {
                    AddCell(new GridCoord(x, z), TypeCell.Wall);
                }
                else if (x % 2 == 0 && z % 2 == 0 &&
                    Random.value > emptyCellChance) /// Иначе, генерируем лабиринт
                {
                    AddCell(new GridCoord(x, z), TypeCell.Wall);
                    int offsetX = Random.value < .3 ? 0 : (Random.value < .5 ? -1 : 1);
                    int offsetZ = offsetX != 0 ? 0 : (Random.value < .3 ? -1 : 1);
                    AddCell(new GridCoord(x + offsetX, z + offsetZ), TypeCell.Wall);
                }
                else
                {
                    AddCell(new GridCoord(x, z), TypeCell.Free);
                }
            }
        }
    }

    static void AddCell(GridCoord coord, TypeCell type)
    {
        if (!cells.Exists((c) => c.coord == coord))
        {
            cells.Add(new CellData(coord, type));
        }
    }

    static void ConstructLevel()
    {
        ConstructWalls();
        SetSpawnPoints();
        CreatePointsAndSpawns();
    }
    /// <summary>
    /// Создание объекта стен и применение к нему компонентов
    /// </summary>
    static void ConstructWalls()
    {
        Mesh meshWall = MeshGenerator.Generate(cells, sizeCell, height);
        GameObject walls = new GameObject();
        walls.transform.position = Vector3.zero;
        walls.name = "Walls";
        walls.tag = "Wall";

        MeshFilter mf = walls.AddComponent<MeshFilter>();
        mf.mesh = meshWall;

        MeshCollider mc = walls.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = walls.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] { Resources.Load("Materials/MatWall") as Material, Resources.Load("Materials/MatFloorWall") as Material };

        instances.Add(walls);
    }
    /// <summary>
    /// Спаун основных объектов
    /// </summary>
    static void CreatePointsAndSpawns()
    {
        foreach (var data in cells)
        {
            switch (data.type)
            {
                case TypeCell.Free:
                    /// Рандом загружаемого префаба, в большинстве обычный поин и с маленьким шансом энерджайзер
                    Object loadingObj = Random.value > .03f ? Resources.Load("Prefabs/Point") :
                        Resources.Load("Prefabs/Energizer");

                    instances.Add(
                        Instantiate(loadingObj, new Vector3(data.coord.x * sizeCell, height, data.coord.z * sizeCell), Quaternion.identity) as GameObject);
                    break;
                case TypeCell.PlayerSpawn:
                    GameObject playerObj = Instantiate(Resources.Load("Prefabs/Player"), new Vector3(data.coord.x * sizeCell, height, data.coord.z * sizeCell), Quaternion.identity) as GameObject;
                    GameLogic.player = playerObj.GetComponent<Player>();
                    instances.Add(playerObj);
                    break;
                case TypeCell.EnemySpawn:
                    GameObject enemySpawnObj = Instantiate(Resources.Load("Prefabs/EnemySpawn"), new Vector3(data.coord.x * sizeCell, .55f, data.coord.z * sizeCell), Quaternion.identity) as GameObject;
                    GameLogic.enemySpawnObj = enemySpawnObj;
                    instances.Add(enemySpawnObj);
                    break;
            }
        }
    }
    /// <summary>
    /// Вычисление стартовых позиций для игрока и для призраков
    /// </summary>
    static void SetSpawnPoints()
    {
        CellData playerSpawnCell = FindFreePlaceForSpecial(Direction.Forward);
        playerSpawnCell.type = TypeCell.PlayerSpawn;
        CellData EnemySpawnCell = FindFreePlaceForSpecial(Direction.Back);
        EnemySpawnCell.type = TypeCell.EnemySpawn;
    }
    /// <summary>
    /// Нахождение свободной ячейки для особых спаунов
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    static CellData FindFreePlaceForSpecial(Direction dir)
    {
        while (true)
        {
            int x = 0;
            int z = 0;
            switch (dir)
            {
                case Direction.Forward:
                    x = Random.Range(1, GameLogic.levelSize - 2);
                    z = Random.Range(1, 3);
                    break;
                case Direction.Back:
                    x = Random.Range(1, GameLogic.levelSize - 2);
                    z = Random.Range(GameLogic.levelSize - 4, GameLogic.levelSize - 2);
                    break;
            }
            GridCoord coord = new GridCoord(x, z);
            CellData findCell = null;
            if (cells.Exists((c) => c.coord == coord, out findCell) && findCell.type == TypeCell.Free)
            {
                return findCell;
            }
        }
    }

}
