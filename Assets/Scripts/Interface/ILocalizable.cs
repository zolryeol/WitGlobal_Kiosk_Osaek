using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocalizable
{
    /// 로컬라이즈된 문자열을 업데이트합니다.
    /// </summary>
    void UpdateLocalizedString(string str);
}
