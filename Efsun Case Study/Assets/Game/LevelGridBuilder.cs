using UnityEngine;

public class LevelGridBuilder : MonoBehaviour
{
    private LevelData _levelData;

    public void SetLevelData(LevelData levelData) => _levelData = levelData; 
    
    [ContextMenu("Build Grid")]
    public void BuildGrid()
    {
        ClearGrid();

        int size = _levelData.Size;
        float halfSize = (size - 1) / 2f;
        float cellSize = _levelData.CellSize + (1 + _levelData.Spacing);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var blockData = _levelData.Map[y, x];
                if (blockData != null)
                {
                    float posX = (x - halfSize) * cellSize;
                    float posZ = (halfSize - y) * cellSize;
                    Vector3 spawnPos = new Vector3(posX, 0f, posZ);

                    var building = Instantiate(_levelData.BuildingPrefab, spawnPos, Quaternion.identity, transform);
                    building.SetBuilding(blockData);
                }
            }
        }
    }

    private void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}