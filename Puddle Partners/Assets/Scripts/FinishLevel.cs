using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class FinishLevel : NetworkBehaviour
{

    private NetworkVariable<bool> finishedLevel = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<FixedString4096Bytes> scoreboardData = new NetworkVariable<FixedString4096Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> water = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> syncTime = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float levelTime = 0.0f;
    private float elapsedTime = 0.0f;
    private float nowTime = 0.0f;
    private bool gameover = false;
    private RectTransform rt;
    public GameObject errorPanel;
    public TMP_Text errorMsg;
    public GameObject scoreboardCanvas;
    public GameObject errorCanvas;
    public GameObject mainMenue;
    public GameObject nextLevel;
    public GameObject scoreboardPanel;
    public GameObject waterRect;
    public GameObject gameoverPanel;
    public int lvl;
    public float waterMaxHeigth = 0.0f;
    public float waterSpeedHeigth = 0.0f;
    public bool lastLevel = false;

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public int state;

        public int getState() { return state; }
    }

    [System.Serializable]
    public class ScoreData
    {
        public ArrayList score;

        public ArrayList getScore() { return score; }
    }

    [System.Serializable]
    public class Error
    {
        public ArrayList error;
        public ArrayList getError() { return error; }
    }

    public class Score
    {
        private int[] rank = new int[51];
        private string[] player_1 = new string[51];
        private string[] player_2 = new string[51];
        private int[] level = new int[51];
        private int[] own = new int[51];
        private int[] new_score = new int[51];
        private int[] better = new int[51];
        private string[] old_time = new string[51];
        private string[] now_time = new string[51];
        private int data_count;

        public int GetRank(int i)
        {
            return rank[i];
        }

        public void SetRank(int i, int dat)
        {
            rank[i] = dat;
        }

        public string GetPlayer1(int i)
        {
            return player_1[i];
        }

        public void SetPlayer1(int i, string dat)
        {
            player_1[i] = dat;
        }

        public string GetPlayer2(int i)
        {
            return player_2[i];
        }

        public void SetPlayer2(int i, string dat)
        {
            player_2[i] = dat;
        }

        public int GetLevel(int i)
        {
            return level[i];
        }

        public void SetLevel(int i, int dat)
        {
            level[i] = dat;
        }

        public int GetOwn(int i)
        {
            return own[i];
        }

        public void SetOwn(int i, int dat)
        {
            own[i] = dat;
        }

        public int GetNewScore(int i)
        {
            return new_score[i];
        }

        public void SetNewScore(int i, int dat)
        {
            new_score[i] = dat;
        }

        public int GetBetter(int i)
        {
            return better[i];
        }

        public void SetBetter(int i, int dat)
        {
            better[i] = dat;
        }

        public string GetOldTime(int i)
        {
            return old_time[i];
        }

        public void SetOldTime(int i, string dat)
        {
            old_time[i] = dat;
        }

        public string GetNowTime(int i)
        {
            return now_time[i];
        }

        public void SetNowTime(int i, string dat)
        {
            now_time[i] = dat;
        }

        public int GetDataCount()
        {
            return data_count;
        }

        public void SetDataCount(int dat)
        {
            data_count = dat;
        }
    }

    private string HttpServer()
    {
        string url = "";
        StreamReader reader = new StreamReader(Application.dataPath + "/config/server.pudl");
        url = reader.ReadToEnd();
        reader.Close();
        return url;
    }

    private void ErrorMsg(ArrayList msg)
    {
        string showMsg = "";
        for (int i = 0; i < msg.Count; i++)
        {
            showMsg += msg[i] + "\n\n";
        }
        errorMsg.text = showMsg;
        errorPanel.SetActive(true);
        errorCanvas.SetActive(true);
    }

    public void ErrorMsgOK()
    {
        errorPanel.SetActive(false);
        errorCanvas.SetActive(false);
        errorMsg.text = "";
    }

    public void Start()
    {
        rt = waterRect.GetComponent<RectTransform>();
        if (NetworkManager.IsHost)
        {
            nowTime = Time.time;
        }
        finishedLevel.OnValueChanged += SetLevel;
        water.OnValueChanged += SetWater;
        if (!NetworkManager.IsHost)
        {
            scoreboardData.OnValueChanged += GetScore;
            syncTime.OnValueChanged += ClientGameover;
        }
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    public void Update()
    {
        if (!gameover && !finishedLevel.Value)
        {
            if (water.Value)
            {
                if (rt.localScale.y < waterMaxHeigth)
                {
                    rt.transform.localScale = new Vector3(rt.transform.localScale.x, (rt.transform.localScale.y + waterSpeedHeigth), 1);
                    rt.transform.position = new Vector3(rt.transform.position.x, (rt.transform.position.y + (waterSpeedHeigth/2)), rt.transform.position.z);
                }
                else
                {
                    if (NetworkManager.IsHost)
                    {
                        Gameover();
                    }
                }
            }
            if (!finishedLevel.Value && !water.Value)
            {
                elapsedTime = GetCurrentTimeOffset(nowTime);
                if (elapsedTime > levelTime && NetworkManager.IsHost)
                {
                    WaterServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void WaterServerRpc(ServerRpcParams serverRpcParams = default)
    {
        water.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        finishedLevel.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientCallServerRpc(FixedString4096Bytes board, ServerRpcParams serverRpcParams = default)
    {
        scoreboardData.Value = board;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncTimeServerRpc(ServerRpcParams serverRpcParams = default)
    {
        syncTime.Value = elapsedTime;
    }

    private void Gameover()
    {
        gameover = true;
        if(NetworkManager.IsHost)
        {
            GameObject newChance = gameoverPanel.transform.GetChild(0).gameObject.gameObject.transform.GetChild(2).gameObject;
            GameObject returnMain = gameoverPanel.transform.GetChild(0).gameObject.gameObject.transform.GetChild(3).gameObject;
            newChance.SetActive(true);
            //returnMain.SetActive(true);
            SyncTimeServerRpc();
        }
        TimeSpan time = TimeSpan.FromSeconds(syncTime.Value);
        GameObject lvlTime = gameoverPanel.transform.GetChild(0).gameObject.gameObject.transform.GetChild(0).gameObject;
        lvlTime.GetComponent<TextMeshProUGUI>().text = "Zeit: " + time.ToString("hh':'mm':'ss");
        gameoverPanel.SetActive(true);
    }

    private void ClientGameover(float oldVal, float newVal) {
        Gameover();
    }

    public void retry()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void SetWater(bool oldVal, bool newVal)
    {
        waterRect.SetActive(true);
    }

    private static float GetCurrentTimeOffset(float continueTime)
    {
        return Time.time - continueTime;
    }

    private void SetLevel(bool oldVal, bool newVal)
    {
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/set_level.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "ID=" + PlayerPrefs.GetString("LoginID") + "&LEVEL=" + lvl.ToString();
        byte[] data = encoding.GetBytes(postData);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;
        Stream stream = request.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();
        WebResponse response = request.GetResponse();
        stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);
        string responseText = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        try
        {
            LevelData playerData = JsonConvert.DeserializeObject<LevelData>(responseText);
            if (playerData.getState() != 0)
            {
                if (NetworkManager.IsHost)
                {
                    SetScores();
                    //mainMenue.SetActive(true);
                    if (!lastLevel)
                    {
                        nextLevel.SetActive(true);
                    }
                }
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(responseText);
                ErrorMsg(error.getError());
            }
        }
        catch (Exception)
        {
            ArrayList err = new ArrayList();
            err.Add("Kein g�ltiges R�ckgabeformat vom Webserver");
            ErrorMsg(err);
        }
    }

    private void SetScores()
    {
        elapsedTime = GetCurrentTimeOffset(nowTime);
        TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
        WebRequest request = WebRequest.Create("http://" + HttpServer() + "/puddle_partners/scoreboard.php");
        ASCIIEncoding encoding = new ASCIIEncoding();
        string postData = "HOST_ID=" + PlayerPrefs.GetString("HostID") + "&CLIENT_ID=" + PlayerPrefs.GetString("ClientID") + "&LEVEL=" + lvl.ToString() + "&TIME=" + time.ToString("hh':'mm':'ss");
        byte[] data = encoding.GetBytes(postData);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;
        Stream stream = request.GetRequestStream();
        stream.Write(data, 0, data.Length);
        stream.Close();
        WebResponse response = request.GetResponse();
        stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);
        string responseText = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        try
        {
            ScoreData sData = JsonConvert.DeserializeObject<ScoreData>(responseText);
            ArrayList aData = sData.getScore();
            if(aData.Count != 0)
            {
                Score score = new Score();
                int s = 0;
                for(int i = 0; i < aData.Count; i+=9)
                {
                    score.SetRank(s, Int32.Parse(aData[i].ToString()));
                    score.SetPlayer1(s, aData[(i + 1)].ToString());
                    score.SetPlayer2(s, aData[(i + 2)].ToString());
                    score.SetLevel(s, Int32.Parse(aData[(i + 3)].ToString()));
                    score.SetOwn(s, Int32.Parse(aData[(i + 4)].ToString()));
                    score.SetNewScore(s, Int32.Parse(aData[(i + 5)].ToString()));
                    score.SetBetter(s, Int32.Parse(aData[(i + 6)].ToString()));
                    score.SetOldTime(s, aData[(i + 7)].ToString());
                    score.SetNowTime(s, aData[(i + 8)].ToString());
                    s++;
                }
                score.SetDataCount((aData.Count/9));
                CreateScoreboard(score);
                ClientCallServerRpc(responseText);
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(responseText);
                ErrorMsg(error.getError());
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            ArrayList err = new ArrayList();
            err.Add("Kein g�ltiges R�ckgabeformat vom Webserver");
            ErrorMsg(err);
        }
    }

    private void CreateScoreboard(Score score)
    {
        GameObject scorePanel = scoreboardPanel.transform.GetChild(0).gameObject.gameObject.transform.GetChild(0).gameObject.gameObject.transform.GetChild(0).gameObject;
        GameObject ownScorePanel = scoreboardPanel.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        GameObject template = scorePanel.transform.GetChild(0).gameObject;
        int dataSize = score.GetDataCount();
        bool is_outranged = false;
        if(dataSize == 51)
        {
            dataSize--;
            is_outranged = true;
        }
        for (int i = 0; i < dataSize; i++)
        {
            GameObject newScore = Instantiate(template);
            newScore.transform.SetParent(scorePanel.transform, false);
            newScore.name = "Score_Rank_" + (i + 1).ToString();
            GameObject scoreRank = newScore.transform.GetChild(2).gameObject;
            scoreRank.GetComponent<TextMeshProUGUI>().text = score.GetRank(i).ToString();
            GameObject scorePlayer1 = newScore.transform.GetChild(3).gameObject;
            scorePlayer1.GetComponent<TextMeshProUGUI>().text = score.GetPlayer1(i);
            GameObject scorePlayer2 = newScore.transform.GetChild(4).gameObject;
            scorePlayer2.GetComponent<TextMeshProUGUI>().text = score.GetPlayer2(i);
            GameObject scoreNowTime = newScore.transform.GetChild(5).gameObject;
            scoreNowTime.GetComponent<TextMeshProUGUI>().text = score.GetNowTime(i);
            newScore.SetActive(true);
            if(score.GetOwn(i) == 1)
            {
                GameObject ownScoreRank = ownScorePanel.transform.GetChild(1).gameObject;
                ownScoreRank.GetComponent<TextMeshProUGUI>().text = score.GetRank(i).ToString();
                GameObject ownScorePlayer1 = ownScorePanel.transform.GetChild(2).gameObject;
                ownScorePlayer1.GetComponent<TextMeshProUGUI>().text = score.GetPlayer1(i);
                GameObject ownScorePlayer2 = ownScorePanel.transform.GetChild(3).gameObject;
                ownScorePlayer2.GetComponent<TextMeshProUGUI>().text = score.GetPlayer2(i);
                if(score.GetNewScore(i) == 1)
                {
                    GameObject ownIsNew = ownScorePanel.transform.GetChild(4).gameObject;
                    ownIsNew.GetComponent<TextMeshProUGUI>().text = "NEU";
                    GameObject ownScoreOldTime = ownScorePanel.transform.GetChild(5).gameObject;
                    ownScoreOldTime.GetComponent<TextMeshProUGUI>().text = "Alt: -";
                } else if (score.GetNewScore(i) == 0 && score.GetBetter(i) == 0)
                {
                    GameObject ownIsNew = ownScorePanel.transform.GetChild(4).gameObject;
                    ownIsNew.GetComponent<TextMeshProUGUI>().text = "Schlechter";
                    GameObject ownScoreOldTime = ownScorePanel.transform.GetChild(5).gameObject;
                    ownScoreOldTime.GetComponent<TextMeshProUGUI>().text = "Alt:  "+ score.GetOldTime(i);
                } else if (score.GetNewScore(i) == 0 && score.GetBetter(i) == 1)
                {
                    GameObject ownIsNew = ownScorePanel.transform.GetChild(4).gameObject;
                    ownIsNew.GetComponent<TextMeshProUGUI>().text = "Besser";
                    GameObject ownScoreOldTime = ownScorePanel.transform.GetChild(5).gameObject;
                    ownScoreOldTime.GetComponent<TextMeshProUGUI>().text = "Alt: " + score.GetOldTime(i);
                }
                GameObject ownScoreNowTime = ownScorePanel.transform.GetChild(6).gameObject;
                ownScoreNowTime.GetComponent<TextMeshProUGUI>().text = "Neu: " + score.GetNowTime(i);
            }
        }
        if(is_outranged)
        {
            GameObject ownScoreRank = ownScorePanel.transform.GetChild(1).gameObject;
            ownScoreRank.GetComponent<TextMeshProUGUI>().text = score.GetRank(50).ToString();
            GameObject ownScorePlayer1 = ownScorePanel.transform.GetChild(2).gameObject;
            ownScorePlayer1.GetComponent<TextMeshProUGUI>().text = score.GetPlayer1(50);
            GameObject ownScorePlayer2 = ownScorePanel.transform.GetChild(3).gameObject;
            ownScorePlayer2.GetComponent<TextMeshProUGUI>().text = score.GetPlayer2(50);
            if (score.GetNewScore(50) == 0)
            {
                GameObject ownIsNew = ownScorePanel.transform.GetChild(4).gameObject;
                ownIsNew.GetComponent<TextMeshProUGUI>().text = "NEU";
                GameObject ownScoreOldTime = ownScorePanel.transform.GetChild(5).gameObject;
                ownScoreOldTime.GetComponent<TextMeshProUGUI>().text = "Alt: -";
            }
            else if (score.GetNewScore(50) == 1 && score.GetBetter(50) == 0)
            {
                GameObject ownIsNew = ownScorePanel.transform.GetChild(4).gameObject;
                ownIsNew.GetComponent<TextMeshProUGUI>().text = "Schlechter";
                GameObject ownScoreOldTime = ownScorePanel.transform.GetChild(5).gameObject;
                ownScoreOldTime.GetComponent<TextMeshProUGUI>().text = "Alt:  " + score.GetOldTime(50);
            }
            else if (score.GetNewScore(50) == 1 && score.GetBetter(50) == 1)
            {
                GameObject ownIsNew = ownScorePanel.transform.GetChild(4).gameObject;
                ownIsNew.GetComponent<TextMeshProUGUI>().text = "Besser";
                GameObject ownScoreOldTime = ownScorePanel.transform.GetChild(5).gameObject;
                ownScoreOldTime.GetComponent<TextMeshProUGUI>().text = "Alt: " + score.GetOldTime(50);
            }
            GameObject ownScoreNowTime = ownScorePanel.transform.GetChild(6).gameObject;
            ownScoreNowTime.GetComponent<TextMeshProUGUI>().text = "Neu: " + score.GetNowTime(50);
        }
        scoreboardCanvas.SetActive(true);
    }

    private void GetScore(FixedString4096Bytes oldVal, FixedString4096Bytes newVal)
    {
        string responseText = newVal.Value;
        ScoreData sData = JsonConvert.DeserializeObject<ScoreData>(responseText);
        ArrayList aData = sData.getScore();
        if (aData.Count != 0)
        {
            Score score = new Score();
            int s = 0;
            for (int i = 0; i < aData.Count; i += 9)
            {
                score.SetRank(s, Int32.Parse(aData[i].ToString()));
                score.SetPlayer1(s, aData[(i + 1)].ToString());
                score.SetPlayer2(s, aData[(i + 2)].ToString());
                score.SetLevel(s, Int32.Parse(aData[(i + 3)].ToString()));
                score.SetOwn(s, Int32.Parse(aData[(i + 4)].ToString()));
                score.SetNewScore(s, Int32.Parse(aData[(i + 5)].ToString()));
                score.SetBetter(s, Int32.Parse(aData[(i + 6)].ToString()));
                score.SetOldTime(s, aData[(i + 7)].ToString());
                score.SetNowTime(s, aData[(i + 8)].ToString());
                s++;
            }
            score.SetDataCount((aData.Count / 9));
            CreateScoreboard(score);
        }
    }

    public void ErrorBack()
    {
        PlayerPrefs.SetString("HostID", "");
        PlayerPrefs.SetString("HostName", "");
        PlayerPrefs.SetString("ClientID", "");
        PlayerPrefs.SetString("ClientName", "");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("LoginRegister");
    }

    public void Back()
    {
        PlayerPrefs.SetString("HostID", "");
        PlayerPrefs.SetString("HostName", "");
        PlayerPrefs.SetString("ClientID", "");
        PlayerPrefs.SetString("ClientName", "");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("LoginRegister");
    }

    public void NextLevel()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Level_" + (lvl + 1).ToString(), LoadSceneMode.Single);
    }

    public void OnClientDisconnectCallback(ulong id)
    {
        PlayerPrefs.SetString("HostID", "");
        PlayerPrefs.SetString("HostName", "");
        PlayerPrefs.SetString("ClientID", "");
        PlayerPrefs.SetString("ClientName", "");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("LoginRegister");
    }

}