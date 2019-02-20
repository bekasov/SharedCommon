namespace SharedCommon.Logger
{
    using System;

    public interface ILogger
    {
        DateTime LastLogTime { get; }

        void LogToStatusLine(string template, params object[] data);

        void Log(Exception e);

        void Log(string template, params object[] data);

        void AddSeparator();
    }
}