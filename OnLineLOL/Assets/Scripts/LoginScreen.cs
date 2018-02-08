using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour {
    #region 登陆面板
    [SerializeField]
    private InputField accountInput;
    [SerializeField]
    private InputField passwordInput;
    [SerializeField]
    private Button loginBtn;
    #endregion

    #region 注册面板
    [SerializeField]
    private InputField regAccountInput;
    [SerializeField]
    private InputField regPwInput;
    [SerializeField]
    private InputField regPw1Input;
    [SerializeField]
    private Button RegBtn;
    #endregion
    
    [SerializeField]
    private GameObject regPanel;

    [SerializeField]
    private WarningWindow ww;

    public void LoginOnClick()
    {
        if (accountInput.text.Length == 0 || accountInput.text.Length > 6)
        {
            //ww.Active("账号不合法");
            //ww.Active("密码不合法");
            WarningManager.errors.Add("账号不合法");
            //Debug.Log("账号不合法");
            return;
        }
        if (passwordInput.text.Length == 0 || passwordInput.text.Length > 6)
        {

            ww.Active("密码不合法");
            //WarningManager.errors.Add("密码不合法");
            //Debug.Log("密码不合法");
            return;
        }
        //验证通过 申请登陆
        //loginBtn.enabled = false;//把登陆按钮置为不可用
        loginBtn.interactable = false;
    }

    public void RegOnClick()//打开登陆面板
    {
        regPanel.SetActive(true);//GameObject才有SetActive方法
    }

    public void RegBtnClick()
    {
        if (regAccountInput.text.Length == 0 || regAccountInput.text.Length > 6)
        {
            WarningManager.errors.Add("账号不合法");
            //Debug.Log("账号不合法");
            return;
        }
        if (regPwInput.text.Length == 0 || regPwInput.text.Length > 6)
        {
            WarningManager.errors.Add("密码不合法");
            //Debug.Log("密码不合法");
            return;
        }
        if (!regPw1Input.text.Equals(regPwInput.text))
        {
            WarningManager.errors.Add("两次密码不一致");
            //Debug.Log("两次密码不一致");
            return;
        }
        //输入合法 发起注册 关闭注册面板
    }

    public void CloseRegPanel()
    {
        regPanel.SetActive(false);
    }

}
