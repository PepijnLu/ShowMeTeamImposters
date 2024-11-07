using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public Image backgroundImage; 
    public float fadeInDuration = 2f; 
    public float waitBeforeFadeIn = 1f;

    public void StartGame() 
    {
        StartCoroutine(FadeIn());

        SceneManager.LoadScene("Art Stage");
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(waitBeforeFadeIn);
        
        backgroundImage.gameObject.SetActive(true);
        
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            SetImageAlpha(alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(1f);
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = backgroundImage.color;
        color.a = alpha;
        backgroundImage.color = color;
    }
}
