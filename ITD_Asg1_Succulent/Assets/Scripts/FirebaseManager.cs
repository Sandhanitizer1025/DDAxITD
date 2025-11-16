using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
public class MyDatabase : MonoBehaviour
{
 DatabaseReference mDatabaseRef;
 void Start() {
 // Get the root reference location of the database.
 mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
 WriteNewUser("291","DDA MAN","dda@gmail.com");
 }
 private void WriteNewUser(string userId, string name, string email)
{
 User user = new User(name, email);
 string json = JsonUtility.ToJson(user);
mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
 }
}

