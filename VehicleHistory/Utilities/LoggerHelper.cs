using NLog;

namespace VehicleTrackingApp.Utilities
{
    public static class LoggerHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            Logger.Error(ex, message);
        }
    }
}
