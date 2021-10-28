using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [Header("Settings")]
    [Range(1, 6)]
    [SerializeField] int playerCount = 1;

    [Header("Debug")]
    public int randomDice;
    [SerializeField] List<Player> players = new List<Player>();
    [SerializeField] List<Color> playerColors = new List<Color>();
    [SerializeField] int currentPlayer;

    [Header("UI")]
    [SerializeField] RectTransform scrollPaper;
    [SerializeField] Slider playerSlider;
    //[SerializeField] Slider columnSlider;
    //[SerializeField] Slider rowSlider;
    [SerializeField] Sprite playerSprite;
    public static LevelGenerator levelGenerator;
    public static Vector2 levelRectTransform;
    [SerializeField] GameObject rollButton;
    [SerializeField] RectTransform forwardButton;
    [SerializeField] RectTransform backwardButton;
    //[SerializeField] TextMeshProUGUI forwardText;
    //[SerializeField] TextMeshProUGUI backwardText;

    bool canGoBack;
    public bool canRoll;
    public bool waitingForRoll;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    void CheckLevelGenerator()
    {
        if (levelGenerator == null)
        {
            levelGenerator = FindObjectOfType<LevelGenerator>();
            levelRectTransform = levelGenerator.GetComponent<RectTransform>().anchoredPosition;
        }
    }


    // Update is called once per frame
    void Update()
    {
        FetchUIValues();
    }

    void FetchUIValues()
    {
        playerCount = (int)playerSlider.value;
        //levelGenerator.columnCount = (int)columnSlider.value;
        //levelGenerator.rowCount = (int)rowSlider.value;
    }

    public void ScrollDown()
    {
        Vector2 scrollPos = scrollPaper.anchoredPosition;
        Vector2 verticalOffset = new Vector2(0, 300);
        scrollPaper.DOAnchorPos(scrollPos - verticalOffset, 0.5f);
    }

    public void CreatePlayers()
    {
        CheckLevelGenerator();
        for (int i = 0; i < playerCount; i++)
        {
            RectTransform newT = new GameObject("Player_" + (i + 1)).AddComponent<RectTransform>();
            newT.SetParent(gameObject.transform);
            Image playerImage = newT.gameObject.AddComponent<Image>();
            playerImage.raycastTarget = false;
            playerImage.sprite = playerSprite;
            playerImage.color = playerColors[i];
            newT.localPosition = Vector3.zero;
            newT.localScale = Vector3.one;
            players.Add(newT.gameObject.AddComponent<Player>());

            canRoll = true;
        }
    }
    /*
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
    }*/

    public void RollDice(int roll)
    {
        randomDice = roll;
        Debug.Log(roll);
        waitingForRoll = false;
        canGoBack = (players[currentPlayer].position - randomDice >= 0) ? true : false;
        if(canGoBack)
        {
            backwardButton.gameObject.SetActive(true);
            UpdateButtonPosition(backwardButton, players[currentPlayer].position - randomDice);
        }
        forwardButton.gameObject.SetActive(true);
        UpdateButtonPosition(forwardButton, players[currentPlayer].position + randomDice);
        canRoll = false;
    }
    void UpdateButtonPosition(RectTransform button, int position)
    {
        Vector3 tilePos = new Vector2(GameManager.levelGenerator.tileArray[position].RectT.anchoredPosition.x, GameManager.levelGenerator.tileArray[position].RectT.anchoredPosition.y);
        Vector3 levelAnchorPos = GameManager.levelRectTransform;

        button.anchoredPosition = tilePos + levelAnchorPos;
        //rectT.DOAnchorPos(tilePos + levelAnchorPos, 1);
    }
    
    public void GoForward()
    {
        players[currentPlayer].SetPosition(players[currentPlayer].position + randomDice);
        SetCurrentPlayer();
        backwardButton.gameObject.SetActive(false);
        forwardButton.gameObject.SetActive(false);
        canRoll = true;
        //rollButton.SetActive(true);
        //forwardText.transform.parent.gameObject.SetActive(false);
        //backwardText.transform.parent.gameObject.SetActive(false);
    }
    public void GoBackwards()
    {
        players[currentPlayer].SetPosition(players[currentPlayer].position - randomDice);
        SetCurrentPlayer();
        backwardButton.gameObject.SetActive(false);
        forwardButton.gameObject.SetActive(false);
        canRoll = true;
        //rollButton.SetActive(true);
        //backwardText.transform.parent.gameObject.SetActive(false);
        //forwardText.transform.parent.gameObject.SetActive(false);
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
