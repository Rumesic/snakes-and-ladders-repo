using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Range(1, 6)]
    [SerializeField] int playerCount = 1;

    [SerializeField] List<Player> players = new List<Player>();
    [SerializeField] int currentPlayer;

    public static LevelGenerator levelGenerator;
    public static Vector2 levelRectTransform;

    private void Awake()
    {
        if(levelGenerator == null)
        {
            levelGenerator = FindObjectOfType<LevelGenerator>();
            levelRectTransform = levelGenerator.GetComponent<RectTransform>().anchoredPosition;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < playerCount; i++)
        {
            RectTransform newT = new GameObject("Player_" + (i + 1)).AddComponent<RectTransform>();
            newT.SetParent(gameObject.transform);
            newT.gameObject.AddComponent<Image>();
            newT.localPosition = Vector3.zero;
            newT.localScale = Vector3.one;
            players.Add(newT.gameObject.AddComponent<Player>());

        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void MovePlayer()
    {
        players[currentPlayer].SetPosition(RandomDice());
        SetCurrentPlayer();
    }

    void SetCurrentPlayer()
    {
        currentPlayer = (currentPlayer < playerCount -1) ? currentPlayer += 1 : currentPlayer = 0;
    }

    int RandomDice()
    {
        return Random.Range(1, 6);
    }
}
