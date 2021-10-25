using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] string playerName;

    [Range(0,99)]
    [SerializeField] public int position;


    RectTransform rectT;

    private void Start()
    {
        rectT = GetComponent<RectTransform>();
    }
    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 tilePos = new Vector2(GameManager.levelGenerator.tileArray[position].RectT.anchoredPosition.x, GameManager.levelGenerator.tileArray[position].RectT.anchoredPosition.y);
        Vector3 levelAnchorPos = GameManager.levelRectTransform;
        rectT.DOAnchorPos(tilePos + levelAnchorPos, 1);
    }

    public void SetPosition(int index)
    {
        position = (position + index < GameManager.levelGenerator.tileArray.Length - 1) ? position += index : position = GameManager.levelGenerator.tileArray.Length -1;
        Tile currentTile = GameManager.levelGenerator.tileArray[position];
        if (currentTile.SpecialTile && currentTile.ConnectedIndex != 0)
        {
            Debug.Log(currentTile.ConnectedIndex);
            position = currentTile.ConnectedIndex;
            //SetPosition(currentTile.ConnectedIndex);
        }

    }
}
