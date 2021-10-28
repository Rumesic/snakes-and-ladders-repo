using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;

public class LevelGenerator : MonoBehaviour
{
    [Range(0, 150)]
    [SerializeField] public float tileSize = 10;

    [Range(0, 20)]
    [SerializeField] public int columnCount = 10;
    [Range(0, 20)]
    [SerializeField] public int rowCount = 10;
    [SerializeField] Sprite testSprite;
    [SerializeField] Material ladderMat;
    [SerializeField] Material snakeMat;

    //[SerializeField] Color tileColor;

    [SerializeField] List<Color> tileColors;
    [SerializeField] Gradient specialGradient;
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
            //tileArray[i].RectT.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //screenSize = new Vector2(Screen.width, Screen.height);
        for (int i = 0; i < tileArray.Length; i++)
        {
            float smoothTime = 1 + (i / 10);
            //tileArray[i].RectT.DOScale(.85f, smoothTime);
        }
    }


    public void ActivateTile(int index)
    {
        if (index == -1)
            index = tileArray.Length - 1;
        tileArray[index].RectT.localScale = Vector3.zero;
        tileArray[index].RectT.DOScale(.85f, .25f);
        if(tileArray[index].SpecialTile && tileArray[index].ConnectedIndex != 0)
        {
            tileArray[tileArray[index].ConnectedIndex].RectT.localScale = Vector3.zero;
            tileArray[tileArray[index].ConnectedIndex].RectT.DOScale(.85f, .5f);
        }

    }

    [ContextMenu("GenerateTiles")]
    public void GenerateTiles()
    {
        ClearTiles();
        screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
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

        //DetermineSpecialTiles();
    }

    void ClearTiles()
    {
        for(int i = 0; i < tileArray.Length; i++)
        {
            if(tileArray[i].RectT != null)
                DestroyImmediate(tileArray[i].RectT.gameObject);
        }
    }


    void CreateChild(int x, int y, int index)
    {
        RectTransform newRect = new GameObject("Tile_"+ index + " |x" + x + ", y" + y).AddComponent<RectTransform>();
        newRect.SetParent(gameObject.transform);
        newRect.sizeDelta = new Vector2(tileSize, tileSize);
        Image img = newRect.gameObject.AddComponent<Image>();
        img.sprite = testSprite;
        img.raycastTarget = false;

        img.color = (index % 2 == 0) ? tileColors[0] : tileColors[1];
        float horizontalPosition = x * tileSize;
        float verticalPosition = y * tileSize;
        float horizontal = (y % 2 == 0) ? horizontalPosition : (tileSize * (rowCount -1)) - horizontalPosition;
        newRect.localScale = Vector3.one;

        newRect.anchoredPosition = new Vector3(horizontal - screenSize.x / 4, verticalPosition - screenSize.y / 2, 0);
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
        textObject.color = (index % 2 == 0) ? tileColors[1] : tileColors[0];
        textObject.fontSize = 45;

        tileArray[index].TextMesh = textObject;
    }

    public void DetermineSpecialTiles()
    {
        int occurance = tileArray.Length / 5;
        int randomConnectedTile = 0;

        for(int currentLadders = 0; currentLadders < occurance / 2; currentLadders ++)
        {
            int getTile = FindFreeTile();
            randomConnectedTile = FindHigherTile(getTile);
            if (randomConnectedTile != 0)
                CreateLineRenderers(tileArray[getTile], tileArray[randomConnectedTile], 0);
        }

        for (int currentSnakes = 0; currentSnakes < occurance / 2; currentSnakes++)
        {
            int getTile = FindFreeTile();
            randomConnectedTile = FindLowerTile(getTile);
            if (randomConnectedTile != 0)
                CreateLineRenderers(tileArray[getTile], tileArray[randomConnectedTile], 1);
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

        rend.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        //rend.material = (orientation == 0) ? ladderMat : snakeMat;
        rend.transform.SetParent(origin.RectT);

        rend.textureMode = LineTextureMode.Tile;
        rend.alignment = LineAlignment.TransformZ;

        Vector3 zOffset = new Vector3(0, 0, 1f);
        rend.SetPosition(0, origin.RectT.transform.position + zOffset);
        rend.SetPosition(1, target.RectT.transform.position + zOffset);

        rend.gameObject.AddComponent<SortingGroup>().sortingOrder = 20;

        rend.colorGradient = specialGradient;
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
            rend.positionCount = 2;
            rend.startWidth = 0.4f;
            rend.endWidth = 0.2f;
            rend.SetPosition(0, origin.RectT.transform.position + zOffset);
            //Vector3 tempPos = new Vector3(origin.RectT.transform.position.x, target.RectT.transform.position.y, origin.RectT.transform.position.z);
            //rend.SetPosition(1, tempPos + zOffset);
            rend.SetPosition(1, target.RectT.transform.position + zOffset);
        }

        origin.LineR = rend;
    }
}
