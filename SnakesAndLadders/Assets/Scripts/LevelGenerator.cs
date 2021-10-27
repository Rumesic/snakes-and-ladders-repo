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

    [Range(0, 20)]
    [SerializeField] int columnCount = 10;
    [Range(0, 20)]
    [SerializeField] int rowCount = 10;
    [SerializeField] Sprite testSprite;
    [SerializeField] Material ladderMat;
    [SerializeField] Material snakeMat;

    [SerializeField] public Tile[] tileArray;
    [SerializeField] TMP_FontAsset fontAsset;
    [SerializeField] Color textColor;
    Vector2 screenSize;
    // Start is called before the first frame update
    void Start()
    {      

        for (int i = 0; i < tileArray.Length; i++)
        {
            float smoothTime = 1 + (i / 10);
            tileArray[i].RectT.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        for (int i = 0; i < tileArray.Length; i++)
        {
            float smoothTime = 1 + (i / 10);
            tileArray[i].RectT.DOScale(.85f, smoothTime);
        }
    }

    [ContextMenu("GenerateTiles")]
    void GenerateTiles()
    {
        ClearTiles();
        screenSize = new Vector2(Screen.width, Screen.height);
        tileArray = new Tile[rowCount * columnCount];
        int i = 0;

        for(int y = 0; y < columnCount; y++)
        {
            for(int x = 0; x < rowCount; x++)
            {
                CreateChild(x, y, i);
                i += 1;
            }
        }

        DetermineSpecialTiles();
    }

    void ClearTiles()
    {
        for(int i = 0; i < tileArray.Length; i++)
        {
            if(tileArray[i].RectT != null)
                DestroyImmediate(tileArray[i].RectT.gameObject);
        }
    }


    private void OnValidate()
    {

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
        float horizontal = (y % 2 == 0) ? horizontalPosition : (tileSize * rowCount) - horizontalPosition;

        newRect.localScale = Vector3.one;
        newRect.anchoredPosition = new Vector3(horizontal, verticalPosition, 0);
        newRect.localPosition = new Vector3(newRect.localPosition.x, newRect.localPosition.y, 0);

        tileArray[index] = new Tile();
        tileArray[index].RectT = newRect;
        tileArray[index].Index = index;
        tileArray[index].LocalCoordinates = new Vector2(x, y);

        CreateTextObject(newRect, index);
    }

    void CreateTextObject(RectTransform parent, int index)
    {
        RectTransform newText = new GameObject("textNumber_" + index).AddComponent<RectTransform>();
        newText.SetParent(parent);
        newText.anchoredPosition = Vector2.zero;
        newText.localPosition = new Vector3(newText.localPosition.x, newText.localPosition.y, 0);
        newText.sizeDelta = parent.sizeDelta;
        newText.localScale = Vector3.one;

        TextMeshProUGUI textObject = newText.gameObject.AddComponent<TextMeshProUGUI>();
        textObject.text = (index +1).ToString();
        textObject.alignment = TMPro.TextAlignmentOptions.Center;
        textObject.font = fontAsset;
        textObject.color = textColor;
        textObject.fontSize = 45;

        tileArray[index].TextMesh = textObject;
    }

    void DetermineSpecialTiles()
    {
        int occurance = tileArray.Length / 6;
        //int minLeap = tileArray.Length / 15;
        //int maxLeap = tileArray.Length - (tileArray.Length / 20);
        int currentLadders = 0;
        int currentSnakes = 0;

        //int incline = tileArray.Length;
        //int decline = tileArray.Length;
        int randomConnectedTile = 0;

        for(int i = 0; i < occurance; i++)
        {
            int getTile = FindFreeTile();
            //int tileY = (int)tileArray[getTile].LocalCoordinates.y;
            int orientation = Random.Range(0, 2);
            if (orientation == 0)
            {
                randomConnectedTile = FindHigherTile(getTile);
                    currentLadders += 1;

            }
            else if (orientation == 1)
            {
                randomConnectedTile = FindLowerTile(getTile);
                currentSnakes += 1;
            }
            //randomConnectedTile = Mathf.Clamp(randomConnectedTile, 0, tileArray.Length - 1);
            if (randomConnectedTile != 0)
                CreateLineRenderers(tileArray[getTile], tileArray[randomConnectedTile], orientation);
        }
    }

    int FindFreeTile()
    {
        int random = Random.Range(0, tileArray.Length - 1);
        for(int i = random; i < tileArray.Length -1; i++)
        {
            if (!tileArray[i].SpecialTile)
            {
                tileArray[i].SpecialTile = true;
                return i;
            }
        }
        return 0;
    }
    int FindHigherTile(int index)
    {
        int targetIndex = index;
        int tileY = (int)tileArray[index].LocalCoordinates.y;

        int randomY = Random.Range(tileY + 1, columnCount -1);
        int randomX = Random.Range(0, rowCount - 1);
        for (int i = index +1; i < tileArray.Length; i ++)
        {
            if (tileArray[i].LocalCoordinates == new Vector2(randomX, randomY))
            {
                tileArray[i].SpecialTile = true;
                tileArray[index].ConnectedIndex = i;
                return targetIndex = i;
            }
        }
        return 0;
    }
    int FindLowerTile(int index)
    {
        int targetIndex = index;
        int tileY = (int)tileArray[index].LocalCoordinates.y;

        int randomY = Random.Range(0, tileY -1);
        int randomX = Random.Range(0, rowCount - 1);

        for (int i = index -1; i > 0; i--)
        {
            if (tileArray[i].LocalCoordinates == new Vector2(randomX, randomY))
            {
                tileArray[i].SpecialTile = true;
                tileArray[index].ConnectedIndex = i;
                return targetIndex = i;
            }
        }
        return 0;
    }

    void CreateLineRenderers(Tile origin, Tile target, int orientation)
    {
        LineRenderer rend = new GameObject(origin.RectT.gameObject.ToString()).AddComponent<LineRenderer>();

        //rend.material = (orientation == 0) ? ladderMat : snakeMat;
        rend.transform.SetParent(origin.RectT);

        rend.textureMode = LineTextureMode.Tile;
        rend.alignment = LineAlignment.TransformZ;

        Vector3 zOffset = new Vector3(0, 0, 1f);
        rend.SetPosition(0, origin.RectT.transform.position + zOffset);
        rend.SetPosition(1, target.RectT.transform.position + zOffset);

        if(orientation == 0)
        {
            rend.material = ladderMat;
            rend.positionCount = 2;
            rend.startWidth = 0.4f;
            rend.endWidth = 0.2f;
            rend.SetPosition(0, origin.RectT.transform.position + zOffset);
            rend.SetPosition(1, target.RectT.transform.position + zOffset);
        }
        else
        {
            rend.material = snakeMat;
            rend.positionCount = 3;
            rend.startWidth = 0.5f;
            rend.endWidth = 0.4f;
            rend.SetPosition(0, origin.RectT.transform.position + zOffset);
            Vector3 tempPos = new Vector3(origin.RectT.transform.position.x, target.RectT.transform.position.y, origin.RectT.transform.position.z);
            rend.SetPosition(1, tempPos + zOffset);
            rend.SetPosition(2, target.RectT.transform.position + zOffset);
        }

        origin.LineR = rend;
    }
}
