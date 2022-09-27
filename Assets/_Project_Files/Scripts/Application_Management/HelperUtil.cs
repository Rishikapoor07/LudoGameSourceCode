using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.Util;

public class HelperUtil : MonoBehaviour
{
    public static HelperUtil instance { get; private set; }

    [SerializeField] private Deftsoft.GameLibrary _gameLibrary;
    public static Deftsoft.GameLibrary GameLibrary => instance._gameLibrary;
    public VivoxManager vivoxManager;
    //Variables
    private GameObject raycastMaskGameobject;
    private GameObject raycastMaskRef;

    [Header("Message pop up related")]
    [SerializeField] GameObject messagePopupPrefab;
    [HideInInspector] public GameObject messagePopup;
    [SerializeField] GameObject messagePopupPrefabNoButton;
    [HideInInspector] public GameObject messagePopupNoButton;

    [Header("Loading screen related")]
    [SerializeField] GameObject loadingScreenPrefab;
    [HideInInspector] public GameObject loadingScreen;

    [Header("Internet pop up related")]
    [SerializeField] GameObject internetPopUpPrefab;
    [HideInInspector] public GameObject internetPopUp;

    [Header("Reward Ad related")]
    [SerializeField] GameObject rewardAdPopUpPrefab;
    [HideInInspector] public GameObject rewardedAdPopUp;

    [Header("Loader")]
    [SerializeField] private GameObject loaderPrefab;
    [HideInInspector] public GameObject loaderGO;

    public delegate void OnAppPause(ApplicationStatus status);
    public static event OnAppPause onAppPauseEvent;
    public static ApplicationStatus applicationBackgroundStatus = ApplicationStatus.Maximized;

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

        Input.multiTouchEnabled = false;
    }

    private void Start()
    {
        InternetConnection.Init(this);

        InternetConnection.OnConnected += () =>
        {
            Debug.Log("Internet connected!!!");
            GameState.isInternetConnected = true;
            HidePopUp(instance.internetPopUp);
            if(!GameInfo.vivoxLogIn && PlayerDataHolder.IsLoggedIn)
                vivoxManager.LogInToVivox(PlayerDataHolder.PlayerName);
            if (GameState.CurrentScene == GameScene.Menu && GameState.needToGetProfile) UIManager.OnGetProfile();
            GameState.needToGetProfile = false;
        };

        InternetConnection.OnDisconneted += () =>
        {
            Debug.Log("Internet disconnected!!!");
            GameState.isInternetConnected = false;
            GameState.needToGetProfile = true;
            GameInfo.vivoxLogIn = false;
            HideLoading();
            HideLoader();
			ShowInternetPopUp(new MessageActionData(GameState.CurrentScene == GameScene.GamePlay ? "Leave" : "Quit", () =>
			{
				GameInfo.vivoxLogIn = false;

                if (GameState.CurrentScene == GameScene.GamePlay) LoadScene(SceneName.UI);
                else
                {
                    Application.Quit();
                }
			}),
			new MessageActionData("Retry", () =>
			{
                UIManager.OnInternetDisconnect();
				HidePopUp(instance.internetPopUp);
			}));
		};
        Input.multiTouchEnabled = false;
        //DontDestroyOnLoad(notificationCanvas);
    }

    public void OnApplicationPause(bool pause)
    {
        applicationBackgroundStatus = pause ? ApplicationStatus.Minimized : ApplicationStatus.Maximized;
        if (onAppPauseEvent != null) onAppPauseEvent(applicationBackgroundStatus);
    }

	public static void RepeativeCall(Action action, float initalCallDelay, float repeatDelay, Func<bool> cancelCondition)
    {
        action();
        instance.StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            yield return new WaitForSeconds(initalCallDelay);
            action();

            while (cancelCondition() == false)
            {
                yield return new WaitForSeconds(repeatDelay);
                action();
            }
        }
    }

    /// <summary>
    /// Helper method to start an async call.(action will be called after delay).
    /// </summary>
    public static void CallAfterDelay(Action action, float delay, Func<bool> cancelCondition = null)
    {
        float initialTime = Time.time;

        instance.StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            while (true)
            {
                //If cancel condition gets true, return control from this line.
                if (cancelCondition != null && cancelCondition()) yield break;
                //If delay is reached, break this loop.
                else if (Time.time > initialTime + delay) break;

                //Hold control for set amount of time to decrease CPU pressure.
                yield return new WaitForSeconds(0.02f);
            }

            //Execute the action if delay reached and cancel condition is still false.
            action();
        }
        
    }

    /// <summary>
    /// Helper method to start an async call.(action will be called after the condition gets true).
    /// </summary>
    public static void CallAfterCondition(Action action, Func<bool> condition, Func<bool> cancelCondition = null)
    {
        instance.StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            while (!condition())
            {
                //If cancel condition gets true, return control from this line.
                if (cancelCondition != null && cancelCondition()) yield break;

                yield return new WaitForSeconds(0.5f);
            }
            action();
        }
    }

    private static List<string> asyncCalls = new List<string>();
    /// <summary>
    /// Helper method to start an async call.(action will be called repeatedly with respect to the 'repeatTime' parameter util the cancel condition gets true.).
    /// </summary>
    public async static void InvokeRepeating(Action action, float repeatTime, Func<bool> cancelCondition = null)
    {
        while (true)
        {
            //Calling the action.
            action();

            //Break this condition if cancelCondition gets true.
            if (cancelCondition != null && cancelCondition() == true) break;

            //Code to wait for fixed period of time.
            int repeatTimeInMilliseconds = (int)((float)Math.Round((repeatTime * 1000) * 100f) / 100f);
            await Task.Delay(repeatTimeInMilliseconds);
        }
    }

    public static void ShowMessage(string message, MessageActionData firstAction = null, MessageActionData secondAction = null, bool withBackground = true, float popupHideDelayAfterButtonClick = 0.0f)
    {
        if (!instance.messagePopup)
        {
            instance.messagePopup = Instantiate(instance.messagePopupPrefab);
            DontDestroyOnLoad(instance.messagePopup);
        }
        Transform messageTextParent = instance.messagePopup.transform.GetChild(0).GetChild(0).transform;
        Transform buttonHolder = messageTextParent.transform.Find("ButtonHolder");

        //Fetching the buttons and remove existing actions if any..
        Button actionButton = buttonHolder.GetChild(0).GetComponent<Button>();
        Button secondaryActionButton = buttonHolder.GetChild(1).GetComponent<Button>();
        actionButton.onClick.RemoveAllListeners();
        secondaryActionButton.onClick.RemoveAllListeners();

        if (secondAction != null)
        {
            secondaryActionButton.gameObject.SetActive(true);

            //Adding close action to both buttons.
            actionButton.onClick.AddListener(() => { instance.messagePopup.SetActive(false); });
            secondaryActionButton.onClick.AddListener(() => { instance.messagePopup.SetActive(false); });

            //Adding additional actions, if any.
            if (firstAction != null && firstAction.action != null) actionButton.onClick.AddListnerWithDelayCall(firstAction.action, popupHideDelayAfterButtonClick);
            if (secondAction != null && secondAction.action != null) secondaryActionButton.onClick.AddListnerWithDelayCall(secondAction.action, popupHideDelayAfterButtonClick);

            //Setting button names, if any.
            if (firstAction != null && !string.IsNullOrEmpty(firstAction.buttonName)) actionButton.GetComponentInChildren<Text>().text = firstAction.buttonName;
            if (secondAction != null && !string.IsNullOrEmpty(secondAction.buttonName)) secondaryActionButton.GetComponentInChildren<Text>().text = secondAction.buttonName;
        }
        else
        {
            secondaryActionButton.gameObject.SetActive(false);
            if (firstAction != null && firstAction.action != null) actionButton.onClick.AddListnerWithDelayCall(firstAction.action, popupHideDelayAfterButtonClick);
            actionButton.onClick.AddListnerWithDelayCall(() => { instance.messagePopup.SetActive(false); }, popupHideDelayAfterButtonClick);
            if (firstAction != null) actionButton.GetComponentInChildren<Text>().text = firstAction.buttonName;
            else actionButton.GetComponentInChildren<Text>().text = "Ok";
        }


        //Setting the message.
        messageTextParent.transform.Find("MessageText").GetComponent<Text>().text = message;
        instance.messagePopup.SetActive(true);
    }

    public static void ShowMessageWithoutButton(string message, Func<bool> hideCondition = null)
    {
        //Instantiating the popup.
        if (!instance.messagePopupNoButton)
        {
            instance.messagePopupNoButton = Instantiate(instance.messagePopupPrefabNoButton);
            DontDestroyOnLoad(instance.messagePopupNoButton);
        }

        //Setting the message.
        Transform messageTextParent = instance.messagePopupNoButton.transform.GetChild(0).GetChild(0);
        messageTextParent.Find("MessageText").GetComponent<Text>().text = message;
        instance.messagePopupNoButton.SetActive(true);

        instance.StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            if (hideCondition != null) yield return new WaitUntil(() => hideCondition());

            //Execute the action if delay reached and cancel condition is still false.
            instance.messagePopupNoButton.SetActive(false);
        }
    }

    public static void ShowMessageWithoutButton(string message, float hideTime, Func<bool> cancelCondition = null, Action onHide = null, Func<bool> onHideCancelCondition = null)
    {
        //Instantiating the popup.
        if (!instance.messagePopupNoButton)
        {
            instance.messagePopupNoButton = Instantiate(instance.messagePopupPrefabNoButton);
            DontDestroyOnLoad(instance.messagePopupNoButton);
        }

        //Setting the message.
        Transform messageTextParent = instance.messagePopupNoButton.transform.GetChild(0).GetChild(0);
        messageTextParent.Find("MessageText").GetComponent<Text>().text = message;
        instance.messagePopupNoButton.SetActive(true);

        float initialTime = Time.time;
        instance.StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            while (true)
            {
                //If cancel condition gets true, return control from this line.
                if (cancelCondition != null && cancelCondition()) yield break;
                //If delay is reached, break this loop.
                else if (Time.time > initialTime + hideTime) break;

                //Hold control for set amount of time to decrease CPU pressure.
                yield return new WaitForSeconds(0.02f);
            }

            if (onHideCancelCondition != null && onHideCancelCondition()) yield break;
            if (onHide != null)
            {
                onHide();
            }

            //Execute the action if delay reached and cancel condition is still false.
            instance.messagePopupNoButton.SetActive(false);
        }
    }

    public static void HidePopUpMessage()
    {
        if (instance.messagePopup != null && instance.messagePopup.activeInHierarchy)
        {
            instance.messagePopup.SetActive(false);
        }
    }

    public static bool MessagePopUpActive()
    {
        if (instance.messagePopup != null)
        {
            return instance.messagePopup.activeInHierarchy;
        }
        return false;
    }

    public static void ShowLoading(float hideTime = 0, bool blockTouch = false)
    {
        if (!instance.loadingScreen)
        {
            instance.loadingScreen = Instantiate(instance.loadingScreenPrefab);
            DontDestroyOnLoad(instance.loadingScreen);
        }
        if (blockTouch)
        {
            instance.loadingScreen.transform.GetChild(0).transform.GetComponent<Image>().color = new Color(255, 219, 206, 0);
            instance.loadingScreen.transform.GetChild(0).GetChild(0).transform.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            instance.loadingScreen.transform.GetChild(0).GetChild(0).GetChild(0).transform.GetComponent<Text>().enabled = false;
            instance.loadingScreen.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).transform.GetComponent<Text>().enabled = false;
        }
        else
        {
            instance.loadingScreen.transform.GetChild(0).transform.GetComponent<Image>().color = new Color(255, 219, 206, 255);
            instance.loadingScreen.transform.GetChild(0).GetChild(0).transform.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            instance.loadingScreen.transform.GetChild(0).GetChild(0).GetChild(0).transform.GetComponent<Text>().enabled = true;
            instance.loadingScreen.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).transform.GetComponent<Text>().enabled = true;
        }
        instance.loadingScreen.SetActive(true);

        if (hideTime != 0) HelperUtil.CallAfterDelay(HideLoading, hideTime);
    }


    public static void HideLoading()
    {
        if (instance.loadingScreen && instance.loadingScreen.activeSelf) instance.loadingScreen.SetActive(false);
    }

    /// <summary>
    /// Method to convert seconds into "MM:SS" time format.
    /// </summary>
    public static string SecondsToTimer(int seconds)
    {
        int numberOfMinutes = seconds / 60;
        int numberOfSecods = seconds - (numberOfMinutes * 60);
        return ((numberOfMinutes < 10) ? "0" : "") + numberOfMinutes + ":" + ((numberOfSecods < 10) ? "0" : "") + numberOfSecods;
    }

    public static Dictionary<string, Texture> userProfilePicDictionary = new Dictionary<string, Texture>();
    public static void GetTextureFromURL(RawImage image, string url, Action onSuccess= null)
    {
        if (userProfilePicDictionary.ContainsKey(url))
        {
            image.texture = userProfilePicDictionary[url];
        }
        else
        {
            instance.StartCoroutine(enumerator());
            IEnumerator enumerator()
            {
                Debug.Log("Photo Url: " + url);

                string photoUrl = "";
                if (url.Contains("%252"))
                    photoUrl = url.Replace("%252", "%2");
                else
                    photoUrl = url;

                Debug.Log("Photo Url: " + photoUrl);

                UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoUrl);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError && request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log("Network Error" + request.error);
                }
                else
                {
                    Texture imageTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    if (!userProfilePicDictionary.ContainsKey(url))
                    {
                        userProfilePicDictionary.Add(url, imageTexture);
                    }
                    if (image) image.texture = imageTexture;

                    //Call OnSuccess callback if any.
                    onSuccess?.Invoke();
                }
            }
        }
    }

    public static void SetTexture(string url, Action<Texture> textureSetCallback)
    {
        //If the texture already exist in the database.
        if (PlayerPrefs.HasKey(url))
        {
            try
            {
                byte[] textureData = File.ReadAllBytes(PlayerPrefs.GetString(url));
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(textureData);
                textureSetCallback(tex);
            }
            catch
            {
                DownloadTexture();
            }
        }
        else
        {
            DownloadTexture();
        }

        void DownloadTexture()
        {
            instance.StartCoroutine(enumerator());
            IEnumerator enumerator()
            {
                string photoUrl = "";
                if (url.Contains("%252"))
                    photoUrl = url.Replace("%252", "%2");
                else
                    photoUrl = url;

                Debug.Log("Photo Url: " + photoUrl);

                UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoUrl);

                if (url.Contains("https"))
                {
                    var cert = new BypassCertificate();
                    request.certificateHandler = cert;
                }
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.ProtocolError && request.result != UnityWebRequest.Result.ConnectionError)
                {
                    Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                    //Saving the file into the local storage.
                    string filePath = Path.Combine(Application.persistentDataPath, "Image_" + UnityEngine.Random.Range(0, 999999999) + ".png");
                    File.WriteAllBytes(filePath, downloadedTexture.EncodeToPNG());
                    PlayerPrefs.SetString(url, filePath);
                    textureSetCallback(downloadedTexture);
                }
            }
        }
    }

    public static void HideMessage()
    {
        if (instance.messagePopup != null && instance.messagePopup.activeInHierarchy)
        {
            instance.messagePopup.SetActive(false);
        }
    }

    public static void HideMessageWithoutButton()
    {
        if (instance.messagePopupNoButton != null && instance.messagePopupNoButton.activeInHierarchy)
        {
            instance.messagePopupNoButton.SetActive(false);
        }
    }

    /// <summary>
    /// Method to restrict UI actions for set period of time.
    /// </summary>
    public async static void EnableRaycastMask(float hideTime = 0)
    {
        if (!instance.raycastMaskRef)
        {
            instance.raycastMaskRef = Instantiate(instance.raycastMaskGameobject);
            DontDestroyOnLoad(instance.raycastMaskRef);
        }
        instance.raycastMaskRef.SetActive(true);

        if (hideTime != 0)
        {
            int timeToWait = (int)(hideTime * 1000);
            await Task.Delay(timeToWait);
            DisableRaycastMask();
        }
    }

    public static void DisableRaycastMask()
    {
        if (instance.raycastMaskRef)
        {
            instance.raycastMaskRef.SetActive(false);
        }
    }

    public static void SetUserPicture(RawImage image, string url)
    {
        Debug.Log("Photo Url: " + url);
        string photoUrl="";
        if (url.Contains("%252"))
            photoUrl = url.Replace("%252", "%2");
        else
            photoUrl = url;

        Debug.Log("Photo Url: " + photoUrl);
            instance.StartCoroutine(enumerator());
        IEnumerator enumerator()
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoUrl);
            yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
                    Debug.Log("Network Error" + request.error);
            else
            {
                Texture imageTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                image.texture = imageTexture;
            }
        }
    }

    public static GameObject Instantiate(string path, Transform theTransform = null, Vector3? thePosition = null) 
    {
        try
        {
            GameObject objectToReturn = GameObject.Instantiate(Resources.Load(path) as GameObject);
            if (thePosition != null)
            {
                Vector3 newPosition = (Vector3)thePosition;
                objectToReturn.transform.position = newPosition;
            }
            objectToReturn.transform.SetParent(theTransform);
            return objectToReturn;
        }
        catch
        {
            return null;
        }

    }

    public static void LoadScene(string sceneName, Action onLoad = null) 
    {
        SceneManager.LoadScene(sceneName);
        if (onLoad == null) return;
        void onChangeMethod(Scene currentScene, Scene nextScene) 
        {
            onLoad?.Invoke();
            SceneManager.activeSceneChanged -= onChangeMethod;
        }

        SceneManager.activeSceneChanged += onChangeMethod;
    }

    //public static void LoadSceneOnNetwork(string sceneName, Action onLoad = null)
    //{
    //    Photon.Pun.PhotonNetwork.LoadLevel(sceneName);

    //    void onChangeMethod(Scene currentScene, Scene nextScene)
    //    {
    //        onLoad?.Invoke();
    //        SceneManager.activeSceneChanged -= onChangeMethod;
    //    }

    //    SceneManager.activeSceneChanged += onChangeMethod;
    //}

    public static Color HexToRGB(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    Coroutine singleTextTimerHolder = null;
    public static void CreateTimerSingleText(Text timerText, int TotalTimer, Action OnTimerEnd = null)
    {
        if (instance.singleTextTimerHolder != null) instance.StopCoroutine(instance.singleTextTimerHolder);

        instance.singleTextTimerHolder = instance.StartCoroutine(startTimersingleText());
        IEnumerator startTimersingleText()
        {
            do
            {
                float minutes = Mathf.FloorToInt(TotalTimer / 60);
                float seconds = TotalTimer % 60;
                string currentTime = string.Format("{00:00}{1:00}", minutes, seconds);
                timerText.text = currentTime[0].ToString() + " " + currentTime[1].ToString() + " : " + currentTime[2].ToString() + "  " + currentTime[3].ToString();
                TotalTimer--;
                yield return new WaitForSecondsRealtime(1f);
                if (TotalTimer == 0)
                {
                    instance.StopCoroutine(startTimersingleText());
                    if (OnTimerEnd != null) OnTimerEnd();
                }
            } while (TotalTimer >= 0);

        }
    }

    public static void LoadInitScene(Action extraAction = null, bool destroyRequiredComponents = true)
    {
        //Photon.Pun.PhotonNetwork.Disconnect();

        if (destroyRequiredComponents)
        {
            //Destroy all the unwanted components on load, if required.
        }

        CallAfterDelay(() => SceneManager.LoadScene(SceneName.Init), Time.deltaTime);
        HideMessage();
        extraAction?.Invoke();
    }

    /// <summary>
    /// Check if all the fields are non Empty fields
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public static bool CheckEmpty(InputField inputField, GameObject errorMessageGO)
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            errorMessageGO.SetActive(true);
            return true;
        }
        else
        {
            errorMessageGO.SetActive(false);
            return false;
        }
    }

    /// <summary>
    /// Check if the Name Length is atleast 4 Characters Long
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public static bool CheckMinLength(InputField inputField, int minLength, Action minAction = null, Action normalAction = null)
    {
        if (inputField.text.Length < minLength)
        {
            if (minAction != null) minAction.Invoke();
            return true;
        }
        if (normalAction != null) normalAction.Invoke();
        return false;
    }

    public static bool CheckMinLength(TMPro.TMP_InputField inputField, int minLength, Action minAction = null, Action normalAction = null)
    {
        if (inputField.text.Length < minLength)
        {
            if (minAction != null) minAction.Invoke();
            return true;
        }
        if (normalAction != null) normalAction.Invoke();
        return false;
    }

    public static Texture2D pickedImage;
    public static byte[] pickedImageBytes;
    /// <summary>
    /// To pick image from gallery
    /// </summary>
    /// <param name="maxSize">max size of image</param>
    public static void PickImage(int maxSize, Action<Texture2D> actionToPerform, Action OnCancel)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                pickedImage = NativeGallery.LoadImageAtPath(path, maxSize);
                pickedImageBytes = File.ReadAllBytes(path);
                if (pickedImage == null)
                {
                    pickedImageBytes = null;
                    return;
                }
                actionToPerform(pickedImage);
            }
            else
            {
                if (OnCancel != null)
                {
                    OnCancel();
                }
            }
        }, "Select an image", "image/*");
    }

    public static void ShowInternetPopUp(MessageActionData firstButton = null, MessageActionData secondButton = null, float popupHideDelayAfterButtonClick = 0.0f)
	{
        if (!instance.internetPopUp)
        {
            instance.internetPopUp = Instantiate(instance.internetPopUpPrefab);
            DontDestroyOnLoad(instance.internetPopUp);
        }
        Transform buttonHolder = instance.internetPopUp.transform.GetChild(0).GetChild(0).GetChild(2).transform;
        //Fetching the buttons and remove existing actions if any..
        Button actionButton = buttonHolder.transform.GetChild(0).GetComponent<Button>();
        Button secondaryActionButton = buttonHolder.transform.GetChild(1).GetComponent<Button>();
        actionButton.onClick.RemoveAllListeners();
        secondaryActionButton.onClick.RemoveAllListeners();

        if (secondButton != null)
        {
            secondaryActionButton.gameObject.SetActive(true);

            //Adding close action to both buttons.
            actionButton.onClick.AddListener(() => { instance.internetPopUp.SetActive(false); });
            secondaryActionButton.onClick.AddListener(() => { instance.internetPopUp.SetActive(false); });

            //Adding additional actions, if any.
            if (firstButton != null && firstButton.action != null) actionButton.onClick.AddListnerWithDelayCall(firstButton.action, popupHideDelayAfterButtonClick);
            if (secondButton != null && secondButton.action != null) secondaryActionButton.onClick.AddListnerWithDelayCall(secondButton.action, popupHideDelayAfterButtonClick);

            ////Setting button names, if any.
            //if (firstButton != null && !string.IsNullOrEmpty(firstButton.buttonName)) actionButton.GetComponentInChildren<Text>().text = firstButton.buttonName;
            //if (secondButton != null && !string.IsNullOrEmpty(secondButton.buttonName)) secondaryActionButton.GetComponentInChildren<Text>().text = secondButton.buttonName;
        }
        else
        {
            secondaryActionButton.gameObject.SetActive(false);
            if (firstButton != null && firstButton.action != null) actionButton.onClick.AddListnerWithDelayCall(firstButton.action, popupHideDelayAfterButtonClick);
            actionButton.onClick.AddListnerWithDelayCall(() => { instance.internetPopUp.SetActive(false); InternetConnection.InternetCheckIterativeBlock(); }, popupHideDelayAfterButtonClick);
            //if (firstButton != null) actionButton.GetComponentInChildren<Text>().text = firstButton.buttonName;
            //else actionButton.GetComponentInChildren<Text>().text = "Ok";
        }

        instance.internetPopUp.SetActive(true);
    }

    public static void HidePopUp(GameObject popUpToHide)
    {
        if (popUpToHide != null && popUpToHide.activeInHierarchy) popUpToHide.SetActive(false);
    }

    public static void ShowRewardedAdPopUp(MessageActionData watchButton, MessageActionData cancelButton)
    {
        if (!instance.rewardedAdPopUp)
        {
            instance.rewardedAdPopUp = Instantiate(instance.rewardAdPopUpPrefab);
            DontDestroyOnLoad(instance.rewardedAdPopUp);
        }

        //Fetching the buttons and remove existing actions if any..
        Button actionButton = instance.rewardedAdPopUp.transform.GetChild(0).GetChild(1).GetChild(2).transform.GetComponent<Button>();
        Button secondaryActionButton = instance.rewardedAdPopUp.transform.GetChild(0).GetChild(1).GetChild(3).transform.GetComponent<Button>();
        actionButton.onClick.RemoveAllListeners();
        secondaryActionButton.onClick.RemoveAllListeners();

        //actionButton.GetComponentInChildren<Text>().text = watchButton.buttonName;
        //secondaryActionButton.GetComponentInChildren<Text>().text = cancelButton.buttonName;

        //Adding close action to both buttons.
        actionButton.onClick.AddListener(() => HidePopUp(instance.rewardedAdPopUp));
        secondaryActionButton.onClick.AddListener(() => HidePopUp(instance.rewardedAdPopUp));

        //Adding additional actions to both buttons
        actionButton.onClick.AddListener(watchButton.action);
        secondaryActionButton.onClick.AddListener(cancelButton.action);

        instance.rewardedAdPopUp.SetActive(true);
    }

    public static void ShowLoader()
    {
        if (!instance.loaderGO)
        {
            instance.loaderGO = GameObject.Instantiate(instance.loaderPrefab);
            GameObject.DontDestroyOnLoad(instance.loaderGO);
        }
        instance.loaderGO.SetActive(true);

        CallAfterDelay(() =>
        {
            HideLoader();
        }, 120);
    }

    public static void HideLoader()
    {
        if (instance.loaderGO && instance.loaderGO.activeSelf) instance.loaderGO.SetActive(false);
    }

    #region Later use

    //[SerializeField] GameObject micAccessPopUpPrefab;
    //[HideInInspector] public GameObject micAccessPopUp;



    //[Header("Notification Canvas References")]
    //[SerializeField] private GameObject notificationCanvas;
    //[SerializeField] private Text notificationText;
    //[SerializeField] private Button notificationButton_1;
    //[SerializeField] private Button notificationButton_2;

    //public void ShowNotification(string message, MessageActionData button_1, MessageActionData button_2, float hideTime = 0f)
    //{
    //    //Remove existing actions from the buttons, if any.
    //    notificationButton_1.onClick.RemoveAllListeners();
    //    notificationButton_2.onClick.RemoveAllListeners();

    //    //Adding the actions to the buttons.
    //    if (button_1 != null)
    //    {
    //        notificationButton_1.onClick.AddListener(button_1.action);
    //        notificationButton_1.onClick.AddListener(() => instance.notificationCanvas.SetActive(false));
    //        notificationButton_1.GetComponentInChildren<Text>().text = button_1.buttonName;
    //        notificationButton_1.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        notificationButton_1.gameObject.SetActive(false);
    //    }
    //    if (button_2 != null)
    //    {
    //        notificationButton_2.onClick.AddListener(button_2.action);
    //        notificationButton_2.onClick.AddListener(() => instance.notificationCanvas.SetActive(false));
    //        notificationButton_2.GetComponentInChildren<Text>().text = button_2.buttonName;
    //        notificationButton_2.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        notificationButton_2.gameObject.SetActive(false);
    //    }

    //    //Setting the message.
    //    notificationText.text = message;
    //    notificationCanvas.SetActive(true);

    //    //Hiding the notification popup after the set time, if any.
    //    if (hideTime > 0) CallAfterDelay(() => instance.notificationCanvas.SetActive(false), hideTime);
    //}

    //public void HideNotification()
    //{
    //    instance.notificationCanvas.SetActive(false);
    //}    

    //public static void ShowMicAccessPopUp(MessageActionData firstAction = null, MessageActionData secondAction = null, float popupHideDelayAfterButtonClick = 0.0f)
    //{
    //    if (!instance.micAccessPopUp)
    //    {
    //        instance.micAccessPopUp = Instantiate(instance.micAccessPopUpPrefab);
    //        DontDestroyOnLoad(instance.micAccessPopUp);
    //    }
    //    Transform buttonHolder = instance.micAccessPopUp.transform.Find("ButtonHolder");

    //    //Fetching the buttons and remove existing actions if any..
    //    Button actionButton = buttonHolder.transform.Find("Button_1").GetComponent<Button>();
    //    Button secondaryActionButton = buttonHolder.transform.Find("Button_2").GetComponent<Button>();
    //    actionButton.onClick.RemoveAllListeners();
    //    secondaryActionButton.onClick.RemoveAllListeners();

    //    if (secondAction != null)
    //    {
    //        secondaryActionButton.gameObject.SetActive(true);

    //        //Adding close action to both buttons.
    //        actionButton.onClick.AddListener(() => { instance.micAccessPopUp.SetActive(false); });
    //        secondaryActionButton.onClick.AddListener(() => { instance.micAccessPopUp.SetActive(false); });

    //        //Adding additional actions, if any.
    //        if (firstAction != null && firstAction.action != null) actionButton.onClick.AddListnerWithDelayCall(firstAction.action, popupHideDelayAfterButtonClick);
    //        if (secondAction != null && secondAction.action != null) secondaryActionButton.onClick.AddListnerWithDelayCall(secondAction.action, popupHideDelayAfterButtonClick);

    //        //Setting button names, if any.
    //        if (firstAction != null && !string.IsNullOrEmpty(firstAction.buttonName)) actionButton.GetComponentInChildren<Text>().text = firstAction.buttonName;
    //        if (secondAction != null && !string.IsNullOrEmpty(secondAction.buttonName)) secondaryActionButton.GetComponentInChildren<Text>().text = secondAction.buttonName;
    //    }
    //    else
    //    {
    //        secondaryActionButton.gameObject.SetActive(false);
    //        if (firstAction != null && firstAction.action != null) actionButton.onClick.AddListnerWithDelayCall(firstAction.action, popupHideDelayAfterButtonClick);
    //        actionButton.onClick.AddListnerWithDelayCall(() => { instance.micAccessPopUp.SetActive(false); }, popupHideDelayAfterButtonClick);
    //        if (firstAction != null) actionButton.GetComponentInChildren<Text>().text = firstAction.buttonName;
    //        else actionButton.GetComponentInChildren<Text>().text = "Ok";
    //    }

    //    instance.micAccessPopUp.SetActive(true);
    //}

    #endregion Later use
}

public class MessageActionData
{
    public string buttonName;
    public UnityEngine.Events.UnityAction action;

    public bool InUse
    {
        get
        {
            return !string.IsNullOrEmpty(buttonName) || action != null;
        }
    }

    public MessageActionData(string newStringName, UnityEngine.Events.UnityAction newAction)
    {
        buttonName = newStringName;
        action = newAction;
    }

}


class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}


public static class InternetConnection
{
    //Settings
    public static float refreshRateInSeconds = 2f;

    public static event System.Action OnConnected;
    public static event System.Action OnDisconneted;

    //Other fields.
    private static bool isConnected;
    private static MonoBehaviour monoBehaviourInstance;

    public static void Init(MonoBehaviour theMonoBehaviour)
    {
        monoBehaviourInstance = theMonoBehaviour;
        InternetCheckIterativeBlock();
    }

    public static void InternetCheckIterativeBlock()
    {
        monoBehaviourInstance.StartCoroutine(CheckInternetConnection());
        IEnumerator CheckInternetConnection()
        {
            while (monoBehaviourInstance != null)
            {
                bool lastConnectionStatus = isConnected;
                //Debug.Log("Connection Status::: " + lastConnectionStatus);
                const string echoServer = "https://google.com/";
                using (var request = UnityEngine.Networking.UnityWebRequest.Head(echoServer))
                {
                    request.timeout = 5;
                    yield return request.SendWebRequest();
                    isConnected = request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError && request.responseCode == 200;

                    if (isConnected != lastConnectionStatus)
                    {
                        if (isConnected) 
                            OnConnected.Invoke();
                        else 
                            OnDisconneted.Invoke();
                    }
                }
                //yield return new WaitForSeconds(refreshRateInSeconds);
                yield return new WaitForSecondsRealtime(refreshRateInSeconds);
            }
        }
    }
}

public static class HelperDebug 
{
    public static void Firebase_Log(object message)
    {
        Debug.Log("<color=orange>Info_Log: " + message + "</color>");
    }

    public static void Info_Log(object message)
    {
        Debug.Log("<color=green>Info_Log: " + message + "</color>");
    }

    public static void Error_Log(string message)
    {
        Debug.Log("<color=yellow>Error_Log: " + message + "</color>");
    }

    public static void Photon_Log(string message)
    {
        Debug.Log("<color=pink>Photon_Log: " + message + "</color>");
    }
}