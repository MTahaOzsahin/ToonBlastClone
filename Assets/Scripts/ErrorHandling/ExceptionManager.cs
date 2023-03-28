using System;
using System.IO;
using UnityEngine;

namespace ErrorHandling
{
    public class ExceptionManager
    {
        private const string LogFolder = "ErrorLogs";

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            Application.logMessageReceived += HandleLog;
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;
            
            try
            {
                var path = LogFolder + "/" + DateTime.Now.ToFileTimeUtc() + ".log";

                var content = logString + "\n" + stackTrace;
                
                if (!Directory.Exists(LogFolder))
                {
                    Directory.CreateDirectory(LogFolder);
                }
                
                File.WriteAllText(path, content);
            }
            catch (Exception e)
            {
                // File write error
                Debug.LogError(e);
            }
        }
    }
}