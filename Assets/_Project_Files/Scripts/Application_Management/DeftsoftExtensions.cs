using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class DeftsoftExtensions
{
    public static void AddListnerWithDelayCall(this UnityEngine.Events.UnityEvent theEvent, UnityEngine.Events.UnityAction theAction, float theDelay)
    {
        theEvent.AddListener(() => HelperUtil.CallAfterDelay(() => theAction(), theDelay));
    }

    public static T RemoveNull_Find<T>(this List<T> theList, Predicate<T> match)
    {
        theList.RemoveAll(itemToRemove => itemToRemove == null);
        return theList.Find(match);
    }

    public static void AddIfNotAvailable<T>(this IList theList, T theItem) 
    {
        if (!theList.Contains(theItem)) theList.Add(theItem);
    }

    public static void RemoveIfNotAvailable<T>(this IList theList, T theItem)
    {
        if (theList.Contains(theItem)) theList.Remove(theItem);
    }

    public static void AddIfNotAvailable<TKey, TValue>(this Dictionary<TKey,TValue> theDictionary, TKey theKey, TValue theValue)
    {
        if (!theDictionary.ContainsKey(theKey)) theDictionary.Add(theKey, theValue);
    }

    public static string CustomReverse(this string str)
    {
        string temp = "";
        for (int i = str.Length - 1; i >= 0; i--)
            temp += str[i];
        return temp;
    }

    public static TypedLanguage CheckIfEnglish(this InputField inputField, int scene = 0)
    {

        TypedLanguage language = TypedLanguage.Other;
        if (System.Text.RegularExpressions.Regex.IsMatch(inputField.text, "^[a-zA-Z0-9 ./<>?;:\"'`!@#$%^&*()\\[\\]{}_+=|\\-]*$")) language = TypedLanguage.English;
        else if (System.Text.RegularExpressions.Regex.IsMatch(inputField.text, "^[\u0621-\u064A\u0660-\u0669 ./<>?;:\"'`!@#$%^&*()\\[\\]{}_+=|\\-]+$")) language = TypedLanguage.Arabic;
        else language = TypedLanguage.Other;
        return language;
    }

    public static TypedLanguage CheckIfEnglish(this TMPro.TMP_InputField inputField, int scene = 0)
    {

        TypedLanguage language = TypedLanguage.Other;
        if (System.Text.RegularExpressions.Regex.IsMatch(inputField.text, "^[a-zA-Z0-9 ./<>?;:\"'`!@#$%^&*()\\[\\]{}_+=|\\-]*$")) language = TypedLanguage.English;
        else if (System.Text.RegularExpressions.Regex.IsMatch(inputField.text, "^[\u0621-\u064A\u0660-\u0669 ./<>?;:\"'`!@#$%^&*()\\[\\]{}_+=|\\-]+$")) language = TypedLanguage.Arabic;
        else language = TypedLanguage.Other;
        return language;
    }

    public static TypedLanguage CheckIfEnglish(string textToCheck, int scene = 0)
    {

        TypedLanguage language = TypedLanguage.Other;
        if (System.Text.RegularExpressions.Regex.IsMatch(textToCheck, "^[a-zA-Z0-9 ./<>?;:\"'`!@#$%^&*()\\[\\]{}_+=|\\-]*$")) language = TypedLanguage.English;
        else if (System.Text.RegularExpressions.Regex.IsMatch(textToCheck, "^[\u0621-\u064A\u0660-\u0669 ./<>?;:\"'`!@#$%^&*()\\[\\]{}_+=|\\-]+$")) language = TypedLanguage.Arabic;
        else language = TypedLanguage.Other;
        return language;
    }

    //public static void SetProperty(this Room currentRoom, string propertyToAdd, object Value)
    //{
    //    Hashtable temp = new Hashtable();
    //    temp.Add(propertyToAdd, Value);
    //    currentRoom.SetCustomProperties(temp);
    //}

    //public static T GetProperty<T>(this Room currentRoom, string propertyToFetch)
    //{
    //    return (T)currentRoom.CustomProperties[propertyToFetch];
    //}

    //public static void SetProperty(this Player LocalPlayer, string propertyToAdd, object Value)
    //{
    //    Hashtable temp = new Hashtable();
    //    temp.Add(propertyToAdd, Value);
    //    LocalPlayer.SetCustomProperties(temp);
    //}

    //public static T GetProperty<T>(this Player LocalPlayer, string propertyToFetch)
    //{
    //    return (T)LocalPlayer.CustomProperties[propertyToFetch];
    //}

}
