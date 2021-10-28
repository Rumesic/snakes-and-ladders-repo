using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;


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
    [SerializeField] GameObject Dice;

    [Header("UI")]
    public CanvasScaler mainCanvas;
    [SerializeField] RectTransform endPanel;
    [SerializeField] RectTransform scrollPaper;
    [SerializeField] Slider playerSlider;
    [SerializeField] TextMeshProUGUI sliderValue;
    //[SerializeField] Slider columnSlider;
    //[SerializeField] Slider rowSlider;
    [SerializeField] Sprite playerSprite;
    public LevelGenerator levelGenerator;
    public Vector2 levelRectTransform;
    [SerializeField] GameObject rollButton;
    [SerializeField] RectTransform forwardButton;
    [SerializeField] RectTransform backwardButton;

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

    public void SetSliderValue()
    {
        sliderValue.text = playerSlider.value.ToString();
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
            Player newPlayer = newT.gameObject.AddComponent<Player>();
            newPlayer.PlayerIndex = i;
            newPlayer.Position = -1;
            players.Add(newPlayer);

            canRoll = true;
        }
    }

    public void RollDice(int roll)
    {
        randomDice = roll;
        waitingForRoll = false;
        int negativeValue = players[currentPlayer].Position - randomDice;
        int positiveValue = players[currentPlayer].Position + randomDice;
        positiveValue = Mathf.Clamp(positiveValue, 0, levelGenerator.tileArray.Length - 1);
        
        if(negativeValue >= 0)
        {
            backwardButton.gameObject.SetActive(true);
            UpdateButtonPosition(backwardButton, negativeValue);
        }

        forwardButton.gameObject.SetActive(true);
        UpdateButtonPosition(forwardButton, positiveValue);
        canRoll = false;
    }
    void UpdateButtonPosition(RectTransform button, int position)
    {
        position = Mathf.Clamp(position, 0, levelGenerator.tileArray.Length - 1);
        Vector3 tilePos = new Vector2(levelGenerator.tileArray[position].RectT.anchoredPosition.x, levelGenerator.tileArray[position].RectT.anchoredPosition.y);
        Vector3 levelAnchorPos = levelRectTransform;
        button.anchoredPosition = tilePos + levelAnchorPos;
    }
    
    public void GoForward() //UI Button
    {
        int value = players[currentPlayer].Position + randomDice;
        value = Mathf.Clamp(value, 0, levelGenerator.tileArray.Length - 1);
        players[currentPlayer].SetPosition(value);
        ResetButtons();
    }
    public void GoBackwards() //UI Button
    {
        int value = players[currentPlayer].Position - randomDice;
        value = Mathf.Clamp(value, 0, levelGenerator.tileArray.Length - 1);
        players[currentPlayer].SetPosition(value);
        ResetButtons();
    }

    void ResetButtons()
    {
        if(players.Count > 0)
        {
            SetCurrentPlayer();
            backwardButton.gameObject.SetActive(false);
            forwardButton.gameObject.SetActive(false);
            canRoll = true;
        }

    }
    
    void SetCurrentPlayer()
    {
        RemovePlayers();
        if (players.Count == 0)
        {
            OnGameEnd();
            return;
        }
        else if (players.Count == 1)
            return;

        else currentPlayer = (currentPlayer <= players.Count - 2) ? currentPlayer += 1 : currentPlayer = 0;
    }

    void RemovePlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GameFinished)
            {
                players.RemoveAt(i);
                players.TrimExcess();
            }
        }
    }

    void OnGameEnd()
    {
        canRoll = false;
        Dice.SetActive(false);
        endPanel.gameObject.SetActive(true);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
