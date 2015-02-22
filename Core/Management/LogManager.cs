using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.Threading;
using System.IO;

#pragma warning disable
namespace Core.Input
{
    public class LogWrittenArgs : EventArgs
    {
        public string LogName { get; private set; }
        public string LogContents { get; private set; }

        public LogWrittenArgs (string logname, string content)
	    {
            this.LogName = logname;
            this.LogContents = content;
	    }
    }

    /// <summary>
    /// Generates log files and manages each one. Allows you to write logs to disk from anywhere in the game context.
    /// Asynchronous
    /// </summary>
    internal sealed class LogManager
    {
        private Dictionary<string, Logger> logs;

        public event EventHandler<LogWrittenArgs> LogWritten, LogCreated;

        internal LogManager(Game game)
        {
            if (game == null)
                throw new ArgumentNullException("game");
            // this component doesn't need to be updated
            this.SubDirectory = "Log";
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        internal void Initialize()
        {
            this.logs = new Dictionary<string, Logger>();

            DirectoryInfo info = new DirectoryInfo(this.SubDirectory);
            if (!info.Exists)
                info.Create();

            this.AddLog("Exception");
            this.AddLog("Initialization");
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        /// <param name="disposing"></param>
        internal void Dispose(bool disposing)
        {
            this.WriteLine("Initialization", "LogManager Disposing");
        }

        /// <summary>
        /// Adds a log to the LogManager
        /// </summary>
        public void AddLog(string logName)
        {
            if (String.IsNullOrEmpty(logName) || String.IsNullOrEmpty(logName))
                throw new ArgumentNullException("logName");
            else if (this.logs.ContainsKey(logName))
                throw new ArgumentException("Log already exists with name: " + logName);
            else
            {
                this.logs.Add(logName, new Logger(logName, this));
                
                if (LogCreated != null)
                    LogCreated(this, new LogWrittenArgs(logName, ""));
            }
        }

        /// <summary>
        /// Writes a string to the specified logger
        /// </summary>
        /// <param name="logName">The name of the logger to output to</param>
        /// <param name="contents">The text to output to the logger</param>
        public void Write(string logName, string contents)
        {
            if (!this.logs.ContainsKey(logName))
                throw new ArgumentException("Log does not exist with name: " + logName);
            else if (String.IsNullOrEmpty(logName))
                throw new ArgumentNullException("logName");
            else
            {
                var item = this.logs[logName];
                item.Content.Append(contents);
                item.BeginFlush();
            }
        }

        /// <summary>
        /// Writes a string to the specified logger and breaks the line
        /// </summary>
        /// <param name="logName">The name of the logger to output to</param>
        /// <param name="contents">The text to output to the logger</param>
        public void WriteLine(string logName, string contents)
        {
            this.Write(logName, contents + Environment.NewLine);
        }

        /// <summary>
        /// A helper methods for the loggers
        /// </summary>
        protected void FireLogWritten(object sender, string logName, string content)
        {
            if (this.LogWritten != null)
                this.LogWritten(sender, new LogWrittenArgs(logName, content));
        }

        /// <summary>
        /// Where the logs are written to
        /// </summary>
        public String SubDirectory { get; set; }

        /// <summary>
        /// Delegates the writing of logs to separate helper threads
        /// </summary>
        private sealed class Logger
        {
            public readonly string name;
            public readonly LogManager LogManager;
            private StringBuilder content;

            private bool flushHasBegun = false;
            private bool firstRun = true;

            /// <summary>
            /// Initializes a new instance of logger
            /// </summary>
            /// <param name="name">The name of this logger, will also be the name of the log txt file</param>
            public Logger(string name, LogManager manager)
            {
                this.name = name;
                this.content = new StringBuilder();
                this.LogManager = manager;
            }

            /// <summary>
            /// Initializes the logger and creates the text file
            /// </summary>
            public void Initialize()
            {
                this.content.Insert
                (0, String.Format("Log {0} created on {1}{2}-------Log-Begin-------{3}", this.name, System.DateTime.Now, Environment.NewLine, Environment.NewLine));
            }

            /// <summary>
            /// Begins flushing operations on another thread
            /// </summary>
            public void BeginFlush()
            {
                if (flushHasBegun)
                    throw new InvalidOperationException("BeginFlush has already been called on this object and is not finished executing");

                // if the buffer doesnt have any data, then we dont bother
                if (!this.BufferHasData)
                    return;

                ThreadPool.QueueUserWorkItem(new WaitCallback(Flush));
            }

            /// <summary>
            /// Flushes the logger to it's text file
            /// </summary>
            private void Flush(object state)
            {
                lock (this.content)
                {
                    var stream = File.Open(this.LogManager.SubDirectory + @"\" + this.name + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter s = new StreamWriter(stream);

                    if (this.firstRun)
                    {
                        this.firstRun = false;
                        this.Initialize();
                    }
                
                    s.Write(this.content.ToString());

                    s.Close();

                    this.LogManager.FireLogWritten(this, this.name, this.content.ToString());
                    this.content.Remove(0, this.content.Length);
                }

                this.flushHasBegun = false;
            }

            /// <summary>
            /// Hashing function for this object
            /// </summary>
            public override int GetHashCode()
            {
                return base.GetHashCode() * name.ToCharArray().Length;
            }

            /// <summary>
            /// If this logger has data in it's buffer
            /// </summary>
            public bool BufferHasData { get { return content.ToString().Length > 0; } }


            public StringBuilder Content
            {
                get { return content; }
                private set { content = value; }
            }
        }
    }

}
