using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public static class LinqExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, System.Func<T, IEnumerable<T>> f)
        => e.SelectMany(c => f(c).Flatten(f)).Concat(e);
}
public class WorldV2 : MonoBehaviour
{
    // Creates a straight line with no branch logic for the mountain builder
    public enum MountainBuilderLineHelperState { Dormant, Active }
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
                _height -= Random.Range(_world.HeightDegredationMin, _world.HeightDegredationMax);
                if (_height <= _world.HeightMinThreshold)
                {
                    Life = 0;
                    return;
                }

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

                var creationResult = _world.MakeCell(X, Y, _height);
                switch (creationResult.Status)
                {
                    case CellCreationResultStatus.HitWorldBoundary:
                        Life = 0;
                        break;
                    case CellCreationResultStatus.OtherCellAlreadyExisted:
                        break;
                    case CellCreationResultStatus.Success:
                        creationResult.Cell.SetState(CellV2State.Animating);
                        break;
                }
            }
        }

        private WorldV2 _world;
        private WorldBuilderDirection _direction;
        private int X, Y;
        private float _height;
        private Queue<Vector2Int> _deferredCellsToCreate = new Queue<Vector2Int>();
    }

    public enum MountainBuilderState { BuildingLines, BuildingLineHelpers, Done }

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
            _healthDivider = Random.Range(_world.HealthDegredationDividerMin, _world.HealthDegredationDividerMax);
        }

        public MountainBuilderState State { get; set; } = MountainBuilderState.BuildingLines;
        public int Life { get; set; }

        public bool Alive => Life > 0 ? true : false;

        public void Step()
        {
            switch (State)
            {
                case MountainBuilderState.BuildingLines:
                    Life--;
                    _height -= Random.Range(_world.HeightDegredationMin, _world.HeightDegredationMax);
                    if (_height <= _world.HeightMinThreshold)
                    {
                        Life = 0;
                    }
                    else
                    {
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
                        // TODO roll a chance to see if we spawn a child here
                        var creationResult = _world.MakeCell(X, Y, _height);

                        switch (creationResult.Status)
                        {
                            case CellCreationResultStatus.HitWorldBoundary:
                                Life = 0;
                                break;
                            case CellCreationResultStatus.OtherCellAlreadyExisted:
                                break;
                            case CellCreationResultStatus.Success:
                                creationResult.Cell.SetState(CellV2State.Animating);
                                float childCreationChance = Mathf.Clamp(
                                    Random.Range(_world.ChanceSpawnChildMin, _world.ChanceSpawnChildMax) - _mountainBulderChildSpawnChanceDegredation,
                                    0.0f,
                                    1.0f);
                                _mountainBulderChildSpawnChanceDegredation -= 0.05f;
                                bool shouldCreateChild = Random.Range(0.0f, childCreationChance) > 0.5f ? true : false;
                                if (shouldCreateChild)
                                {
                                    bool randomChildSideCoinToss = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;
                                    if (randomChildSideCoinToss)
                                    {
                                        switch (_direction)
                                        {
                                            case WorldBuilderDirection.North:
                                                MakeMountainBuilderChild(WorldBuilderDirection.East);
                                                MakeLineHelper(WorldBuilderDirection.West);
                                                break;
                                            case WorldBuilderDirection.South:
                                                MakeMountainBuilderChild(WorldBuilderDirection.East);
                                                MakeLineHelper(WorldBuilderDirection.West);
                                                break;
                                            case WorldBuilderDirection.East:
                                                MakeMountainBuilderChild(WorldBuilderDirection.North);
                                                MakeLineHelper(WorldBuilderDirection.South);
                                                break;
                                            case WorldBuilderDirection.West:
                                                MakeMountainBuilderChild(WorldBuilderDirection.North);
                                                MakeLineHelper(WorldBuilderDirection.South);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (_direction)
                                        {
                                            case WorldBuilderDirection.North:
                                                MakeLineHelper(WorldBuilderDirection.East);
                                                MakeMountainBuilderChild(WorldBuilderDirection.West);
                                                break;
                                            case WorldBuilderDirection.South:
                                                MakeLineHelper(WorldBuilderDirection.East);
                                                MakeMountainBuilderChild(WorldBuilderDirection.West);
                                                break;
                                            case WorldBuilderDirection.East:
                                                MakeLineHelper(WorldBuilderDirection.North);
                                                MakeMountainBuilderChild(WorldBuilderDirection.South);
                                                break;
                                            case WorldBuilderDirection.West:
                                                MakeLineHelper(WorldBuilderDirection.North);
                                                MakeMountainBuilderChild(WorldBuilderDirection.South);
                                                break;
                                        }
                                    }
                                }
                                else
                                {

                                    switch (_direction)
                                    {
                                        case WorldBuilderDirection.North:
                                            MakeLineHelper(WorldBuilderDirection.East);
                                            MakeLineHelper(WorldBuilderDirection.West);
                                            break;
                                        case WorldBuilderDirection.South:
                                            MakeLineHelper(WorldBuilderDirection.East);
                                            MakeLineHelper(WorldBuilderDirection.West);
                                            break;
                                        case WorldBuilderDirection.East:
                                            MakeLineHelper(WorldBuilderDirection.North);
                                            MakeLineHelper(WorldBuilderDirection.South);
                                            break;
                                        case WorldBuilderDirection.West:
                                            MakeLineHelper(WorldBuilderDirection.North);
                                            MakeLineHelper(WorldBuilderDirection.South);
                                            break;
                                    }
                                }
                                break;
                        }
                    }

                    if (Life <= 0)
                    {
                        State = MountainBuilderState.BuildingLineHelpers;
                    }
                    break;
                case MountainBuilderState.BuildingLineHelpers:

                    foreach (var helper in _lineHelpers)
                    {
                        if (helper.Alive)
                        {
                            helper.Step();
                        }
                    }
                    bool areAllHelpersDone = _lineHelpers.All(x => !x.Alive);
                    if (areAllHelpersDone)
                    {
                        State = MountainBuilderState.Done;
                    }

                    break;
                case MountainBuilderState.Done:
                    break;
            }
            foreach (var child in _mountainBuilderChildren)
            {
                child.Step();
            }

        }

        private void MakeLineHelper(WorldBuilderDirection direction)
        {
            int life = (Life / _healthDivider) - Random.Range(_world.HealthDegredationMin, _world.HealthDegredationMax);
            MountainBuilderLineHelper helper = new MountainBuilderLineHelper(
                _world,
                direction,
                life,
                X,
                Y,
                _height
            );
            _lineHelpers.Add(helper);
        }

        private void MakeMountainBuilderChild(WorldBuilderDirection direction)
        {
            int life = (Life / _healthDivider) - Random.Range(_world.HealthDegredationMin, _world.HealthDegredationMax);
            MountainBuilder builder = new MountainBuilder(
                _world,
                direction,
                life,
                X,
                Y,
                _height);
            _mountainBuilderChildren.Add(builder);
        }

        private bool AreAllDescendantsDead()
        {
            IEnumerable<MountainBuilder> flattenedElements = _mountainBuilderChildren.Flatten(x => x._mountainBuilderChildren);
            return flattenedElements.Any(x => x.Alive);
        }

        private WorldV2 _world;
        private WorldBuilderDirection _direction;
        private int X, Y;
        private float _height;
        private int _healthDivider;
        private List<MountainBuilderLineHelper> _lineHelpers =
            new List<MountainBuilderLineHelper>();
        private List<MountainBuilder> _mountainBuilderChildren =
            new List<MountainBuilder>();
        private float _mountainBulderChildSpawnChanceDegredation = 0.05f;
    }

    public enum WorldBuilderDirection { North, South, East, West }

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

    [Header("World settings")]
    public int Dimensions = 20;
    public float CellSize = 10;

    [Header("World generation animation time settings")]
    public float CellStepAnimTimeTarget = 0.5f;
    public float AnimTimeTarget = 0.5f;

    [Header("Mountain builder settings")]
    public int InitialHealth = 5;
    public int HealthDegredationDividerMin = 1;
    public int HealthDegredationDividerMax = 3;
    public int HealthDegredationMin = 1;
    public int HealthDegredationMax = 3;
    public float InitialHeight = 10;
    public float HeightDegredationMin = 1;
    public float HeightDegredationMax = 3;
    public float HeightMinThreshold = 1.0f;

    public float ChanceSpawnChildMin = 0.005f;
    public float ChanceSpawnChildMax = 0.05f;

    [Header("Debug")]
    public bool VerboseDebuggingEnabled = true;

    public List<GameObject> cellObjects;

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

        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.North);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.South);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.East);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.West);

    }

    void StepWorldGeneration()
    {
        if (_currentBuilder != null)
        {
            _currentBuilder.Step();
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
            int modifiedHealth = health / Random.Range(HealthDegredationDividerMin, HealthDegredationDividerMax);
            _currentBuilder = new MountainBuilder(this, direction, modifiedHealth, x, z, height);
            //Debug.Log("made world builder facing " + direction.ToString());
        }
    }

    void Update()
    {
        bool currentBuilderInvalid = _currentBuilder == null || _currentBuilder.State == MountainBuilderState.Done;
        if (currentBuilderInvalid && _directionsToBuildLeft.Count > 0)
        {
            float height = InitialHeight;
            int health = InitialHealth;
            var dir = _directionsToBuildLeft.Dequeue();
            int x = Dimensions / 2;
            int z = Dimensions / 2;
            MakeWorldBuilder(x, z, height, health, dir);
            _animTimeCurrent = 0;
        }

        if (_animTimeCurrent >= CellStepAnimTimeTarget)
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
    private MountainBuilder _currentBuilder;
}