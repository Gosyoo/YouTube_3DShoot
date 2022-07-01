using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class GameUI : MonoBehaviour {
    public Image fadePanel; //褪色背板
    public GameObject gameOverUI;

    void Start() {
        FindObjectOfType<Player>().OnDeath += OnGmaeOver;
    }

    void OnGmaeOver() {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    /// <summary>
    /// 褪色
    /// </summary>
    /// <param name="from">初始颜色</param>
    /// <param name="to">蜕变颜色</param>
    /// <param name="time">褪色时间</param>
    /// <returns></returns>
    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePanel.color = Color.Lerp(from, to, percent);
            yield return null;
        }

    }

    public void PlayAgainGame() {
        EditorSceneManager.LoadScene("Game");
    }
}
