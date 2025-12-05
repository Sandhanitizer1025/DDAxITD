using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI messageText;

    // Optional – panels to switch after login
    public GameObject loginPanel;
    public GameObject mainGamePanel;

    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private bool firebaseReady = false;

    void Awake()
    {
        // Make sure Firebase is ready before using it
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                firebaseReady = true;
                ShowMessage("Firebase ready. You can login or register.");
            }
            else
            {
                ShowMessage("Firebase init failed: " + status.ToString());
            }
        });
    }

    // Called by Login button
    public void OnLoginButton()
    {
        if (!firebaseReady)
        {
            ShowMessage("Please wait, Firebase is still initialising...");
            return;
        }

        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter both email and password.");
            return;
        }

        ShowMessage("Logging in...");

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    HandleAuthError(task.Exception);
                    return;
                }

                currentUser = task.Result.User;
                ShowMessage("Logged in as " + currentUser.Email);
                OnLoginSuccess();
            });
    }

    // Called by Register button
    public void OnRegisterButton()
    {
        if (!firebaseReady)
        {
            ShowMessage("Please wait, Firebase is still initialising...");
            return;
        }

        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter both email and password.");
            return;
        }

        if (password.Length < 6)
        {
            ShowMessage("Password must be at least 6 characters.");
            return;
        }

        ShowMessage("Creating account...");

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    HandleAuthError(task.Exception);
                    return;
                }

                currentUser = task.Result.User;
                ShowMessage("Account created! Logged in as " + currentUser.Email);
                OnLoginSuccess();
            });
    }

    private void OnLoginSuccess()
    {
        // This is where you switch to your AR succulent scene,
        // or hide login panel & show main UI.

        if (loginPanel != null) loginPanel.SetActive(false);
        if (mainGamePanel != null) mainGamePanel.SetActive(true);

        // Example if you want to load a scene instead:
        // SceneManager.LoadScene("SucculentScene");
    }

    public void OnLogoutButton()
    {
        if (auth != null)
        {
            auth.SignOut();
            currentUser = null;
            ShowMessage("You have been logged out.");

            if (loginPanel != null) loginPanel.SetActive(true);
            if (mainGamePanel != null) mainGamePanel.SetActive(false);
        }
    }

    private void HandleAuthError(AggregateException ex)
    {
        string niceMessage = "Authentication failed.";

        if (ex != null)
        {
            foreach (var e in ex.InnerExceptions)
            {
                FirebaseException fe = e as FirebaseException;
                if (fe != null)
                {
                    var errorCode = (AuthError)fe.ErrorCode;
                    switch (errorCode)
                    {
                        case AuthError.InvalidEmail:
                            niceMessage = "Invalid email format.";
                            break;
                        case AuthError.WrongPassword:
                            niceMessage = "Incorrect password.";
                            break;
                        case AuthError.UserNotFound:
                            niceMessage = "No account found with this email.";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            niceMessage = "Email is already in use.";
                            break;
                        case AuthError.WeakPassword:
                            niceMessage = "Password is too weak.";
                            break;
                        default:
                            niceMessage = "Auth error: " + errorCode.ToString();
                            break;
                    }
                }
            }
        }

        ShowMessage(niceMessage);
    }

    private void ShowMessage(string msg)
    {
        Debug.Log("[AuthManager] " + msg);
        if (messageText != null)
        {
            messageText.text = msg;
        }
    }

    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }
}
