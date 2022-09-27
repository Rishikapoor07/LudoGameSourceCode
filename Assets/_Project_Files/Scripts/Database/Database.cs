using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;
using System;
using System.Threading.Tasks;
//using UnityEditor.Localization.Plugins.XLIFF.V20;

public class Database
{
    FirebaseFirestore firestoredatabase;
    public static bool needToUpdateProfile;
    public QuerySnapshot iapItemDoc;

    public Database()
    {
        firestoredatabase = FirebaseFirestore.DefaultInstance;
    }

    public void WriteNewUser(UserDataHolder userDataHolder, bool shouldSync, Action OnWriteUserSucces = null)
    {
        Action tempAction = OnWriteUserSucces;
        if(OnWriteUserSucces != null) OnWriteUserSucces = null;
        if (!string.IsNullOrEmpty(userDataHolder.userData[StaticKeywords.UserDataKeyWords.userName].ToString()) && userDataHolder.userData[StaticKeywords.UserDataKeyWords.accountType].ToString() == StaticKeywords.AuthProvider.guest)
        {
            string userName = FirebaseAuthentication.instance.guestAuth.GetGuestName(FirebaseAuthentication.instance.guestNamesJson.text);
            userDataHolder.userData[StaticKeywords.UserDataKeyWords.userName] = userName;
        }
        OnWriteUserSucces = tempAction;
        FindOrWriteUser(OnWriteUserSucces, shouldSync, userDataHolder);
    }

    public void ReadUserData(Action<Dictionary<string, object>> OnGetProfileSuccess, Action OnGetProfileFailed)
    {
        DocumentReference docRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).Document(PlayerDataHolder.UserId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (task.IsCompleted)
            {
                if (snapshot.Exists)
                {
                    OnGetProfileSuccess(snapshot.ToDictionary());
                }
                else
                {
                    Debug.Log(string.Format("Document {0} does not exist!", snapshot.Id));
                    OnGetProfileFailed();
                }
            }
            else if (task.IsFaulted) Debug.Log(task.Exception.ToString());
        });
    }

    public void UpdateUserData(Dictionary<string, object> updatedData, Action SuccessCallback = null)
    {
        DocumentReference docRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).Document(PlayerDataHolder.UserId);
        docRef.UpdateAsync(updatedData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("User data canceled");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("User data updated");
                if (SuccessCallback != null) SuccessCallback();
            }
            else if (task.IsFaulted)
            {
                Debug.Log("User data failed");
            }
        });
    }

    void IsDocExists(DocumentReference docRef, Action<bool> isExists)
    {
        docRef.GetSnapshotAsync(Source.Server).ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot doc = task.Result;
            if (doc.Exists)
            {
                isExists(true);
            }
            else
            {
                isExists(false);
            }
        });
    }

    public void AddCollectables(int countToAdd, string collectableKey, Action OnCollectableAddedSuccess)
    {
        DocumentReference docRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).Document(PlayerDataHolder.UserId);
        firestoredatabase.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(docRef).ContinueWithOnMainThread((task) =>
            {
                long newCoins = task.Result.GetValue<long>(collectableKey) + countToAdd;
                Dictionary<string, object> updates = new Dictionary<string, object>{
          { collectableKey, newCoins}
        };
                transaction.Update(docRef, updates);
                return true;
            });
        }).ContinueWithOnMainThread((transactionResultTask) =>
        {
            if (transactionResultTask.Result)
            {
                Debug.Log("Collectable updated successfully.");
                OnCollectableAddedSuccess();
            }
        });
    }

    public void DeductCurrency(int countToRemove, string collectableKey, Action OnCollectableRemoveSuccess, Action OnFailed = null)
    {
        DocumentReference docRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).Document(PlayerDataHolder.UserId);
        firestoredatabase.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(docRef).ContinueWithOnMainThread((task) =>
            {
                long count = task.Result.GetValue<long>(collectableKey);
                if (count >= countToRemove)
                {
                    count = count - countToRemove;
                    Dictionary<string, object> updates = new Dictionary<string, object>{
            { collectableKey, count}
          };
                    transaction.Update(docRef, updates);
                    return true;
                }
                return false;
            });
        }).ContinueWithOnMainThread((transactionResultTask) =>
        {
            if (transactionResultTask.Result)
            {
                Debug.Log("Collectable updated successfully.");
                OnCollectableRemoveSuccess();
            }
            else
            {
                Debug.Log("Not enough coin");
                if (OnFailed != null) OnFailed();
            }
        });
    }

    public void DeleteUser(string userId, Action OnDeleteDataSuccess)
    {
        DocumentReference docRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).Document(userId);
        docRef.DeleteAsync().ContinueWithOnMainThread((transactionResultTask) =>
        {
            if (transactionResultTask.IsCompleted)
            {
                Debug.Log("Account Deleted");
                OnDeleteDataSuccess();
            }
        });
    }

    public void FindOrWriteUser(Action OnUserFound, bool shouldSync, UserDataHolder userDataHolder)
    {
        if (string.IsNullOrEmpty(PlayerDataHolder.SocialId))
        {
            Debug.Log("Empty Social id!!!");
            if (GameState.NeedToSync)
            {
                Debug.Log("Need to sync profile!!!");
                Dictionary<string, object> updatedData = new Dictionary<string, object>()
                {
                    {StaticKeywords.UserDataKeyWords.userName, FirebaseAuthentication.instance.auth.CurrentUser.DisplayName},
                    {StaticKeywords.UserDataKeyWords.profileImageUrl, FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl.ToString() },
                    {StaticKeywords.UserDataKeyWords.accountType, userDataHolder.userData[StaticKeywords.UserDataKeyWords.accountType].ToString() },
                    {StaticKeywords.UserDataKeyWords.socialId, PlayerDataHolder.SocialId },
                };
                listner = null;
                OnSuccess = null;
                UpdateUserData(updatedData, () =>
                {
                    listner = null;
                    OnSuccess = null;
                    OnSuccess = (_userData) => OnUserFound();
                    OnDatabseDocUpdate();
                });
                needToUpdateProfile = false;
                GameState.NeedToSync = false;
                PlayerDataHolder.PlayerName = updatedData[StaticKeywords.UserDataKeyWords.userName].ToString();
            }
            else CreateNewUser();
        }
        else
        {
            Debug.Log("Social Id: " + PlayerDataHolder.SocialId);
            Query queryRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).WhereEqualTo(StaticKeywords.UserDataKeyWords.socialId, PlayerDataHolder.SocialId);
            queryRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    var result = task.Result;
                    if (result.Count > 0)
                    {
                        foreach (var userData in result)
                        {
                            foreach (var userFields in userData.ToDictionary())
                            {
                                if (userFields.Key == StaticKeywords.UserDataKeyWords.socialId)
                                {
                                    if (!string.IsNullOrEmpty(userFields.Value.ToString()) && userFields.Value.ToString() == PlayerDataHolder.SocialId)
                                    {
                                        Debug.Log("User already exists!!! ");
                                        if (userData.ToDictionary().ContainsKey(StaticKeywords.UserDataKeyWords.userId))
                                        {
                                            if (PlayerDataHolder.isGuestLogin) DeleteUser(PlayerDataHolder.UserId, null);
                                            PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, userData.ToDictionary()[StaticKeywords.UserDataKeyWords.userId].ToString());
                                            if (shouldSync)
                                            {
                                                needToUpdateProfile = false;
                                                GameState.NeedToSync = false;
                                            }
                                            OnUserFound();
                                            UIManager.OpenHomeScreen();
                                            PlayerDataHolder.PlayerName = userData.ToDictionary()[StaticKeywords.UserDataKeyWords.userName].ToString();
                                        }
                                        else
                                        {
                                            if (PlayerDataHolder.isGuestLogin) DeleteUser(PlayerDataHolder.UserId, null);
                                            if (shouldSync)
                                            {
                                                needToUpdateProfile = false;
                                                GameState.NeedToSync = false;
                                            }
                                            CreateNewUser();
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        GameState.NeedToSync = true;
                                        CreateNewUser();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GameState.NeedToSync)
                        {
                            Debug.Log("Need to sync profile!!!");
                            Dictionary<string, object> updatedData = new Dictionary<string, object>() {
                            {StaticKeywords.UserDataKeyWords.userName, FirebaseAuthentication.instance.auth.CurrentUser.DisplayName},
                            {StaticKeywords.UserDataKeyWords.profileImageUrl, FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl.ToString() },
                            {StaticKeywords.UserDataKeyWords.accountType, userDataHolder.userData[StaticKeywords.UserDataKeyWords.accountType].ToString() },
                            {StaticKeywords.UserDataKeyWords.socialId, PlayerDataHolder.SocialId },
                            };
                            listner = null;
                            OnSuccess = null;
                            UpdateUserData(updatedData, ()=>
                            {
                                OnUserFound();
                                UIManager.OpenHomeScreen();
                            });
                            needToUpdateProfile = false;
                            GameState.NeedToSync = false;
                            PlayerDataHolder.PlayerName = updatedData[StaticKeywords.UserDataKeyWords.userName].ToString();
                        }
                        else
                        {
                            listner = null;
                            OnSuccess = null;
                            if (!string.IsNullOrEmpty(PlayerDataHolder.GuestUserId))
                            {
                                DeleteUser(PlayerDataHolder.UserId, () =>
                                {
                                    userDataHolder.userData[StaticKeywords.UserDataKeyWords.userId] = FirebaseAuthentication.instance.auth.CurrentUser.UserId;
                                    PlayerPrefs.SetString(StaticKeywords.UserDataKeyWords.userId, userDataHolder.userData[StaticKeywords.UserDataKeyWords.userId].ToString());
                                    CreateNewUser();
                                });
                            }
                            else CreateNewUser();
                        }
                    }
                }
                else if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.Log("Task is faulted: " + task.Exception.ToString());
                }
            });
        }

        void CreateNewUser()
        {
            Debug.Log("User not found. Creating new user...!");
            DocumentReference docRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).Document(PlayerDataHolder.UserId);
            docRef.SetAsync(userDataHolder.userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("New user created");
                    if (userDataHolder.userData[StaticKeywords.UserDataKeyWords.accountType].Equals(StaticKeywords.AuthProvider.guest)) needToUpdateProfile = true;
                    if (GameState.NeedToSync)
                    {
                        listner = null;
                        OnSuccess = null;
                        OnSuccess = (_userData) =>
                        {
                            Debug.Log("Need to sync profile!!!");
                            Dictionary<string, object> updatedData = new Dictionary<string, object>()
                            {
                                {StaticKeywords.UserDataKeyWords.userName, FirebaseAuthentication.instance.auth.CurrentUser.DisplayName},
                                {StaticKeywords.UserDataKeyWords.profileImageUrl, FirebaseAuthentication.instance.auth.CurrentUser.PhotoUrl.ToString() },
                                {StaticKeywords.UserDataKeyWords.coins, PlayerDataHolder._CoinCount },
                                {StaticKeywords.UserDataKeyWords.gems, PlayerDataHolder._GemCount }
                            };
                            UpdateUserData(updatedData, OnUserFound);
                            needToUpdateProfile = false;
                            GameState.NeedToSync = false;
                        };
                        OnDatabseDocUpdate();
                        PlayerDataHolder.PlayerName = FirebaseAuthentication.instance.auth.CurrentUser.DisplayName;
                    }
                    else
                    {
                        Debug.Log("No need to sync");
                        listner = null;
                        OnSuccess = null;
                        Database.OnSuccess = (userData) => OnUserFound();
                        OnDatabseDocUpdate();
                        needToUpdateProfile = false;
                        GameState.NeedToSync = false;
                        PlayerDataHolder.PlayerName = userDataHolder.userData[StaticKeywords.UserDataKeyWords.userName].ToString();
                    }
                }
                else
                {
                    Debug.Log("Task not completed: " + task.Exception.ToString());
                    UIManager.OnLogInFailed("");
                }
            });
        }
    }

    public static Action<Dictionary<string, object>> OnSuccess;
    public static ListenerRegistration listner = null;
    public void OnDatabseDocUpdate(string userId = null)
    {
        Debug.Log("User id: " + userId);
        if (listner != null) listner.Stop();
        DocumentReference docRef = FirebaseAuthentication.instance.firebaseFirestore.Collection(StaticKeywords.CollectionKeyWords.users).Document(userId == null ? PlayerDataHolder.UserId : userId);
        listner = docRef.Listen(snapshot =>
        {
            Debug.Log("Doc updated");
            foreach (var _data in snapshot.ToDictionary()) Debug.Log("Data: " + _data.Key + ":::" + _data.Value);
            if (OnSuccess != null)
            {
                Debug.Log("Executing snapshot!!!");
                OnSuccess(snapshot.ToDictionary());
            }
        });
    }

    public void CheckUserAccount(Action UserFound, Action UserNotFound)
    {
        Query queryRef = firestoredatabase.Collection(StaticKeywords.CollectionKeyWords.users).WhereEqualTo(StaticKeywords.UserDataKeyWords.socialId, PlayerDataHolder.SocialId);
        queryRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                var result = task.Result;
                if (result.Count > 0)
                {
                    foreach (var userData in result)
                    {
                        foreach (var userFields in userData.ToDictionary())
                        {
                            if (userFields.Key == StaticKeywords.UserDataKeyWords.socialId)
                            {
                                if (!string.IsNullOrEmpty(userFields.Value.ToString()) && userFields.Value.ToString() == PlayerDataHolder.SocialId)
                                {
                                    Debug.Log("User already exists!!! ");
                                    if (UserFound != null) UserFound();
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("User does not exist!!!");
                    if (UserNotFound != null) UserNotFound();
                }
            }
            else if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Task is faulted: " + task.Exception.ToString());
            }
        });
    }
}
