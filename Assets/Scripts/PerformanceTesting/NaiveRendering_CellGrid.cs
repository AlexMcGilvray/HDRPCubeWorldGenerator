using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaiveRendering_CellGrid : MonoBehaviour
{

    public int Dimensions = 10;
    public float Height = 1.0f;
    public int CellSize = 1;

    public GameObject CellTemplate;

    public List<GameObject> cellObjects;

    void Start()
    {
        cellObjects = new List<GameObject>(Dimensions * Dimensions);
        for (int i = 0; i < Dimensions * Dimensions; ++i)
        {
            cellObjects.Add(null);
        }
        for (int z = 0; z < Dimensions; ++z)
        {
            for (int x = 0; x < Dimensions; ++x)
            {
                float xCoord = x * CellSize - (Dimensions / 2) * CellSize;
                float zCoord = z * CellSize - (Dimensions / 2) * CellSize;
                Vector3 coord = new Vector3(xCoord, 0, zCoord);
                var cellObject = Instantiate(CellTemplate, coord, Quaternion.identity);
                cellObjects[z * Dimensions + x] = cellObject;
                var cell = cellObject.GetComponent<CellV2>();
                cell.MakeCell(Height, CellSize / 2, 1.0f);
            }
        }
    }

    void Update()
    {

    }



}
