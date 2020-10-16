using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldV2 : MonoBehaviour
{
    // Creates a straight line with no branch logic for the mountain builder
    public class MountainBuilderLineHelper
    {
        public MountainBuilderLineHelper(
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

        public bool Alive => Life > 0 ? true : false;
        public void Step()
        {
            if (Alive)
            {
                Life--;
                var creationResult = _world.MakeCell(X, Y, _height);

                switch (creationResult.Status)
                {
                    case CellCreationResultStatus.HitWorldBoundary:
                        Life = 0;
                        break;
                    case CellCreationResultStatus.OtherCellAlreadyExisted:
                        break;
                    case CellCreationResultStatus.Success:
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
                        break;
                }
            }
        }

        private WorldV2 _world;
        private WorldBuilderDirection _direction;

        private int X, Y;
        private float _height;
    }
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

        public bool Alive => Life > 0 ? true : false;

        public void Step()
        {
            if (Alive)
            {
                Life--;

                var creationResult = _world.MakeCell(X, Y, _height);
                switch (creationResult.Status)
                {
                    case CellCreationResultStatus.HitWorldBoundary:
                        Life = 0;
                        break;
                    case CellCreationResultStatus.OtherCellAlreadyExisted:
                        break;
                    case CellCreationResultStatus.Success:
                        switch (_direction)
                        {
                            case WorldBuilderDirection.North:
                                MakeLineHelper(WorldBuilderDirection.East);
                                MakeLineHelper(WorldBuilderDirection.West);
                                Y++;
                                break;
                            case WorldBuilderDirection.South:
                                MakeLineHelper(WorldBuilderDirection.East);
                                MakeLineHelper(WorldBuilderDirection.West);
                                Y--;
                                break;
                            case WorldBuilderDirection.East:
                                MakeLineHelper(WorldBuilderDirection.North);
                                MakeLineHelper(WorldBuilderDirection.South);
                                X++;
                                break;
                            case WorldBuilderDirection.West:
                                MakeLineHelper(WorldBuilderDirection.North);
                                MakeLineHelper(WorldBuilderDirection.South);
                                X--;
                                break;

                        }
                        break;
                }
            }

            foreach (var helper in _lineHelpers)
            {
                if (helper.Alive)
                {
                    helper.Step();
                }
            }
        }

        private void MakeLineHelper(WorldBuilderDirection direction)
        {
            MountainBuilderLineHelper helper = new MountainBuilderLineHelper(
                _world,
                direction,
                Life / 4,
                X,
                Y,
                _height
            );
            _lineHelpers.Add(helper);
        }

        private WorldV2 _world;
        private WorldBuilderDirection _direction;
        private int X, Y;
        private float _height;

        private List<MountainBuilderLineHelper> _lineHelpers =
            new List<MountainBuilderLineHelper>();
    }

    public enum WorldBuilderDirection
    {
        North, South, East, West
    }

    public class CellCreationResult
    {
        public CellCreationResultStatus Status { get; set; }
        public CellV2 Cell { get; set; }
    }

    public enum CellCreationResultStatus
    {
        Success, HitWorldBoundary, OtherCellAlreadyExisted
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

        MakeWorldBuilder(x, z, height, health, direction);
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
            MakeWorldBuilder(x, z, height, health, dir);
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

    CellCreationResult MakeCell(int x, int z, float height)
    {
        CellCreationResult result = new CellCreationResult();
        if (VerboseDebuggingEnabled)
        {
            Debug.Log("making cell x " + x + " and z " + z);
        }
        if (x < 0 || x >= Dimensions || z < 0 || z >= Dimensions)
        {
            result.Status = CellCreationResultStatus.HitWorldBoundary;
            return result;
        }
        if (cellObjects[z * Dimensions + x] != null)
        {
            result.Status = CellCreationResultStatus.OtherCellAlreadyExisted;
            return result;
        }
        float xCoord = x * CellSize - (Dimensions / 2) * CellSize;
        float zCoord = z * CellSize - (Dimensions / 2) * CellSize;
        Vector3 coord = new Vector3(xCoord, 0, zCoord);
        var cellObject = Instantiate(cellTemplate, coord, Quaternion.identity);
        cellObjects[z * Dimensions + x] = cellObject;
        var cell = cellObject.GetComponent<CellV2>();
        cell.MakeCell(height, CellSize / 2, AnimTimeTarget);
        result.Status = CellCreationResultStatus.Success;
        result.Cell = cell;
        return result;
    }

    private float _animTimeCurrent;

    private Queue<WorldBuilderDirection> _directionsToBuildLeft = new Queue<WorldBuilderDirection>();

    private HashSet<MountainBuilder> _builders = new HashSet<MountainBuilder>();
}