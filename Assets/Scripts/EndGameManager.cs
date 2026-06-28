using Photon.Pun;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public TMP_Text isWinning;
    public TMP_Text winner;
    public TMP_Text waiting;
    public float timeLimit = 10;
    private float delta = 0f;
    private float span = 0.5f;
    private float currentTime;           // 內部記憶當前剩下幾秒
    private bool isTimerRunning = false; // 計時器開關
    void Start()
    {
        StartTimer(10);
        if (PhotonNetwork.IsConnected == false)
        {
            //SceneManager.LoadScene("StartScene");
            return;
        }
        if (StaticData.winnerName != "")
        {
            winner.text = "贏家是 " + StaticData.winnerName;
        }
        else
        {
            winner.text = "一群廢物，竟然全死了";
        }
        if (StaticData.winnerName == PhotonNetwork.LocalPlayer.NickName)
        {
            isWinning.text = "呀呼！贏啦！";
        }
        if (StaticData.winnerName == PhotonNetwork.LocalPlayer.NickName)
        {
            isWinning.text = "你社死了，菜雞";
        }
    }
    // 啟動計時器
    public void StartTimer(float timeLimit)
    {
        currentTime = timeLimit;
        isTimerRunning = true;
    }

    // 關閉計時器
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // 時間到的強制懲罰
    private void TimeUp()
    {
        SceneManager.LoadScene("ReadyScene");
    }
    // Update is called once per frame
    void Update()
    {
        // === 【新增】：每幀扣除時間 ===
        delta += Time.deltaTime;
        int dots = ((int)(delta / span)) % 3;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= dots; i++)
        {
            sb.Append(".");
        }
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime; // Time.deltaTime 是上一幀到這一幀經過的秒數

            waiting.text = Mathf.CeilToInt(currentTime).ToString() + "秒後回到房間畫面" + sb.ToString();

            if (currentTime <= 0)
            {
                currentTime = 0;
                StopTimer();
                TimeUp(); // 觸發超時懲罰！
            }
        }
    }
}