using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    public static IAPManager instance;

	private void Start()
	{
        instance = this;
        DontDestroyOnLoad(instance);
	}

	#region PRIVATE VARIABLE
	// Coins Product ID......
	private string coinsPack1 = "coinPack1";
    private string coinsPack2 = "coinPack2";
    private string coinsPack3 = "coinPack3";
    private string coinsPack4 = "coinPack4";

    // Gems Product ID.......
    private string gemsPack1 = "diamondPack1";
    private string gemsPack2 = "diamondPack2";
    private string gemsPack3 = "diamondPack3";
    private string gemsPack4 = "diamondPack4";

    #endregion

    public const string currencyTypeToUpdate = "inApp";
    public const string coin = "coin";
    public const string gem = "gem";

    public void OnPurchaseComplete(Product product)
    {
        //BUYING COINS
        if (product.definition.id == coinsPack1)
        {
            PlayerDataHolder._CoinCount += 5000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, coin, FirebaseAuthentication.TypesOfCurrency.pack1, onCompletion: onPurchaseCompleteFirebase);
        }
        else if(product.definition.id == coinsPack2)
        {
            PlayerDataHolder._CoinCount += 10000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, coin, FirebaseAuthentication.TypesOfCurrency.pack2, onCompletion: onPurchaseCompleteFirebase);
        }
        else if (product.definition.id == coinsPack3)
        {
            PlayerDataHolder._CoinCount += 15000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, coin, FirebaseAuthentication.TypesOfCurrency.pack3, onCompletion: onPurchaseCompleteFirebase);
        }
        else if (product.definition.id == coinsPack4)
        {
            PlayerDataHolder._CoinCount += 20000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, coin, FirebaseAuthentication.TypesOfCurrency.pack4, onCompletion: onPurchaseCompleteFirebase);
        }

        //BUYING GEMS 
        else if (product.definition.id == gemsPack1)
        {
            PlayerDataHolder._GemCount += 5000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, gem, FirebaseAuthentication.TypesOfCurrency.pack1, onCompletion: onPurchaseCompleteFirebase);
            Debug.Log("Give Player " + PlayerDataHolder._GemCount + " Gems");
        }
        else if (product.definition.id == gemsPack2)
        {
            PlayerDataHolder._GemCount += 10000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, gem, FirebaseAuthentication.TypesOfCurrency.pack2, onCompletion: onPurchaseCompleteFirebase);
            Debug.Log("Give Player " + PlayerDataHolder._GemCount + " Gems");
        }
        else if (product.definition.id == gemsPack3)
        {
            PlayerDataHolder._GemCount += 15000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, gem, FirebaseAuthentication.TypesOfCurrency.pack3, onCompletion: onPurchaseCompleteFirebase);
            Debug.Log("Give Player " + PlayerDataHolder._GemCount + " Gems");
        }
        else if (product.definition.id == gemsPack4)
        {
            PlayerDataHolder._GemCount += 20000;
            FirebaseAuthentication.instance.UpdateCurrency(currencyTypeToUpdate, gem, FirebaseAuthentication.TypesOfCurrency.pack4, onCompletion: onPurchaseCompleteFirebase);
            Debug.Log("Give Player " + PlayerDataHolder._GemCount + " Gems");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of " + product.definition.id + " failed due to " + reason);
        HelperUtil.HideLoading();
        HelperUtil.HideLoader();
    }

    private static void onPurchaseCompleteFirebase()
    {
        Debug.Log("Purchasing complete!!!");
        Database.listner = null;
        Database.OnSuccess = null;
        Database.OnSuccess = (_userData) =>
        {
            if (_userData.ContainsKey(StaticKeywords.UserDataKeyWords.coins)) PlayerDataHolder._CoinCount = int.Parse(_userData[StaticKeywords.UserDataKeyWords.coins].ToString());
            if (_userData.ContainsKey(StaticKeywords.UserDataKeyWords.gems)) PlayerDataHolder._GemCount = int.Parse(_userData[StaticKeywords.UserDataKeyWords.gems].ToString());
            if (GameState.CurrentScene == GameScene.Menu) UIManager.OnIAPDone();
            else if (GameState.CurrentScene == GameScene.GamePlay) GameManager.OnIAPDone();
        };
        Database db = new Database();
        db.OnDatabseDocUpdate();
    }
}
