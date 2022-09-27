using Unity.Services.Core.Editor;

namespace Unity.Services.Vivox.Editor
{
    public struct VivoxIdentifier : IEditorGameServiceIdentifier
    {
        /// <summary>
        /// Key for the Vivox package
        /// </summary>
        public string GetKey() => "Vivox";
    }
}
