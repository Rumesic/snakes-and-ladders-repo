using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Tile 
{
    [SerializeField] private int index;
    [SerializeField] private Vector2 localCoordinates;
    [SerializeField] private RectTransform rectT;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private int connectedIndex;
    [SerializeField] private LineRenderer lineR;
    [SerializeField] private bool specialTile;

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }


    public Vector2 LocalCoordinates
    {
        get
        {
            return localCoordinates;
        }
        set
        {
            localCoordinates = value;
        }
    }

    public RectTransform RectT
    {
        get
        {
            return rectT;
        }
        set
        {
            rectT = value;
        }
    }

    public TextMeshProUGUI TextMesh
    {
        get
        {
            return textMesh;
        }
        set
        {
            textMesh = value;
        }
    }

    public int ConnectedIndex
    {
        get
        {
            return connectedIndex;
        }
        set
        {
            connectedIndex = value;
        }
    }

    public LineRenderer LineR
    {
        get
        {
            return lineR;
        }
        set
        {
            lineR = value;
        }
    }

    public bool SpecialTile
    {
        get
        {
            return specialTile;
        }
        set
        {
            specialTile = value;
        }
    }


}
