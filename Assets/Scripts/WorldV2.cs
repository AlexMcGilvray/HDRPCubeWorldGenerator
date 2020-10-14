using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldV2 : MonoBehaviour
{
    public class MountainBuilder
    {
        public MountainBuilder(WorldV2 world, WorldBuilderDirection direction)
        {
            _world = world;
            _direction = direction;
        }

        public void Step()
        {

        }

        private WorldV2 _world;
        private WorldBuilderDirection _direction;
    }


    public enum WorldBuilderDirection
    {
        North, South, East, West
    }

    public GameObject cellTemplate;
    public int Dimensions = 20;
    public float CellSize = 10;
    public float AnimTimeTarget = 0.5f;

    public List<GameObject> cellObjects;

    void Start()
    {

    }

    void Update()
    {

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
}