using UnityEditor.SettingsManagement;

namespace Unity.Services.Vivox.Editor
{
    public class VivoxSettings
    {
        const string k_PackageName = "com.unity.services.vivox";
        const string k_Server = "server";
        const string k_Domain = "domain";
        const string k_TokenIssuer = "tokenIssuer";
        const string k_TokenKey = "tokenKey";
        const string k_IsServiceEnabledKey = "isServiceEnabled";
        const string k_IsTestModeKey = "isTestMode";
        const string k_IsEnvironmentCustom = "isEnvironmentCustom";

        static VivoxSettings s_Instance;

        public static VivoxSettings Instance
        {
            get
            {
                if (s_Instance is null)
                {
                    s_Instance = new VivoxSettings();
                }

                return s_Instance;
            }
        }

        readonly Settings m_Settings;

        VivoxSettings()
        {
            m_Settings = new Settings(k_PackageName);
        }

        public string Server
        {
            get => m_Settings.Get<string>(k_Server);
            set => m_Settings.Set(k_Server, value);
        }

        public string Domain
        {
            get => m_Settings.Get<string>(k_Domain);
            set => m_Settings.Set(k_Domain, value);
        }

        public string TokenIssuer
        {
            get => m_Settings.Get<string>(k_TokenIssuer);
            set => m_Settings.Set(k_TokenIssuer, value);
        }

        public string TokenKey
        {
            get => m_Settings.Get<string>(k_TokenKey);
            set => m_Settings.Set(k_TokenKey, value);
        }

        public bool IsEnvironmentCustom
        {
            get => m_Settings.Get<bool>(k_IsEnvironmentCustom);
            set => m_Settings.Set(k_IsEnvironmentCustom, value);
        }

        public bool IsServiceEnabled
        {
            get => m_Settings.Get<bool>(k_IsServiceEnabledKey);
            set => m_Settings.Set(k_IsServiceEnabledKey, value);
        }

        public bool IsTestMode
        {
            get => m_Settings.Get<bool>(k_IsTestModeKey, fallback: false);
            set => m_Settings.Set(k_IsTestModeKey, value);
        }

        public void Save()
        {
            m_Settings.Save();
        }
    }
}