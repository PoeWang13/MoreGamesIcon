using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[System.Serializable]
public class GameDatas
{
    public List<string> Datas = new List<string>();
}
[System.Serializable]
public class GameDataContainer
{
    public List<GameDatas> GameDatas;
}
public class GamesData : MonoBehaviour
{
    /// Nasýl yapýlacak
    /// 1-Driveda oyun için yeni klasör aç ve ismini oyunun ismi yap
    /// 2-Oyunun resimlerini klasöre yükle
    /// 3-Resimlere sað týkla Baðlantýyý Al seçeneðini seç
    /// 4-Kýsýtlanmýþ tikine týkla ve seçeneði Baðlantýya Sahip Olan Herkes olarak deðiþtir.
    /// 5-Baðlantý linkini kopyala. Þöyle bir þey olmasý lazým
    /// https://drive.google.com/file/d/0B6kWpoVPKv9DVkFudENneU9QX2s/view?usp=sharing&resourcekey=0-Oley_7RdN9VXlu3DpM9CmA
    /// 6-file/d/Kod No/view? kýsmýndaki Kod No kýsmýný seç ve kopyala
    /// 7-Drivedaki More Game Data klasörü içindeki jsonu aç ve oyun için baþka bir { } li kýsým oluþtur
    /// 8-Kýsým içindekileri yeni oyuna göre doldur.
    /// 9-Resim için gerekli url kýsmýna 6. adýmda kopyaladýðýn link kýsýmlarýný yapýþtýr.
    
    [Header("Data Atamalarý")]
    private string driveStartLink = "https://drive.google.com/uc?export=download&id=";
    [SerializeField] private string gameName = "Brain Master";
    [SerializeField] private string driveJsonLink = "1UEQ7xYorgh9H3lwQizcDyRX4dxceQwY-";
    [Header("Script Atamalarý")]
    [SerializeField] private AllGamesButton allGamesButton;
    [SerializeField] private RectTransform gameDataButton;
    [SerializeField] private RectTransform gamesDataHolder;
    [SerializeField] private Transform gamesDataButtonParent;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private GameDataContainer gameDataStruct = new GameDataContainer();

    private void Awake()
    {
        StartCoroutine(GetGamesData(driveStartLink + driveJsonLink));
    }
    IEnumerator GetGamesData(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        int order = 0;

        yield return unityWebRequest.SendWebRequest();
        UnityWebRequest.Result result = unityWebRequest.result;
        if (result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Bilgi gelmedi." + unityWebRequest.error);
        }
        else
        {
            gamesDataHolder.gameObject.SetActive(true);
            gameDataStruct = JsonUtility.FromJson<GameDataContainer>(unityWebRequest.downloadHandler.text);

            allGamesButton.SetGameButton(gameDataStruct.GameDatas);

            float sizeDeltaX = gameDataButton.sizeDelta.x;
            float sizeDeltaY = gamesDataButtonParent.GetComponent<RectTransform>().rect.height - 30;
            for (int e = 0; e < gameDataStruct.GameDatas.Count; e++)
            {
                if (gameName != gameDataStruct.GameDatas[e].Datas[0])
                {
                    RectTransform gDB = Instantiate(gameDataButton, gamesDataButtonParent);
                    gDB.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
                    gDB.GetComponent<GamesButton>().SetGameButton(gameDataStruct.GameDatas[e].Datas);
                }
            }
            order = gameDataStruct.GameDatas.Count;
        }
        float size = (175 * order) + 25;
        float canvasSize = canvas.GetComponent<RectTransform>().rect.width;
        if (size <= canvasSize - 50)
        {
            gamesDataHolder.sizeDelta = new Vector2(size, 0);
        }
        else
        {
            gamesDataHolder.sizeDelta = new Vector2(canvasSize - 50, 0);
        }
        unityWebRequest.Dispose();
    }
}