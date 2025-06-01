using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataDeclaration;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseManager : SingletonBehaviour<FirebaseManager>
{
    private FirebaseAuth auth; // 로그인, 회원가입에 사용
    private FirebaseUser user; // 인증이 완료된 유저 정보

    private FirebaseFirestore db;

    public Action<bool> SignInState;

    public FirebaseAuth Auth => auth;
    public FirebaseUser User => user;
    public FirebaseFirestore DB => db;

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }

    public async Task SignUpAsync(string email, string password, string nickname)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var newUser = result.User;

            var userInfo = new UserInfo()
            {
                UserName = nickname,
                Gold = 0,
            };
            await db.Collection("users").Document(newUser.UserId).SetAsync(userInfo);

            var userRankData = new UserRankData();
            await db.Collection("users").Document(newUser.UserId).Collection("rank").Document("data").SetAsync(userRankData);
        }
        catch (Exception)
        {
            Debug.LogAssertion("회원가입 실패");
            throw;
        }
    }

    public async Task SignInAsync(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var newUser = result.User;
        }
        catch (Exception)
        {
            Debug.LogAssertion("로그인 실패");
            throw;
        }
    }

    public void SignOut()
    {
        auth.SignOut();
        Debug.Log("로그아웃");
    }

    public async Task UpdateDataAsync(string field, object value)
    {
        await db.Collection("users").Document(user.UserId).UpdateAsync(field, value);
    }

    public async Task<T> LoadUserDataAsync<T>(string field)
    {
        var result = await db.Collection("users").Document(user.UserId).GetSnapshotAsync();
        return result.GetValue<T>(field);
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;

                auth.StateChanged += OnAuthStateChanged;
                OnAuthStateChanged(this, null); // 처음 한 번 호출해서 현재 로그인 상태 체크
                Debug.Log("Firebase 초기화 완료");
                SignOut();
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
    }

    private void OnAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("로그아웃됨");
                SignInState?.Invoke(false);
            }

            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"로그인됨: {user.UserId}");
                SignInState?.Invoke(true);
            }
        }
    }

    public async Task SaveAchievementAsync(string achievementId)
    {
        var data = new Dictionary<string, object>
    {
        { "isCompleted", true },
        { "achievedAt", Timestamp.GetCurrentTimestamp() }
    };

        await db.Collection("users")
                .Document(user.UserId)
                .Collection("achievements")
                .Document(achievementId)
                .SetAsync(data, SetOptions.MergeAll);

        Debug.Log($"도전과제 저장 완료: {achievementId}");
    }

    public async Task<HashSet<string>> LoadCompletedAchievementsAsync()
    {
        var snapshot = await db.Collection("users")
                               .Document(user.UserId)
                               .Collection("achievements")
                               .GetSnapshotAsync();

        var result = new HashSet<string>();
        foreach (var doc in snapshot.Documents)
        {
            bool isCompleted = doc.ContainsField("isCompleted") && doc.GetValue<bool>("isCompleted");
            if (isCompleted)
            {
                result.Add(doc.Id);
            }
        }

        return result;
    }

}