using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestAuth{
  public GuestNames guestNames = new GuestNames();
  public string GetGuestName(string nameJson) {
    JsonUtility.FromJsonOverwrite(nameJson, guestNames);
    string firstName = guestNames.names.firstname[UnityEngine.Random.Range(0, guestNames.names.firstname.Count)];
    string lastName = guestNames.names.lastname[UnityEngine.Random.Range(0, guestNames.names.lastname.Count)];
    return firstName + " " + lastName;
  }
}

[System.Serializable]
public class Names {
  public List<string> firstname;
  public List<string> lastname;
}

[System.Serializable]
public class GuestNames {
  public Names names;
}
