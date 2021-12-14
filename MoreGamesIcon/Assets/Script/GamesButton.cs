using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GamesButton : MonoBehaviour
{
    private string driveStartLink = "https://drive.google.com/uc?export=download&id=";
    [Header("Script Atamalarý")]
    [SerializeField] private TextMeshProUGUI gameNameText;
    [SerializeField] private Button gameButton;
    [SerializeField] private RawImage gameImage;
    [SerializeField] private CanvasGroup canvasGroup;
    private List<Texture> gameSprites = new List<Texture>();
    private float waitSpriteTime = 1;
    private float waitSpriteTimeNext;
    private int waitNextSprite;
    private bool canChangeSprite = false;

    private void Update()
    {
        if (!canChangeSprite)
        {
            return;
        }
        waitSpriteTimeNext += Time.deltaTime;
        if (waitSpriteTimeNext >= waitSpriteTime)
        {
            waitSpriteTimeNext = 0;
            waitSpriteTime = Random.Range(2, 5);
            waitNextSprite++;
            waitNextSprite = waitNextSprite == gameSprites.Count ? 0 : waitNextSprite;
            gameImage.texture = gameSprites[waitNextSprite];
        }
    }
    private void GoGameLink(string url)
    {
        Application.OpenURL(url);
    }
    public void SetGameButton(List<string> datas)
    {
        gameNameText.text = datas[0];
        gameButton.onClick.AddListener(() => GoGameLink(datas[1]));
        for (int e = 2; e < datas.Count; e++)
        {
            StartCoroutine(GetSpriteData(datas[e]));
        }
    }
    IEnumerator GetSpriteData(string url)
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
            canvasGroup.alpha = 1;
            AddGameSprite(DownloadHandlerTexture.GetContent(unityWebRequest));
        }
        unityWebRequest.Dispose();
    }
    public void AddGameSprite(Texture gameTexture)
    {
        gameSprites.Add(gameTexture);
        if (gameSprites.Count == 1)
        {
            gameImage.texture = gameTexture;
            canChangeSprite = true;
        }
    }
}