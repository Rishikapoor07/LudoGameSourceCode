using System;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	#region Variables

	[SerializeField] GameObject prizePopUp;
	[SerializeField] GameObject muteMicPopUp;
	[SerializeField] GameObject searchingOpponentsPopUp;
	[SerializeField] GameObject findingPlayersPopUp;
	[SerializeField] GameObject winnerPopUp;
	[SerializeField] GameObject quickGemPurchasePopUp;
	[SerializeField] GameObject gemShopPopUp;
	[SerializeField] GameObject letsPlayScreen;
	[SerializeField] InputField chat_IF;
	//[SerializeField] TMPro.TMP_InputField chat_IF_TMP;
	[SerializeField] Text textToShow;
	[SerializeField] Text chatTextCounts;
	[SerializeField] Button micButton;
	[SerializeField] Button[] opponentsMicButton;
	[SerializeField] Sprite[] micButtonImages;
	[SerializeField] Button ExitButton;
	[SerializeField] Text[] GemCountText;
	[SerializeField] Button[] IAP_Buttons;
	public static Action OnIAPDone;

    #endregion Variables

    #region Unity Callbacks

    private void OnEnable()
    {
		ResetMembers();
    }

    private void Start()
	{
		ExitButton.interactable = false;
		EnableGameObject(searchingOpponentsPopUp);
		HelperUtil.CallAfterDelay(() =>
		{
			EnableGameObject(prizePopUp);
			DisableGameObject(searchingOpponentsPopUp);
			ExitButton.interactable = true;
		}, 2, () => GameState.CurrentScene != GameScene.GamePlay);

		HelperUtil.HideLoading();
		HelperUtil.HideLoader();

		chatTextCounts.text = "0/200";
		//chat_IF.onValueChanged.RemoveAllListeners();
		//chat_IF.onValueChanged.AddListener(delegate
		//{
		//	chatTextCounts.text = chat_IF.text.Length.ToString() + "/200";

		//	//chat_IF.textComponent.GetComponent<ArabicFixer>().isInputFieldFix = true;
		//	//chat_IF.textComponent.GetComponent<ArabicFixer>().fixedText = chat_IF.text;
		//	//chat_IF.text = ArabicSupport.ArabicFixer.Fix(chat_IF.text);
		//	//chat_IF.textComponent.GetComponent<ArabicFixer>().FixTextForUI();

		//});
		//chat_IF.onEndEdit.AddListener(delegate
		//{
		//	if (string.IsNullOrEmpty(chat_IF.text))
		//		chat_IF.text = "";

		//	switch (chat_IF.CheckIfEnglish(1))
		//	{
		//		case TypedLanguage.English:
		//			break;

		//		case TypedLanguage.Arabic:
		//			ChatText = chat_IF.text;
		//			//chat_IF.textComponent.GetComponent<ArabicFixer>().isInputFieldFix = true;
		//			chat_IF.text = ArabicSupport.ArabicFixer.Fix(chat_IF.text);
		//			//chat_IF.text = chat_IF.text.CustomReverse();
		//			break;

		//		case TypedLanguage.Other:
		//			break;
		//	}


		//});

		//chat_IF_TMP.onValueChanged.RemoveAllListeners();
		//chat_IF_TMP.onValueChanged.AddListener(delegate
		//{
		//	chatTextCounts.text = chat_IF_TMP.text.Length.ToString() + "/200";
		//	textToShow.text = chat_IF_TMP.text;

		//	switch (chat_IF_TMP.CheckIfEnglish(1))
		//	{
		//		case TypedLanguage.English:
		//			break;

		//		case TypedLanguage.Arabic:
		//		case TypedLanguage.Other:
		//			textToShow.text = ArabicFixer.Fix(textToShow.text);
		//			break;
		//	}

		//	//chat_IF.textComponent.GetComponent<ArabicFixer>().isInputFieldFix = true;
		//	//chat_IF.textComponent.GetComponent<ArabicFixer>().fixedText = chat_IF.text;
		//	//chat_IF.text = ArabicSupport.ArabicFixer.Fix(chat_IF.text);
		//	//chat_IF.textComponent.GetComponent<ArabicFixer>().FixTextForUI();

		//});

		chat_IF.onValueChanged.RemoveAllListeners();
		chat_IF.onValueChanged.AddListener(delegate
		{
			chatTextCounts.text = chat_IF.text.Length.ToString() + "/200";
			textToShow.text = chat_IF.text;

			switch (chat_IF.CheckIfEnglish(1))
			{
				case TypedLanguage.English:
					break;

				case TypedLanguage.Arabic:
				case TypedLanguage.Other:
					textToShow.text = ArabicFixer.Fix(textToShow.text);
					break;
			}
		});

		//chat_IF_TMP.onDeselect.AddListener(delegate
		//{
		//	return;
		//	if (string.IsNullOrEmpty(chat_IF_TMP.text))
		//		chat_IF_TMP.text = "";

		//	switch (chat_IF_TMP.CheckIfEnglish(1))
		//	{
		//		case TypedLanguage.English:
		//			break;

		//		case TypedLanguage.Arabic:
		//			ChatText = chat_IF_TMP.text;
		//			//chat_IF.textComponent.GetComponent<ArabicFixer>().isInputFieldFix = true;
		//			chat_IF_TMP.text = ArabicSupport.ArabicFixer.Fix(chat_IF_TMP.text);
		//			//chat_IF.text = chat_IF.text.CustomReverse();
		//			break;

		//		case TypedLanguage.Other:
		//			break;
		//	}
		//});

		OnIAPDone += OnPurchasingDone;
		foreach (Text gemText in GemCountText) gemText.text = PlayerDataHolder._GemCount.ToString();
		foreach (Button iapButton in IAP_Buttons)
		{
			iapButton.onClick.RemoveAllListeners();
			iapButton.onClick.AddListener(() => HelperUtil.ShowLoader());
		}
		foreach (Button micButton in opponentsMicButton) micButton.image.sprite = micButtonImages[0];
    }


	public void OnPointerClickEventForChatPopup()
	{
		if (!string.IsNullOrEmpty(ChatText) && EventSystem.current.currentSelectedGameObject != chat_IF/*chat_IF_TMP*/)
		{
			chat_IF/*chat_IF_TMP*/.text = ChatText;
		}
	}

	string _ChatText = "";
	public string ChatText
    {
		get => _ChatText;
		set => _ChatText = value;
    }

#endregion Unity Callbacks

#region Other Callbacks

public void ShowWinnerScreen()
	{
		EnableGameObject(winnerPopUp);
	}

	public void OnClickWinnerPopButton(string buttonEvent)
	{
		switch (buttonEvent)
		{
			case "Replay":
				EnableGameObject(letsPlayScreen);
				DisableGameObject(winnerPopUp);
				break;
			case "Find":
				EnableGameObject(findingPlayersPopUp);
				DisableGameObject(letsPlayScreen);
				DisableGameObject(winnerPopUp);
				HelperUtil.CallAfterDelay(() =>
				{
					DisableGameObject(findingPlayersPopUp);
					DisableGameObject(muteMicPopUp);
					DisableGameObject(prizePopUp);
					DisableGameObject(winnerPopUp);
					DisableGameObject(searchingOpponentsPopUp);
				}, 5);
				break;
			case "Home":

				HelperUtil.ShowLoading();
				HelperUtil.instance.vivoxManager.DisconnectAllChannels();
				HelperUtil.CallAfterDelay(() => HelperUtil.LoadScene(SceneName.UI), 2);
				break;
		}
	}

	public void EnableGameObject(GameObject objectToEnable)
	{
		objectToEnable.SetActive(true);
	}
	public void DisableGameObject(GameObject objectToEnable)
	{
		objectToEnable.SetActive(false);
	}

	public void MuteOwnMic()
	{
		switch (GameState._IsMuted)
		{
			case true:
				micButton.image.sprite = micButtonImages[1];
				break;
			case false:
				EnableGameObject(muteMicPopUp);
				micButton.image.sprite = micButtonImages[0];
				GameState._IsMuted = !GameState._IsMuted;
				//HelperUtil.instance.vivoxManager.MuteOwnMic(GameState._IsMuted);
				break;
		}
	}

	public void MuteOpponentMic(Image imageToCheck)
	{
		if (imageToCheck.sprite == micButtonImages[0])
		{
			imageToCheck.sprite = micButtonImages[1];
		}
		else imageToCheck.sprite = micButtonImages[0];
	}

	#endregion Other Callbacks

	#region IAP calling functions

	public void OnPurchaseComplete(UnityEngine.Purchasing.Product product)
	{

		IAPManager.instance.OnPurchaseComplete(product);
	}

	public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, UnityEngine.Purchasing.PurchaseFailureReason reason)
	{
		IAPManager.instance.OnPurchaseFailed(product, reason);
	}

	public void OnPurchasingDone()
	{
		foreach(Text gemText in GemCountText) gemText.text = PlayerDataHolder._GemCount.ToString();
		HelperUtil.HideLoading();
		HelperUtil.HideLoader();
	}

	#endregion IAP calling functions

	public static void ResetMembers()
    {
		OnIAPDone = null;
    }
}