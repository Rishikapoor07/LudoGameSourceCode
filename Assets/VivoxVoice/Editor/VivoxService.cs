using Unity.Services.Core.Editor;
using UnityEditor;

namespace Unity.Services.Vivox.Editor
{
    class VivoxService : IEditorGameService
    {
        public string Name => "Vivox Service";

        public IEditorGameServiceIdentifier Identifier { get; } = new VivoxIdentifier();

        public bool RequiresCoppaCompliance => false;

        public bool HasDashboard => true;

        /// <summary>
        /// Getter for the formatted dashboard url
        /// If <see cref="HasDashboard"/> is false, this field only need return null or empty string
        /// </summary>
        /// <returns>The formatted URL</returns>
        public string GetFormattedDashboardUrl()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return $"https://dashboard.unity3d.com/organizations/{CloudProjectSettings.organizationKey}/projects/{CloudProjectSettings.projectId}/vivox/overview";
#else
            return "https://dashboard.unity3d.com/vivox/";
#endif
        }

        public IEditorGameServiceEnabler Enabler { get; } = new VivoxServiceEnabler();
    }
}
