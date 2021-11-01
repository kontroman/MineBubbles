using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridRowController : MonoBehaviour
{
    private GridLayoutGroup _gridGroup;
    private int _cellCount;
    private bool _showImages;

    public GameObject gridCellPrefab;

    public void  Setup(int cellCount)
    {
        _cellCount = cellCount;

        _gridGroup.cellSize = new Vector2(
            GameConfig.BALL_RADIUS * 2,
            GameConfig.BALL_RADIUS * 2
        );
        CreateCells();
    }

    void Awake()
    {
        this._gridGroup = GetComponent<GridLayoutGroup>();
    }

    void CreateCells()
    {
        for (int i = 0; i < _cellCount; i++)
        {
            GameObject cell = GameObject.Instantiate(gridCellPrefab, this.gameObject.transform);
            cell.GetComponent<GridCellController>().Setup();

            cell.GetComponent<Image>().enabled = _showImages;
        }
    }
}
