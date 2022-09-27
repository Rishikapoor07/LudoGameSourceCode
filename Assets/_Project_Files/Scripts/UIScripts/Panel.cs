using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
	public void Show() => this.gameObject.SetActive(true);
	public void Hide() => this.gameObject.SetActive(false);

    public Text[] textToFix;
    private void Start()
    {
        foreach (Text _text in textToFix) if (_text != null) _text.text = ArabicFixer.Fix(_text.text, false, false);

        foreach (TMPro.TextMeshProUGUI _text in tmp_IFText) if (_text != null) _text.text = ArabicFixer.Fix(_text.text, false, false);
    }

    public TMPro.TextMeshProUGUI[] tmp_IFText;
}
