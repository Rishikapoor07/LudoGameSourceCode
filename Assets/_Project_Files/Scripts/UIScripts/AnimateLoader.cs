using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimateLoader : MonoBehaviour
{
    [SerializeField] Sprite[] loaderImageSprites;
    [SerializeField] Image loaderImage;
    float animSpeed;
    bool b_startLoaderAnim;
    public bool b_StartLoaderAnim
    {
        get
        {
            return b_startLoaderAnim;
        }
        set
        {
            b_startLoaderAnim = value;
            if (b_startLoaderAnim) animCoroutine = StartCoroutine(StartLoaderAnim());
            else
            {
                if (animCoroutine != null)
                {
                    StopCoroutine(animCoroutine);
                    animCoroutine = null;
                }
            }
        }
    }
    Coroutine animCoroutine;
    int loaderImageIndex;

    private void OnEnable()
    {
        b_StartLoaderAnim = true;
    }

    private void OnDisable()
    {
        b_StartLoaderAnim = false;
    }

    IEnumerator StartLoaderAnim()
    {
        animSpeed = 0.08f;
        while (b_StartLoaderAnim)
        {
            loaderImageIndex = (loaderImageIndex + 1) % loaderImageSprites.Length;

            yield return new WaitForSeconds(animSpeed);
            loaderImage.sprite = loaderImageSprites[loaderImageIndex];
        }
    }
}
