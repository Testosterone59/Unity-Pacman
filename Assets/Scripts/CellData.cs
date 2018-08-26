using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData
{
    public Vector3 position;
    public GridCoord coord;
    public TypeCell type;
    public CellData(GridCoord coord, TypeCell type) { this.coord = coord; this.type = type; this.position = new Vector3(coord.x * Generator.sizeCell, 1, coord.z * Generator.sizeCell); }
}