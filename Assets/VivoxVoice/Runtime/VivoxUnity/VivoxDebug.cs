using System.Diagnostics;
using SystemDebug = System.Diagnostics.Debug;
#if UNITY_5_3_OR_NEWER
using UnityDebug = UnityEngine.Debug;
#endif

namespace VivoxUnity
{
    public class VivoxDebug
    {
        private static VivoxDebug _instance;

        /// <summary>Where to put the logs: 0 = both, 1 = Unity console only, 2 = Visual Studio console only.</summary>
        public int debugLocation;

        /// <summary>Set this to tell VivoxUnity whether to rethrow an exception that has occured internally.</summary>
        public bool throwInternalExcepetions = true;

        public static VivoxDebug Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VivoxDebug();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Debug the message.
        /// VivoxUnity uses this to catch exceptions thrown internally for logging.
        /// </summary>
        /// <param name="message">Message.</param>
        internal void VxExceptionMessage(string message)
        {
            string callerMethodName = new StackFrame(1).GetMethod().Name;
            string newMessage = $"{callerMethodName}: {message}";

            DebugMessage(newMessage, vx_log_level.log_error);
        }

        /// <summary>
        /// Debug the message.
        /// log_none (-1) throws away your message.
        /// log_warning (1) displays as a warning.
        /// log_error (0) displays as an error.
        /// log_info (2), log_debug (3), log_trace (4), log_all (5) display as a normal debug.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="severity">Defaults to 2.</param>
        public virtual void DebugMessage(object message, vx_log_level severity = vx_log_level.log_debug)
        {
            // Example: Attempt to log what was sent.
            try
            {
                if (severity == vx_log_level.log_none) return;
                if (severity != vx_log_level.log_error && severity != vx_log_level.log_warning)
                {
#if UNITY_5_3_OR_NEWER
                    if (debugLocation != 2)
                        UnityDebug.Log(message);
#endif
                    if (debugLocation != 1)
                        SystemDebug.WriteLine(message.ToString(), TraceLevel.Info.ToString());
                }
                if (severity == vx_log_level.log_warning)
                {
#if UNITY_5_3_OR_NEWER

                    if (debugLocation != 2)
                        UnityDebug.LogWarning(message);
#endif
                    if (debugLocation != 1)
                        SystemDebug.WriteLine(message.ToString(), TraceLevel.Warning.ToString());
                }
                if (severity == vx_log_level.log_error)
                {
#if UNITY_5_3_OR_NEWER

                    if (debugLocation != 2)
                        UnityDebug.LogError(message);
#endif
                    if (debugLocation != 1)
                        SystemDebug.WriteLine(message.ToString(), TraceLevel.Error.ToString());
                }
            }
            catch (System.Exception e)
            {
#if UNITY_5_3_OR_NEWER

                if (debugLocation != 2)
                    UnityDebug.LogError(message);
#endif
                if (debugLocation != 1)
                    SystemDebug.WriteLine(e, TraceLevel.Error.ToString());
            }
        }
    }
}
