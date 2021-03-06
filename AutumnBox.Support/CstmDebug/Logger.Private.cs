﻿/* =============================================================================*\
*
* Filename: Logger.Private
* Description: 
*
* Version: 1.0
* Created: 2017/10/28 15:35:06 (UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
//#define LOG_TEST
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace AutumnBox.Support.CstmDebug
{
    [LogProperty(TAG = "Logger Father!", Show = true)]
    public static partial class Logger
    {
        private static readonly string DEFAULT_LOGFLODER = "logs/";
        private static readonly string DEFAULT_LOGFILE = "default.log";
        private static string NewFloder;
        static Logger()
        {
            if (!Directory.Exists(DEFAULT_LOGFLODER)) Directory.CreateDirectory(DEFAULT_LOGFLODER);
            NewFloder = DateTime.Now.ToString("yy_MM_dd/");
            if (!Directory.Exists(DEFAULT_LOGFLODER + NewFloder)) Directory.CreateDirectory(DEFAULT_LOGFLODER + NewFloder);
        }
        /// <summary>
        /// Get the method caller
        /// </summary>
        /// <returns></returns>
        private static MethodBase GetCaller(int skip = 2)
        {
            MethodBase result;
            do
            {
                result = new StackFrame(skip).GetMethod();
                skip++;
            } while (!(result.IsOk()));
#if LOG_TEST
            Debug.WriteLine($"{result.Name} is invoke?" + result.IsInvoke());
#endif
            return result;
        }
        private static bool IsOk(this MethodBase method)
        {
            if (method.IsAnonymous()) return false;
            else if (method.IsInvoke()) return false;
            else if (method.IsDispatcher()) return false;
            else return true;
        }
        /// <summary>
        /// Core from https://stackoverflow.com/questions/23228075/determine-if-methodinfo-represents-a-lambda-expression
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool IsAnonymous(this MethodBase method)
        {
            var invalidChars = new[] { '<', '>' };
            return method.Name.Any(invalidChars.Contains);
        }
        private static bool IsDispatcher(this MethodBase method)
        {
            return method.Name.Contains("Dispatcher");
        }
        private static bool IsInvoke(this MethodBase method)
        {
            return method.Name == "Invoke";
        }

        /// <summary>
        /// bool.ToErrorLevel
        /// </summary>
        /// <param name="isError"></param>
        /// <returns></returns>
        private static int ToErrorLevel(this bool isError)
        {
            if (isError) return 1;
            else return 0;
        }
        /// <summary>
        /// 智能化获取log依赖,如果调用者没有Log特性,则返回一个常规的log特性
        /// </summary>
        /// <returns></returns>
        private static LogPropertyAttribute GetLogPropertyAttribute(MethodBase caller)
        {
            LogPropertyAttribute result = new LogPropertyAttribute();
            var methodAttr = (LogPropertyAttribute)caller.GetCustomAttribute(typeof(LogPropertyAttribute));
            var classAttr = (LogPropertyAttribute)caller.ReflectedType.GetCustomAttribute(typeof(LogPropertyAttribute));
            if (methodAttr != null)
            {
                result.TAG = methodAttr.TAG;
                result.Show = methodAttr.Show;
            }
            else if (classAttr != null)
            {
                result.TAG = classAttr.TAG;
                result.Show = (result.Show == true && classAttr.Show == true);
            }
            if (result.TAG == LogPropertyAttribute.NOT_LOAD_TAG)
            {
                result.TAG = caller.ReflectedType.Name;
            }
            return result;
        }
        /// <summary>
        /// Get Full message like [17-10-31_03:50:23][LoggerTest/EXCEPTION] : hehe
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="errorLevel"></param>
        /// <returns></returns>
        private static string GetFullMessage(string tag, string message, int errorLevel)
        {
            string t = DateTime.Now.ToString("yy-MM-dd_HH:mm:ss");
            switch (errorLevel)
            {
                case 0:
                    return $"[{t}][{tag}/INFO] : {message}";
                case 1:
                    return $"[{t}][{tag}/WARNING] : {message}";
                default:
                    return $"[{t}][{tag}/EXCEPTION] : {message}";
            }
        }
        private static string LogFileNameOf(MethodBase methodCaller)
        {
            return LogFileNameOf(methodCaller.ReflectedType);
        }
        private static string LogFileNameOf(Type type)
        {
            string _LogFileName = DEFAULT_LOGFILE;
            var assInfo = Assembly.GetAssembly(type);
            var assAttr = assInfo.GetCustomAttributes(typeof(LogFilePropertyAttribute), true);
            if (assAttr.Length != 0)
            {
                _LogFileName = ((LogFilePropertyAttribute)assAttr[0]).FileName;
            }
            return _LogFileName;
        }
        private static string LogFileNameOf(ILogSender sender)
        {
            if (sender is LogSender)
            {
                return LogFileNameOf((sender as LogSender).GetOwnerType());
            }
            else
            {
                return LogFileNameOf(sender.GetType());
            }
        }
        private static void WriteToFile(string fileName, string fullMsg)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(DEFAULT_LOGFLODER + NewFloder + fileName, true))
                {
                    writer.WriteLine(fullMsg);
                    writer.Flush();
                }
            }
            catch { }
        }
    }
}
