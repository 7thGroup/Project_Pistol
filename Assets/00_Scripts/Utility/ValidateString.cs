using System.Text.RegularExpressions;
using UnityEngine;

public static class ValidateString
{
    public static void ValidateID(string input)
    {
        var pattern = @"^[a-zA-Z0-9.@_-]{3,20}$";

        if (Regex.IsMatch(input, pattern))
        {
            Debug.Log("아이디 유효함");
        }
        else
        {
            var ui = UIManager.Instance.OpenPopupUI<PopupNotice>();
            ui.SetContentText("경고", $"아이디가 유효하지 않음\n\n최소 3자, 최대 20자여야 하며 문자 A-Z 및 a-z, 숫자, 기호 ., -, @, _만 지원", "취소", "확인");
            
        }
    }
    
    public static void ValidatePassword(string input)
    {
        var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$";

        if (Regex.IsMatch(input, pattern))
        {
            Debug.Log("비밀번호 유효함");
        }
        else
        {
            var ui = UIManager.Instance.OpenPopupUI<PopupNotice>();
            ui.SetContentText("경고", $"비밀번호가 유효하지 않음\n\n최소 8자, 최대 30자이며 소문자 1자, 대문자 1자, 숫자 1자, 기호 1자 이상을 포함해야 합니다.", "취소", "확인");
        }
    }
}
