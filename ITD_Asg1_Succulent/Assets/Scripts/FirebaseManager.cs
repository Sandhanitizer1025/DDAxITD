using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    private FirebaseAuth auth;

    async void Start()
    {
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        var result = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            Debug.Log("Firebase Auth Ready!");
        }
        else
        {
            Debug.LogError("Firebase not ready: " + result);
        }
    }

    // ================= REGISTER =================
    public async void RegisterUser()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        try
        {
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            Debug.Log("Registered! UID = " + user.UserId);
            statusText.text = "Registered as: " + user.Email;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Register Error: " + e.Message);
            statusText.text = "Error: " + e.Message;
        }
    }

    // ================= LOGIN =================
    public async void LoginUser()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        try
        {
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            Debug.Log("Logged in! UID = " + user.UserId);
            statusText.text = "Logged in as: " + user.Email;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Login Error: " + e.Message);
            statusText.text = "Error: " + e.Message;
        }
    }
}
