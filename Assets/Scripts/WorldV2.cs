using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldV2 : MonoBehaviour
{
    public class MountainBuilder
    {
        public MountainBuilder(
            WorldV2 world,
            WorldBuilderDirection direction,
            int life,
            int x,
            int y,
            float height)
        {
            _world = world;
            _direction = direction;
            Life = life;
            X = x;
            Y = y;
            _height = height;
        }

        public int Life { get; set; }

        public bool Alive => Life >= 0 ? true : false;

        public void Step()
        {
            if (Alive)
            {
                _world.MakeCell(X, Y, _height);
                Life--;
                switch (_direction)
                {
                    case WorldBuilderDirection.North:
                        Y++;
                        break;
                    case WorldBuilderDirection.South:
                        Y--;
                        break;
                    case WorldBuilderDirection.East:
                        X++;
                        break;
                    case WorldBuilderDirection.West:
                        X--;
                        break;
                }
            }
        }

        private WorldV2 _world;
        private WorldBuilderDirection _direction;

        private int X, Y;
        private float _height;
    }

    public enum WorldBuilderDirection
    {
        North, South, East, West
    }

    public GameObject cellTemplate;
    public int Dimensions = 20;
    public float CellSize = 10;
    public float AnimTimeTarget = 0.5f;
    public int InitialHealth = 5;
    public float InitialHeight = 10;

    public bool VerboseDebuggingEnabled = true;

    public List<GameObject> cellObjects;
    bool AreAnyBuildersAlive() => _builders.Any(x => x.Alive);

    void Start()
    {
        cellObjects = new List<GameObject>(Dimensions * Dimensions);
        for (int i = 0; i < Dimensions * Dimensions; ++i)
        {
            cellObjects.Add(null);
        }
        InitWorldGeneration(Dimensions / 2, Dimensions / 2);
    }
    void InitWorldGeneration(int x, int z)
    {
        float height = InitialHeight;
        int health = InitialHealth;

        MakeCell(x, z, height);
        _animTimeCurrent = 0;

        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.West);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.East);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.North);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.South);

        var direction = _directionsToBuildLeft.Dequeue();

        MakeWorldBuilder(x , z, height, health, direction);
    }

    void StepWorldGeneration()
    {
        foreach (var builder in _builders)
        {
            if (builder.Alive)
            {
                builder.Step();
            }
        }
    }

    public void MakeWorldBuilder(
        int x,
        int z,
        float height,
        int health,
        WorldBuilderDirection direction = WorldBuilderDirection.North)
    {
        if (x >= 0 && x < Dimensions && z >= 0 && z < Dimensions)
        {
            var worldBuilder = new MountainBuilder(this, direction, health, x, z, height);
            _builders.Add(worldBuilder);
        }
    }

    void Update()
    {
        if (!AreAnyBuildersAlive() && _directionsToBuildLeft.Count > 0)
        {
            float height = InitialHeight;
            int health = InitialHealth;
            var dir = _directionsToBuildLeft.Dequeue();
            int x = Dimensions / 2;
            int z = Dimensions / 2;
            _builders.Clear();
            MakeWorldBuilder(x , z, height, health, dir);
        }

        if (_animTimeCurrent >= AnimTimeTarget && AreAnyBuildersAlive())
        {
            StepWorldGeneration();
            _animTimeCurrent = 0;
        }
        else
        {
            _animTimeCurrent += Time.deltaTime;
        }
    }

    void MakeCell(int x, int z, float height)
    {
        if (x < 0 || x > Dimensions || z < 0 || z > Dimensions)
        {
            Debug.LogError(
                "make cell parameter out of range. value of x is  " + x + " and z is " + z);
            return;
        }
        if (VerboseDebuggingEnabled)
        {
            Debug.Log("making cell x " + x + " and z " + z);
        }
        if (cellObjects[z * Dimensions + x] != null)
        {
            return;
        }
        float xCoord = x * CellSize - (Dimensions / 2) * CellSize;
        float zCoord = z * CellSize - (Dimensions / 2) * CellSize;
        Vector3 coord = new Vector3(xCoord, 0, zCoord);
        var cellObject = Instantiate(cellTemplate, coord, Quaternion.identity);
        cellObjects[z * Dimensions + x] = cellObject;
        var cell = cellObject.GetComponent<CellV2>();
        cell.MakeCell(height, CellSize / 2, AnimTimeTarget);
    }

    private float _animTimeCurrent;

    private Queue<WorldBuilderDirection> _directionsToBuildLeft = new Queue<WorldBuilderDirection>();

    private HashSet<MountainBuilder> _builders = new HashSet<MountainBuilder>();
}