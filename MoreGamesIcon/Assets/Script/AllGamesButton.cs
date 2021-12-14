using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[System.Serializable]
public class GamesDatas
{
    public string gameName;
    public string gameLink;
    public List<Texture> gameSprites;
    public GamesDatas(string name, string gameLink)
    {
        this.gameName = name;
        this.gameLink = gameLink;
        this.gameSprites = new List<Texture>();
    }
}
public class AllGamesButton : MonoBehaviour
{
    private string driveStartLink = "https://drive.google.com/uc?export=download&id=";
    [Header("Script Atamalarý")]
    [SerializeField] private string gameName = "Brain Master";
    [SerializeField] private TextMeshProUGUI gameNameText;
    [SerializeField] private Button gameButton;
    [SerializeField] private RawImage gameImage;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool canChangeSprite = false;
    private int gameOrder = 0;
    private int spriteOrder = 0;
    private float waitGameTime = 3;
    private float waitGameTimeNext;
    private float waitSpriteTime = 3;
    private float waitSpriteTimeNext;
    private List<GamesDatas> gamesDatas = new List<GamesDatas>();

    private void Update()
    {
        if (!canChangeSprite)
        {
            return;
        }
        ChangeGame();
        ChangeSprite();
    }
    private void ChangeGame()
    {
        waitGameTimeNext += Time.deltaTime;
        if (waitGameTimeNext >= waitGameTime)
        {
            waitGameTimeNext = 0;
            waitSpriteTimeNext = 0;
            spriteOrder = 0;
            gameOrder++;
            gameOrder = gameOrder == gamesDatas.Count ? 0 : gameOrder;
            waitGameTime = gamesDatas[gameOrder].gameSprites.Count * 3;
            gameNameText.text = gamesDatas[gameOrder].gameName;
            gameButton.onClick.RemoveAllListeners();
            gameButton.onClick.AddListener(() => GoGameLink(gamesDatas[gameOrder].gameLink));
            gameImage.texture = gamesDatas[gameOrder].gameSprites[spriteOrder];
        }
    }
    private void ChangeSprite()
    {
        waitSpriteTimeNext += Time.deltaTime;
        if (waitSpriteTimeNext >= waitSpriteTime)
        {
            waitSpriteTimeNext = 0;
            spriteOrder++;
            spriteOrder = spriteOrder == gamesDatas[gameOrder].gameSprites.Count ? 0 : spriteOrder;
            gameImage.texture = gamesDatas[gameOrder].gameSprites[spriteOrder];
        }
    }
    private void GoGameLink(string url)
    {
        Application.OpenURL(url);
    }
    public void SetGameButton(List<GameDatas> datas)
    {
        for (int e = 0; e < datas.Count; e++)
        {
            if (gameName != datas[e].Datas[0])
            {
                gamesDatas.Add(new GamesDatas(datas[e].Datas[0], datas[e].Datas[1]));
                for (int h = 2; h < datas[e].Datas.Count; h++)
                {
                    int listOrder = e;
                    StartCoroutine(GetSpriteData(datas[e].Datas[h], listOrder));
                }
            }
        }
    }
    IEnumerator GetSpriteData(string url, int listOrder)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(driveStartLink + url);
        unityWebRequest.downloadHandler = new DownloadHandlerTexture();

        yield return unityWebRequest.SendWebRequest();
        UnityWebRequest.Result result = unityWebRequest.result;
        if (result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Bilgi gelmedi." + unityWebRequest.error);
        }
        else
        {
            gamesDatas[listOrder].gameSprites.Add(DownloadHandlerTexture.GetContent(unityWebRequest));
            if (!canChangeSprite)
            {
                canChangeSprite = true;
                gameOrder = listOrder;
                waitGameTime = gamesDatas[gameOrder].gameSprites.Count * 3;
                gameNameText.text = gamesDatas[gameOrder].gameName;
                gameButton.onClick.RemoveAllListeners();
                gameButton.onClick.AddListener(() => GoGameLink(gamesDatas[gameOrder].gameLink));
                gameImage.texture = gamesDatas[gameOrder].gameSprites[spriteOrder];
                canvasGroup.alpha = 1;
            }
        }
        unityWebRequest.Dispose();
    }
}