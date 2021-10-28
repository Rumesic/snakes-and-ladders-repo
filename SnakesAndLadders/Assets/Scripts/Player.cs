using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] int playerIndex;

    [Range(-1, 99)]
    [SerializeField] int position;
    
    
    [SerializeField] private bool gameFinished;

    [SerializeField] private Color playerColor;




    RectTransform rectT;
    public Color PlayerColor
    {
        get
        {
            return playerColor;
        }
        set
        {
            playerColor = value;
        }
    }
    public bool GameFinished
    {
        get
        {
            return gameFinished;
        }
        set
        {
            gameFinished = value;
        }
    }

    public int PlayerIndex
    {
        get
        {
            return playerIndex;
        }
        set
        {
            playerIndex = value;
        }
    }
    public int Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
        }
    }

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
        Vector3 levelAnchorPos = GameManager.Instance.levelRectTransform;
        if (Position >= GameManager.Instance.levelGenerator.tileArray.Length - 1)
            Position = -1;

        else if (Position >= 0)
        {
            Vector3 tilePos = new Vector2(GameManager.Instance.levelGenerator.tileArray[position].RectT.anchoredPosition.x, GameManager.Instance.levelGenerator.tileArray[position].RectT.anchoredPosition.y);
            rectT.DOAnchorPos(tilePos + levelAnchorPos, 1);
        }

        else if (Position == -1)
        {
            float tileSize = GameManager.Instance.levelGenerator.tileSize;
            float verticalPos = tileSize * playerIndex;
            Vector3 tilePos = new Vector2(GameManager.Instance.levelGenerator.tileArray[0].RectT.anchoredPosition.x - tileSize, GameManager.Instance.levelGenerator.tileArray[0].RectT.anchoredPosition.y + verticalPos);
            rectT.DOAnchorPos(tilePos + levelAnchorPos, 1);
        }

    }

    public void SetPosition(int index)
    {
        position = (index < GameManager.Instance.levelGenerator.tileArray.Length - 1) ? index : -1;

        if (position == -1)
        {
            GameFinished = true;
            return;
        }
        GameManager.Instance.levelGenerator.ActivateTile(position);
        Tile currentTile = GameManager.Instance.levelGenerator.tileArray[position];

        if (currentTile.SpecialTile && currentTile.ConnectedIndex != 0)
        {
            position = currentTile.ConnectedIndex;
        }

    }
}
