using Unity.Services.Core.Editor;
using UnityEngine;

namespace Unity.Services.Vivox.Editor
{
    class VivoxServiceEnabler : IEditorGameServiceEnabler
    {
        public void Enable()
        {
            var settings = VivoxSettings.Instance;
            settings.IsServiceEnabled = true;
            settings.Save();
        }

        public void Disable()
        {
            var settings = VivoxSettings.Instance;
            settings.IsServiceEnabled = false;
            settings.Save();
        }

        public bool IsEnabled()
        {
            return VivoxSettings.Instance.IsServiceEnabled;
        }
    }
}
