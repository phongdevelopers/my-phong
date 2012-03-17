//***************
//Based on code from http://developer.coreweb.com/articles/Default7.aspx
//Log4Net: The Definitive(?) How-To for Those Who Have Already Experienced Some Degree of Frustration Trying to Set It Up the First-Time Around
//by Chad Finsterwald
//***************

using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Web;
using CommerceBuilder.Common;
using CommerceBuilder.Stores;
using System.IO;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class to support logging
    /// </summary>
    public static class Logger
    {
        private static ILog m_log;

        /// <summary>
        /// Static constructor
        /// </summary>
        static Logger()
        {
            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\log4net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configFile));
        }

        /// <summary>
        /// Writes a log message in the log
        /// </summary>
        /// <param name="message">The message to write to log</param>
        /// <param name="messageType">The type of the log message to write</param>
        public static void Write(string message, LogMessageType messageType)
        {
            DoLog(message, messageType, null, Type.GetType("System.Object"));
        }

        /// <summary>
        /// Writes a log message in the log
        /// </summary>
        /// <param name="message">The message to write to log</param>
        /// <param name="messageType">The type of the log message to write</param>
        /// <param name="type">The object type for which to get the logger for writing the log</param>
        public static void Write(string message, LogMessageType messageType, Type type)
        {
            DoLog(message, messageType, null, type);
        }

        /// <summary>
        /// Writes a log message in the log
        /// </summary>
        /// <param name="message">The message to write to log</param>
        /// <param name="messageType">The type of the log message to write</param>
        /// <param name="ex">Exception to be logged</param>
        public static void Write(string message, LogMessageType messageType, Exception ex)
        {
            DoLog(message, messageType, ex, Type.GetType("System.Object"));
        }

        /// <summary>
        /// Writes a log message in the log
        /// </summary>
        /// <param name="message">The message to write to log</param>
        /// <param name="messageType">The type of the log message to write</param>
        /// <param name="ex">Exception to be logged</param>
        /// <param name="type">The object type for which to get the logger for writing the log</param>
        public static void Write(string message, LogMessageType messageType, Exception ex,
           Type type)
        {
            DoLog(message, messageType, ex, type);
        }

        /// <summary>
        /// Assertion is logged for given condition with the given message
        /// </summary>
        /// <param name="condition">The value condition evaluates to</param>
        /// <param name="message">The message to log</param>
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, Type.GetType("System.Object"));
        }

        /// <summary>
        /// Assertion is logged for given condition with the given message
        /// </summary>
        /// <param name="condition">The value condition evaluates to</param>
        /// <param name="message">The message to log</param>
        /// <param name="type">The object type for which to get the logger for writing the log</param>
        public static void Assert(bool condition, string message, Type type)
        {
            if (condition == false)
                Write(message, LogMessageType.Info);
        }

        /// <summary>
        /// Writes a log message in the log
        /// </summary>
        /// <param name="message">The message to write to log</param>
        /// <param name="messageType">The type of the log message to write</param>
        /// <param name="ex">Exception to be logged</param>
        /// <param name="type">The object type for which to get the logger for writing the log</param>
        private static void DoLog(string message, LogMessageType messageType, Exception ex, Type type)
        {
            m_log = LogManager.GetLogger(type);
            ErrorMessage errmsg;
            string debugData = string.Empty;
            switch (messageType)

            {
                case LogMessageType.Debug:
                    //THE ONLINE ERROR LOG SHOULD BE LIMITED TO ERRORS
                    //errmsg = new ErrorMessage(MessageSeverity.Debug, message, string.Empty);
                    //errmsg.Save();
                    Logger.m_log.Debug(message, ex);
                    break;

                case LogMessageType.Info:
                    //THE ONLINE ERROR LOG SHOULD BE LIMITED TO ERRORS
                    //errmsg  = new ErrorMessage(MessageSeverity.Info, message, string.Empty);
                    //errmsg.Save();
                    Logger.m_log.Info(message, ex);
                    break;

                case LogMessageType.Warn:
                    debugData = (ex == null) ? string.Empty : ex.Message;
                    if ((ex != null) && (ex.InnerException != null)) debugData += "; " + ex.InnerException.Message;
                    errmsg = new ErrorMessage(MessageSeverity.Warn, StringHelper.Truncate(message,255), debugData);
                    errmsg.Save();
                    Logger.m_log.Warn(message, ex);
                    break;

                case LogMessageType.Error:
                    debugData = (ex == null) ? string.Empty : ex.Message;
                    if ((ex != null) && (ex.InnerException != null)) debugData += "; " + ex.InnerException.Message;
                    errmsg = new ErrorMessage(MessageSeverity.Error, StringHelper.Truncate(message,255), debugData);
                    errmsg.Save();
                    Logger.m_log.Error(message, ex);
                    break;

                case LogMessageType.Fatal:
                    debugData = (ex == null) ? string.Empty : ex.Message;
                    if ((ex != null) && (ex.InnerException != null)) debugData += "; " + ex.InnerException.Message;
                    errmsg = new ErrorMessage(MessageSeverity.Fatal, StringHelper.Truncate(message,255), debugData);
                    errmsg.Save();
                    Logger.m_log.Fatal(message, ex);
                    break;
            }
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public static void Debug(string message)
        {
            Write(message, LogMessageType.Debug);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="ex">The exception to log</param>
        public static void Debug(string message, Exception ex)
        {
            Write(message, LogMessageType.Debug, ex);
        }

        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public static void Info(string message)
        {
            Write(message, LogMessageType.Info);
        }

        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="ex">The exception to log</param>
        public static void Info(string message, Exception ex)
        {
            Write(message, LogMessageType.Info, ex);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public static void Warn(string message)
        {
            Write(message, LogMessageType.Warn);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="ex">The exception to log</param>
        public static void Warn(string message, Exception ex)
        {
            Write(message, LogMessageType.Warn, ex);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public static void Error(string message)
        {
            Write(message, LogMessageType.Error);
        }
        
        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="ex">The exception to log</param>
        public static void Error(string message, Exception ex)
        {
            Write(message, LogMessageType.Error, ex);
        }

        /// <summary>
        /// Log a fatal error message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public static void Fatal(string message)
        {
            Write(message, LogMessageType.Fatal);
        }

        /// <summary>
        /// Log a fatal error message
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        /// <param name="ex">The exception to log</param>
        public static void Fatal(string message, Exception ex)
        {
            Write(message, LogMessageType.Fatal, ex);
        }

        /// <summary>
        /// Writes an event to the audit log using the specified parameters.
        /// </summary>
        /// <param name="eventType">The type of the audit event</param>
        /// <param name="successful">Indicates whether the event was successful</param>
        /// <param name="comment">A brief explanation of the event</param>
        public static void Audit(AuditEventType eventType, bool successful, string comment)
        {
            string remoteIP;
            HttpContext context = HttpContext.Current;
            if (context != null) remoteIP = context.Request.UserHostAddress;
            else remoteIP = string.Empty;
            Audit(eventType, successful, comment, Token.Instance.UserId, 0, remoteIP);
        }

        /// <summary>
        /// Writes an event to the audit log using the specified parameters.
        /// </summary>
        /// <param name="eventType">The type of the audit event</param>
        /// <param name="successful">Indicates whether the event was successful</param>
        /// <param name="comment">A brief explanation of the event</param>
        /// <param name="userId">The user causing the audit event</param>
        public static void Audit(AuditEventType eventType, bool successful, string comment, int userId)
        {
            string remoteIP;
            HttpContext context = HttpContext.Current;
            if (context != null) remoteIP = context.Request.UserHostAddress;
            else remoteIP = string.Empty;
            Audit(eventType, successful, comment, userId, 0, remoteIP);
        }

        /// <summary>
        /// Writes an event to the audit log using the specified parameters.
        /// </summary>
        /// <param name="eventType">The type of the audit event</param>
        /// <param name="successful">Indicates whether the event was successful</param>
        /// <param name="comment">A brief explanation of the event</param>
        /// <param name="userId">The user causing the audit event</param>
        /// <param name="relatedId">A related ID to the event, if applicable</param>
        public static void Audit(AuditEventType eventType, bool successful, string comment, int userId, int relatedId)
        {
            string remoteIP;
            HttpContext context = HttpContext.Current;
            if (context != null) remoteIP = context.Request.UserHostAddress;
            else remoteIP = string.Empty;
            Audit(eventType, successful, comment, userId, relatedId, remoteIP);
        }

        /// <summary>
        /// Writes an event to the audit log using the specified parameters.
        /// </summary>
        /// <param name="eventType">The type of the audit event</param>
        /// <param name="successful">Indicates whether the event was successful</param>
        /// <param name="comment">A brief explanation of the event</param>
        /// <param name="userId">The user causing the audit event</param>
        /// <param name="relatedId">A related ID to the event, if applicable</param>
        /// <param name="remoteIP">IP of the remote system causing the event</param>
        public static void Audit(AuditEventType eventType, bool successful, string comment, int userId, int relatedId, string remoteIP)
        {
            AuditEvent logEntry = new AuditEvent();
            logEntry.EventDate = DateTime.UtcNow;
            logEntry.EventType = eventType;
            logEntry.Successful = successful;
            logEntry.UserId = userId;
            logEntry.RelatedId = relatedId;
            logEntry.RemoteIP = StringHelper.Truncate(remoteIP, 39);
            logEntry.Comment = comment;
            logEntry.Save();
        }

        /// <summary>
        /// Enumeration that represents the type of log message
        /// </summary>
        public enum LogMessageType
        {
            /// <summary>
            /// This is a debugging log
            /// </summary>
            Debug,

            /// <summary>
            /// This is information log
            /// </summary>
            Info,

            /// <summary>
            /// This is a warning log
            /// </summary>
            Warn,

            /// <summary>
            /// This is a error log
            /// </summary>
            Error,

            /// <summary>
            /// This is a fatal error log
            /// </summary>
            Fatal
        }

        /// <summary>
        /// Records communication in App_Data/Logs/ folder
        /// </summary>
        /// <param name="providerName">Name of the provider used for log file name</param>
        /// <param name="category">Category for the log entry.  Commonly used to indicate whether this is data being sent to or received from the provider</param>
        /// <param name="message">The message data to record</param>
        public static void WriteProviderLog(string providerName, string category, string message)
        {
            //GET LOG DIRECTORY
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\Logs\\");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string fileName = Path.Combine(directory, providerName + ".Log");
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(category.ToString() + ": " + message);
                sw.WriteLine(string.Empty);
                sw.Close();
            }
        }
    }
}