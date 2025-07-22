using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 用の名前空間

public class TextFadeController : MonoBehaviour
{
    public TextMeshProUGUI messageText; // フェードするText (または TextMeshProUGUI)
    public float fadeInDuration = 1.0f; // フェードインの時間
    public float displayDuration = 2.0f; // テキストが表示される時間
    public float fadeOutDuration = 1.0f; // フェードアウトの時間

    private Coroutine fadeCoroutine;

    void Start()
    {
        // 初期状態でテキストを完全に透明にする
        if (messageText != null)
        {
            SetAlpha(0);
        }
    }

    // 特定の条件で呼び出す
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message; // テキスト内容を設定
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // 前回のフェード処理を停止
            }
            fadeCoroutine = StartCoroutine(FadeInAndOut());
        }
    }

    private IEnumerator FadeInAndOut()
    {
        // フェードイン
        yield return FadeText(0, 1, fadeInDuration);

        // 指定時間表示
        yield return new WaitForSeconds(displayDuration);

        // フェードアウト
        yield return FadeText(1, 0, fadeOutDuration);
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float timeElapsed = 0;
        Color currentColor = messageText.color;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            currentColor.a = alpha;
            messageText.color = currentColor;
            yield return null;
        }

        // 最終的な値を確定
        currentColor.a = endAlpha;
        messageText.color = currentColor;
    }

    private void SetAlpha(float alpha)
    {
        if (messageText != null)
        {
            Color color = messageText.color;
            color.a = alpha;
            messageText.color = color;
        }
    }
}
