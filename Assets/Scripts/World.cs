using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    public enum WorldBuilderDirection
    {
        North, South, East, West
    }

    public class WorldBuilder
    {
        public WorldBuilder(
            World world,
            int x,
            int y,
            float height,
            int health,
            WorldBuilderDirection direction = WorldBuilderDirection.North)
        {
            _world = world;
            Direction = direction;
            X = x;
            Z = y;
            Height = height;
            Health = health;
        }
        public WorldBuilderDirection Direction { get; set; }
        public int X { get; set; }
        public int Z { get; set; }
        public int Health { get; set; }

        public float ChanceOfSpawningNewBuilder { get; set; } = 0.03f;
        public float Height { get; set; }

        public float HeightDegredationMin { get; set; } = 1;

        public float HeightDegredationMax { get; set; } = 10;

        public bool Alive => Health > 0;

        public bool IsInWorldBounds =>
            X >= 0 && X < _world.Dimensions && Z >= 0 && Z < _world.Dimensions;

        public void UpdateMovement()
        {
            Health--;

            switch (Direction)
            {
                case WorldBuilderDirection.West:
                    X--;
                    break;
                case WorldBuilderDirection.East:
                    X++;
                    break;
                case WorldBuilderDirection.South:
                    Z--;
                    break;
                case WorldBuilderDirection.North:
                    Z++;
                    break;
                default:
                    break;
            }

            if (!IsInWorldBounds) // for now we'll just kill the builder if it's out of bounds.
            {
                Health = 0;
                return;
            }

            Height -= Random.Range(HeightDegredationMin, HeightDegredationMax);

            UpdateNewBuilderSpawning();
        }

        private static List<WorldBuilderDirection> GetPerpendicularDirections(
            WorldBuilderDirection direction)
        {
            List<WorldBuilderDirection> results = new List<WorldBuilderDirection>(2);

            switch (direction)
            {
                case WorldBuilderDirection.West:
                case WorldBuilderDirection.East:
                    results.Add(WorldBuilderDirection.North);
                    results.Add(WorldBuilderDirection.South);
                    break;
                case WorldBuilderDirection.South:
                case WorldBuilderDirection.North:
                    results.Add(WorldBuilderDirection.East);
                    results.Add(WorldBuilderDirection.West);
                    break;
                default:
                    break;
            }

            return results;
        }

        private void UpdateNewBuilderSpawning()
        {
            var directions = GetPerpendicularDirections(Direction);
            foreach (var dir in directions)
            {
                if (Random.value < ChanceOfSpawningNewBuilder)
                {
                    float newHeight = Height - Random.Range(HeightDegredationMin, HeightDegredationMax);
                    switch (dir)
                    {
                        case WorldBuilderDirection.West:
                            _world.MakeWorldBuilder(X - 1, Z, newHeight, Health - 1, dir);
                            break;
                        case WorldBuilderDirection.East:
                            _world.MakeWorldBuilder(X + 1, Z, newHeight, Health - 1, dir);

                            break;
                        case WorldBuilderDirection.South:
                            _world.MakeWorldBuilder(X, Z - 1, newHeight, Health - 1, dir);
                            break;
                        case WorldBuilderDirection.North:
                            _world.MakeWorldBuilder(X, Z + 1, newHeight, Health - 1, dir);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private World _world;
    }

    public GameObject cellTemplate;
    public int Dimensions = 20;
    public float CellSize = 10;
    // how long each step animates
    public float AnimTimeTarget = 1;

    public int WorldGenerationInitialHealth = 5;
    public float WorldGenerationInitialHeight = 50;
    public List<GameObject> cellObjects;

    public float HeightDegredationMin = 1;

    public float HeightDegredationMax = 10;

    void Start()
    {
        cellObjects = new List<GameObject>(Dimensions * Dimensions);
        for (int i = 0; i < Dimensions * Dimensions; ++i)
        {
            cellObjects.Add(null);
        }
        InitWorldGeneration(Dimensions / 2, Dimensions / 2);
    }

    //GameObject GetCell(int x, int z) => cellObjects[z * Dimensions + x];

    bool AreAnyBuildersAlive() => _builders.Any(x => x.Alive);

    void InitWorldGeneration(int x, int z)
    {
        float height = WorldGenerationInitialHeight;
        int health = WorldGenerationInitialHealth;

        MakeCell(x, z, height);
        _animTimeCurrent = 0;

        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.West);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.East);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.North);
        _directionsToBuildLeft.Enqueue(WorldBuilderDirection.South);

        var direction = _directionsToBuildLeft.Dequeue();

        MakeWorldBuilder(x - 1, z, height, health, direction);
    }

    void MakeCell(int x, int z, float height)
    {
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

    public void MakeWorldBuilder(
        int x,
        int z,
        float height,
        int health,
        WorldBuilderDirection direction = WorldBuilderDirection.North)
    {
        if (x >= 0 && x < Dimensions && z >= 0 && z < Dimensions)
        {
            var worldBuilder = new WorldBuilder(this, x, z, height, health, direction);
            worldBuilder.HeightDegredationMax = HeightDegredationMax;
            worldBuilder.HeightDegredationMin = HeightDegredationMin;
            _builders.Add(worldBuilder);
        }
    }

    void StepWorldGeneration()
    {
        // we are going to add to the list while modifying so we need to make a temp copy
        List<WorldBuilder> tempBuilders = new List<WorldBuilder>(_builders);
        foreach (var builder in tempBuilders)
        {
            if (!builder.Alive)
            {
                continue;
            }

            MakeCell(builder.X, builder.Z, builder.Height);
            builder.UpdateMovement();
        }
    }

    void Update()
    {

        if (!AreAnyBuildersAlive() && _directionsToBuildLeft.Count > 0)
        {
            float height = WorldGenerationInitialHeight;
            int health = WorldGenerationInitialHealth;
            var dir = _directionsToBuildLeft.Dequeue();
            int x = Dimensions / 2;
            int z = Dimensions / 2;

            _builders.Clear();

            switch (dir)
            {
                case WorldBuilderDirection.East:
                    MakeWorldBuilder(x + 1, z, height, health, WorldBuilderDirection.East);
                    break;

                case WorldBuilderDirection.South:
                    MakeWorldBuilder(x, z - 1, height, health, WorldBuilderDirection.South);
                    break;

                case WorldBuilderDirection.North:
                    MakeWorldBuilder(x, z + 1, height, health, WorldBuilderDirection.North);
                    break;
            }

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

    private float _animTimeCurrent;

    private Queue<WorldBuilderDirection> _directionsToBuildLeft = new Queue<WorldBuilderDirection>();

    private HashSet<WorldBuilder> _builders = new HashSet<WorldBuilder>();
}