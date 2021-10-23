using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelGenerator : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] float tileSize = 10;

    [Range(0, 10)]
    [SerializeField] int rowCount = 10;
    [Range(0, 10)]
    [SerializeField] int columnCount = 10;
    [SerializeField] Sprite testSprite;
    
    [SerializeField] public List<RectTransform> tiles = new List<RectTransform>();
    Vector2 screenSize;
    // Start is called before the first frame update
    void Start()
    {
        foreach(RectTransform t in tiles)
        {
            t.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        for (int i = 0; i < tiles.Count; i++)
        {
            float smoothTime = 1 + (i / 10);
            tiles[i].DOScale(1, smoothTime);
        }
    }

    [ContextMenu("GenerateTiles")]
    void GenerateTiles()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        int i = 0;
        if (tiles.Count > 0)
            ClearTiles();

        for(int y = 0; y < rowCount; y++)
        {
            for(int x = 0; x < columnCount; x++)
            {
                i += 1;
                CreateChild(x, y, i);
            }
        }
    }

    void ClearTiles()
    {
        tiles.TrimExcess();
        foreach(RectTransform t in tiles)
        {
            if(t != null)
                DestroyImmediate(t.gameObject);
        }
        tiles.Clear();
    }


    private void OnValidate()
    {
        //GenerateTiles();
    }


    void CreateChild(int x, int y, int index)
    {
        RectTransform newRect = new GameObject("Tile_"+ index + " |x" + x + ", y" + y).AddComponent<RectTransform>();
        newRect.SetParent(gameObject.transform);
        newRect.sizeDelta = new Vector2(tileSize, tileSize);
        Image img = newRect.gameObject.AddComponent<Image>();
        img.sprite = testSprite;

        float horizontalPosition = x * tileSize;
        float verticalPosition = y * tileSize;
        float horizontal = (y % 2 == 0) ? horizontalPosition : (tileSize * columnCount) - horizontalPosition;

        newRect.anchoredPosition = new Vector3(horizontal, verticalPosition, 0);
        newRect.localScale = Vector3.one;

        tiles.Add(newRect);
        CreateTextObject(newRect, index);
    }

    void CreateTextObject(RectTransform parent, int index)
    {
        RectTransform newText = new GameObject("textNumber_" + index).AddComponent<RectTransform>();
        newText.SetParent(parent);
        newText.anchoredPosition = Vector2.zero;

        TextMeshProUGUI textObject = newText.gameObject.AddComponent<TextMeshProUGUI>();
        textObject.text = (index).ToString();
        textObject.alignment = TMPro.TextAlignmentOptions.Center;
    }
}
