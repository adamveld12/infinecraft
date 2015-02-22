using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Console
{
    public interface IInterpreter
    {
        /// <summary>
        /// Parses a command
        /// </summary>
        /// <returns>a commandlet from the message</returns>
        Commandlet ParseCommand(string command);

        /// <summary>
        /// 
        /// </summary>
        //string GetSplitCharacters
        //{
        //    get;
        //}
    }

    /// <summary>
    /// Where commands are processed and turned into commandlets.
    /// </summary>
    public class Interpreter : IInterpreter
    {
        private string[] splitCharacters;
        private string command;

        /// <summary>
        /// Initializes a new instance of Interpreter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="splitCharacters"></param>
        public Interpreter(string command, string[] splitCharacters)
        {
            this.command = command;
            this.splitCharacters = splitCharacters;
        }

        public Commandlet ParseCommand(string command)
        {
            var tokens = command.Split(splitCharacters, StringSplitOptions.RemoveEmptyEntries);

            var type = Type.GetType(tokens[0], false, true);

            return null;
        }
    }
}
