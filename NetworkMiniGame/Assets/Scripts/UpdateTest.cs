using System;
using Firebase.Auth;
using UnityEngine;

public class UpdateTest : MonoBehaviour
{
    private string nickName;
    
    private void Start()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        nickName = user.DisplayName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"현재 유저 닉네임 : {nickName}");
        }
    }
}
