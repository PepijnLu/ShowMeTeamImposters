using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleFadeInWithSoundAndDualTransition : MonoBehaviour
{
    public Image titleImage; 
    public Image nextImage;  
    public Image additionalImage; 
    public Image backgroundImage;
    public float fadeInDuration = 2f; 
    public float waitBeforeFadeOut = 4f; 
    public float fadeOutDuration = 2f; 
    public float overlapDuration = 0.5f;

    public AudioSource audioSource; 
    public AudioClip fadeOutSound; 

    private void Start()
    {
        SetImageAlpha(titleImage, 0f);
        SetImageAlpha(nextImage, 0f);
        SetImageAlpha(additionalImage, 0f);
        SetImageAlpha(backgroundImage, 0f);

        StartCoroutine(FadeInTitleImage());

        backgroundImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeInTitleImage()
    {
        yield return new WaitForSeconds(3f);
        
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            SetImageAlpha(titleImage, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(titleImage, 1f);

        yield return new WaitForSeconds(waitBeforeFadeOut - 1f);

        if (audioSource && fadeOutSound)
        {
            audioSource.PlayOneShot(fadeOutSound);
        }

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeOutTitleAndFadeInNext());
    }

    private IEnumerator FadeOutTitleAndFadeInNext()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            float titleAlpha = Mathf.Clamp01(1 - (elapsedTime / fadeOutDuration));
            float nextAlpha = Mathf.Clamp01((elapsedTime - fadeOutDuration + overlapDuration) / overlapDuration);

            SetImageAlpha(titleImage, titleAlpha);
            SetImageAlpha(nextImage, nextAlpha);
            SetImageAlpha(additionalImage, nextAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(titleImage, 0f);
        SetImageAlpha(nextImage, 1f);
        SetImageAlpha(additionalImage, 1f);

        titleImage.gameObject.SetActive(false);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}



// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class TitleFadeInWithSoundAndTransition : MonoBehaviour
// {
//     public Image titleImage; 
//     public Image nextImage; 
//     public float fadeInDuration = 2f; 
//     public float waitBeforeFadeOut = 2f; 
//     public float fadeOutDuration = 2f; 
//     public float overlapDuration = 0.5f; 

//     public AudioSource audioSource; 
//     public AudioClip fadeOutSound; 

//     private void Start()
//     {

//         SetImageAlpha(titleImage, 0f);
//         SetImageAlpha(nextImage, 0f);

//         StartCoroutine(FadeInTitleImage());
//     }

//     private IEnumerator FadeInTitleImage()
//     {
//         yield return new WaitForSeconds(3f);
        
//         float elapsedTime = 0f;

//         while (elapsedTime < fadeInDuration)
//         {
//             float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
//             SetImageAlpha(titleImage, alpha);

//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }

//         SetImageAlpha(titleImage, 1f);

//         yield return new WaitForSeconds(waitBeforeFadeOut - 1f);

//         if (audioSource && fadeOutSound)
//         {
//             audioSource.PlayOneShot(fadeOutSound);
//         }

//         yield return new WaitForSeconds(1f);

//         StartCoroutine(FadeOutTitleAndFadeInNext());
//     }

//     private IEnumerator FadeOutTitleAndFadeInNext()
//     {
//         float elapsedTime = 0f;

//         while (elapsedTime < fadeOutDuration)
//         {
//             float titleAlpha = Mathf.Clamp01(1 - (elapsedTime / fadeOutDuration));
//             float nextAlpha = Mathf.Clamp01((elapsedTime - fadeOutDuration + overlapDuration) / overlapDuration);

//             SetImageAlpha(titleImage, titleAlpha);
//             SetImageAlpha(nextImage, nextAlpha);

//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }

//         SetImageAlpha(titleImage, 0f);
//         SetImageAlpha(nextImage, 1f);
//     }

//     private void SetImageAlpha(Image image, float alpha)
//     {
//         Color color = image.color;
//         color.a = alpha;
//         image.color = color;
//     }
// }
