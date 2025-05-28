using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataDeclaration;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public sealed class UserManager : SingletonBehaviour<UserManager>
{
    private UserInfo userInfo;

    public UserInfo UserInfo => userInfo;

    protected override void Awake()
    {
        base.Awake();
        AuthenticationService.Instance.SignedIn += OnSignedIn;
    }

    private void Update()
    {
        Debug.Log($"플레이어 이름: {userInfo.UserName}");

        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("로그인 상태입니다.");
        }
        else
        {
            Debug.Log("아직 로그인하지 않았습니다.");
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
    public async Task SignUpWithUsernamePasswordAsync(string userID, string password, string userName)
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
    public async Task SignInWithUsernamePasswordAsync(string userId, string password)
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

    #region Achievement Enums

    // <summary>
    // 도전과제 조건 종류(조건 비교를 위한 타입)
    // </summary>
    //public enum AchievementConditionType
    //{
    //    /// <summary> 누적 킬 수 </summary>
    //    KillCount,

    //    /// <summary> 헤드샷 명중률 (퍼센트) </summary>
    //    HeadshotRatio,

    //    /// <summary> 특정 거리 이상에서 명중 </summary>
    //    DistanceShot,

    //    /// <summary> 연속 명중 횟수 </summary>
    //    ConsecutiveHits,

    //    /// <summary> 한 번도 빗나가지 않고 클리어 </summary>
    //    NoMissRun,

    //    /// <summary> 제한 시간 내 클리어 </summary>
    //    ClearTime,

    //    /// <summary> 연속 콤보 수 </summary>
    //    ComboCount,

    //    /// <summary> 특정 스테이지 클리어 </summary>
    //    StageClear,

    //    /// <summary> 특정 무기로 킬 </summary>
    //    WeaponSpecificKill
    //}

    #endregion
}