using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(1, 6)]
    [SerializeField] int playerCount = 1;

    [Header("Debug")]
    public int randomDice;
    [SerializeField] List<Player> players = new List<Player>();
    [SerializeField] List<Color> playerColors = new List<Color>();
    [SerializeField] int currentPlayer;

    [Header("UI")]
    [SerializeField] Sprite playerSprite;
    public static LevelGenerator levelGenerator;
    public static Vector2 levelRectTransform;
    [SerializeField] GameObject rollButton;
    [SerializeField] TextMeshProUGUI forwardText;
    [SerializeField] TextMeshProUGUI backwardText;

    bool canGoBack;

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
            Image playerImage = newT.gameObject.AddComponent<Image>();
            playerImage.sprite = playerSprite;
            playerImage.color = playerColors[i];
            newT.localPosition = Vector3.zero;
            newT.localScale = Vector3.one;
            players.Add(newT.gameObject.AddComponent<Player>());

        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void RollDice()
    {
        rollButton.SetActive(false);
        randomDice = RandomDice();
        canGoBack = (players[currentPlayer].position - randomDice >= 0) ? true : false;
        forwardText.transform.parent.gameObject.SetActive(true);

        if (canGoBack)
        {
            backwardText.text = (players[currentPlayer].position - randomDice + 1).ToString();
            backwardText.transform.parent.gameObject.SetActive(true);
        }
        else
            backwardText.transform.parent.gameObject.SetActive(false);

        forwardText.text = (players[currentPlayer].position + randomDice + 1).ToString();
        //players[currentPlayer].SetPosition(RandomDice());
        //SetCurrentPlayer();
    }


    public void GoForward()
    {
        players[currentPlayer].SetPosition(players[currentPlayer].position + randomDice);
        SetCurrentPlayer();
        rollButton.SetActive(true);
        forwardText.transform.parent.gameObject.SetActive(false);
        backwardText.transform.parent.gameObject.SetActive(false);
    }
    public void GoBackwards()
    {
        players[currentPlayer].SetPosition(players[currentPlayer].position - randomDice);
        SetCurrentPlayer();
        rollButton.SetActive(true);
        backwardText.transform.parent.gameObject.SetActive(false);
        forwardText.transform.parent.gameObject.SetActive(false);
    }

    void SetCurrentPlayer()
    {
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].gameFinished)
            {
                players.RemoveAt(i);
                players.TrimExcess();
            }    
        }
        currentPlayer = (currentPlayer < players.Count - 1) ? currentPlayer += 1 : currentPlayer = 0;
        
    }

    int RandomDice()
    {
        return Random.Range(1, 6);
    }
}
