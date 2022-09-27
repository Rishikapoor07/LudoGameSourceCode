using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperHelper : MonoBehaviour
{
    public static DeveloperHelper instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Test_2(GameObject objectToActivate)
    {
		if (objectToActivate != null)
		{
            objectToActivate.SetActive(objectToActivate.activeInHierarchy ? false : true);
		}
    }

    public void Test_3()
    {
        HelperUtil.ShowLoader();
        //Database db = new Database();
        //Dictionary<string, object> userData = new Dictionary<string, object>()
        //{
        //    {StaticKeywords.UserDataKeyWords.coins,0 }
        //};
        //db.UpdateUserData(userData, UIManager.OnGetProfile);
        //HelperUtil.ShowRewardedAdPopUp(new MessageActionData("Watch", () => GoogleAdManager.instance.ShowRewardedAd()), new MessageActionData("Cancel", null));
    }
}
