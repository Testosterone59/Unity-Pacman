using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

	public static Mesh Generate(List<CellData> cells, float sizeCell, float height)
    {
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;

        List<Vector3> vertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        List<int> floorTriangles = new List<int>();
        List<Vector2> UVs = new List<Vector2>();

        foreach (var data in cells)
        {
            if (data.type != TypeCell.Wall) { continue; }

            CreateQuad(Matrix4x4.TRS(
                    new Vector3(data.coord.x * sizeCell, height * 1.5f, data.coord.z * sizeCell),
                    Quaternion.LookRotation(Vector3.up),
                    new Vector3(sizeCell, sizeCell, 1)
                ), ref vertices, ref UVs, ref floorTriangles);

            GridCoord cForward = data.coord.GetNewCoordFromDirection(Direction.Forward);
            GridCoord cBack = data.coord.GetNewCoordFromDirection(Direction.Back);
            GridCoord cRight = data.coord.GetNewCoordFromDirection(Direction.Right);
            GridCoord cLeft = data.coord.GetNewCoordFromDirection(Direction.Left);

            if (!CheckBounds(cells, cForward) || cells.Find((c) => c.coord == cForward).type != TypeCell.Wall)
            {
                CreateQuad(Matrix4x4.TRS(
                    new Vector3(data.coord.x * sizeCell, height, (data.coord.z + .5f) * sizeCell),
                    Quaternion.LookRotation(Vector3.forward),
                    new Vector3(sizeCell, height, 1)
                ), ref vertices, ref UVs, ref wallTriangles);
            }
            if (!CheckBounds(cells, cLeft) || cells.Find((c) => c.coord == cLeft).type != TypeCell.Wall)
            {
                CreateQuad(Matrix4x4.TRS(
                    new Vector3((data.coord.x - .5f) * sizeCell, height, data.coord.z * sizeCell),
                    Quaternion.LookRotation(Vector3.left),
                    new Vector3(sizeCell, height, 1)
                ), ref vertices, ref UVs, ref wallTriangles);
            }
            if (!CheckBounds(cells, cRight) || cells.Find((c) => c.coord == cRight).type != TypeCell.Wall)
            {
                CreateQuad(Matrix4x4.TRS(
                    new Vector3((data.coord.x + .5f) * sizeCell, height, data.coord.z * sizeCell),
                    Quaternion.LookRotation(Vector3.right),
                    new Vector3(sizeCell, height, 1)
                ), ref vertices, ref UVs, ref wallTriangles);
            }

            if (!CheckBounds(cells, cBack) || cells.Find((c) => c.coord == cBack).type != TypeCell.Wall)
            {
                CreateQuad(Matrix4x4.TRS(
                    new Vector3(data.coord.x * sizeCell, height, (data.coord.z - .5f) * sizeCell),
                    Quaternion.LookRotation(Vector3.back),
                    new Vector3(sizeCell, height, 1)
                ), ref vertices, ref UVs, ref wallTriangles);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.SetTriangles(wallTriangles.ToArray(), 0);
        mesh.SetTriangles(floorTriangles.ToArray(), 1);
        mesh.RecalculateNormals();

        return mesh;
    }

    static bool CheckBounds(List<CellData> cells, GridCoord coord)
    {
        return cells.Exists((x) => x.coord == coord);
    }

    static void CreateQuad(Matrix4x4 matrix, ref List<Vector3> vertices,
    ref List<Vector2> UVs, ref List<int> triangles)
    {
        int index = vertices.Count;

        Vector3 v1 = new Vector3(-.5f, -.5f, 0);
        Vector3 v2 = new Vector3(-.5f, .5f, 0);
        Vector3 v3 = new Vector3(.5f, .5f, 0);
        Vector3 v4 = new Vector3(.5f, -.5f, 0);

        vertices.Add(matrix.MultiplyPoint3x4(v1));
        vertices.Add(matrix.MultiplyPoint3x4(v2));
        vertices.Add(matrix.MultiplyPoint3x4(v3));
        vertices.Add(matrix.MultiplyPoint3x4(v4));

        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(0, 0));

        triangles.Add(index + 2);
        triangles.Add(index + 1);
        triangles.Add(index);

        triangles.Add(index + 3);
        triangles.Add(index + 2);
        triangles.Add(index);
    }

}
