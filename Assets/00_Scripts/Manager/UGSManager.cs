using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataDeclaration;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

/// <summary>
/// Unity Gaming Service 관리 클래스
/// </summary>
public sealed class UGSManager : SingletonBehaviour<UGSManager>
{
    private UserInfo userInfo;

    public UserInfo UserInfo => userInfo;
    
    protected override async void Awake()
    {
        try
        {
            base.Awake();

            var options = new InitializationOptions();
            options.SetOption("Authentication.Anonymous", false); // 익명 로그인 비활성화

            // Unity Services SDK 초기화
            await UnityServices.InitializeAsync(options);
            
            AuthenticationService.Instance.SignedIn += OnSignedIn;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    /// <summary>
    /// <para>사용자 이름/비밀번호 자격 증명을 사용하여 새 플레이어를 생성한다.</para>
    /// <para>참고: 사용자 이름은 사용자 이름은 대소문자를 구분하지 않습니다. 최소 3자, 최대 20자여야 하며 문자 A-Z 및 a-z, 숫자, 기호 ., -, @, _만 지원한다.</para>
    /// <para>참고: 비밀번호는 대소문자를 구분합니다. 최소 8자, 최대 30자이며 소문자 1자, 대문자 1자, 숫자 1자, 기호 1자 이상을 포함해야 합니다.</para>
    /// </summary>
    /// <param name="userID">사용자 아이디</param>
    /// <param name="password">사용자 비밀번호</param>
    /// <param name="userName">사용자 이름</param>
    public async Task SignUpAsync(string userID, string password, string userName)
    {
        // 회원가입
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(userID, password);

            // 닉네임 설정
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(userName);
                userInfo = new UserInfo
                {
                    UserName = AuthenticationService.Instance.PlayerName
                };
                await SaveDataAsync(Constants.USER_INFO, userInfo);
            }
            catch (Exception)
            {
                await AuthenticationService.Instance.DeleteAccountAsync();
                Debug.LogError("닉네임 설정 오류");
                throw;
            }
        }
        catch (AuthenticationException e)
        {
            Debug.LogError("인증 실패: " + e.Message);
            throw;
        }
        catch (RequestFailedException e)
        {
            Debug.LogError("요청 실패: " + e.Message);
            throw;
        }
    }

    /// <summary>
    /// <para>사용자 이름/비밀번호 자격 증명을 사용하여 기존 플레이어를 로그인시킨다.</para>
    /// <para>참고: 사용자 이름은 사용자 이름은 대소문자를 구분하지 않습니다. 최소 3자, 최대 20자여야 하며 문자 A-Z 및 a-z, 숫자, 기호 ., -, @, _만 지원한다.</para>
    /// <para>참고: 비밀번호는 대소문자를 구분합니다. 최소 8자, 최대 30자이며 소문자 1자, 대문자 1자, 숫자 1자, 기호 1자 이상을 포함해야 합니다.</para>
    /// </summary>
    /// <param name="userId">사용자 이름</param>
    /// <param name="password">사용자 비밀번호</param>
    public async Task SignInAsync(string userId, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(userId, password);
        }
        catch (AuthenticationException e)
        {
            Debug.LogError("인증 실패: " + e.Message);
            throw;
        }
        catch (RequestFailedException e)
        {
            Debug.LogError("요청 실패: " + e.Message);
            throw;
        }
    }

    /// <summary>
    /// Unity Cloud Save를 통해 유저 데이터 저장
    /// </summary>
    /// <param name="key">저장할 데이터의 key 값</param>
    /// <param name="data">저장할 데이터</param>
    public static async Task SaveDataAsync(string key, object data)
    {
        var saveData = new Dictionary<string, object> { { key, data } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
    }

    /// <summary>
    /// Unity Cloud Save를 통해 유저 데이터 로드
    /// </summary>
    /// <param name="key">로드할 데이터의 key 값</param>
    /// <typeparam name="T">반환받을 데이터의 타입</typeparam>
    /// <returns>로드된 데이터</returns>
    public static async Task<T> LoadDataAsync<T>(string key)
    {
        var dataKey = new HashSet<string> { key };
        try
        {
            var loadDataDict = await CloudSaveService.Instance.Data.Player.LoadAsync(dataKey);
            var data = loadDataDict[key].Value.GetAs<T>();
            return data;
        }
        catch (KeyNotFoundException)
        {
            var newData = default(T);
            return newData;
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 불러오기 실패: {e.Message}");
            throw;
        }
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut(true);
        userInfo.UserName = AuthenticationService.Instance.PlayerName;
    }

    private async void OnSignedIn()
    {
        try
        {
            userInfo = await LoadDataAsync<UserInfo>(Constants.USER_INFO);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    

    /// <summary>
    /// 인증 이벤트 등록
    /// <para>플레이어의 상태에 대한 업데이트를 받으려면, SignedIn, SignInFailed, SignedOut 이벤트 핸들러에 함수를 등록한다.</para>
    /// </summary>
    private static void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) => { Debug.LogError(err); };

        AuthenticationService.Instance.SignedOut += () => { Debug.Log("Player signed out."); };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }
}