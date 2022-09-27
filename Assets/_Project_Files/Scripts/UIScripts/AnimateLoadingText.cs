using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimateLoadingText : MonoBehaviour
{
    [SerializeField] Text textToAnimate;
    bool startAnimating;
    public bool StartAnimating
    {
        get=>startAnimating;
        set
        {
            startAnimating = value;
            if(startAnimating) StartCoroutine(StartTextAnimation());
            else StopCoroutine(StartTextAnimation());
        }
    }
    [SerializeField] int dotLength;
    public int DotLength
    {
        get=>dotLength;
        set
        {
            dotLength = value;
            if(dotLength==1) textToAnimate.text = ".";
            if(dotLength==2) textToAnimate.text = "..";
            if(dotLength==3) textToAnimate.text = "...";
        }
    }
    [Range(0.1f,1)]
    public float animSpeed;

    private void OnEnable()
    {
        DotLength = 3;
        StartAnimating=true;
    }

    private void OnDisable()
    {
        StartAnimating=false;
    }

    public Text[] textToFix;
    private void Start()
    {
        foreach (Text _text in textToFix) _text.text = ArabicSupport.ArabicFixer.Fix(_text.text, true, true);
    }

    IEnumerator StartTextAnimation()
    {
        startAnimating = true;
        while(startAnimating)
        {
            yield return new WaitForSeconds(animSpeed);
            DotLength += 1;
            if(DotLength>=4) DotLength=1;
        }
    }
}
