using System;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App => Instance.app;

    private FirebaseAuth auth;
    public static FirebaseAuth Auth => Instance.auth;
    
    private TitleUI titleUi;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => 
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase dependencies check success");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
                // Firebase Unity SDK is not safe to use here.
                app = null;
                auth = null;
            }
        });
    }

    private void Start()
    {
        titleUi = FindFirstObjectByType<TitleUI>();
    }

    public void SignUp(string email, string password)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                titleUi.SignUpFail();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                titleUi.SignUpFail();
                return;
            }

            titleUi.SignUpSuccess();

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
        });
    }
}