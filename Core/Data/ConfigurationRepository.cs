using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Initialization.INi;
using Microsoft.Xna.Framework;

namespace Core.Data
{
    /// <summary>
    /// A repository of engine settings
    /// </summary>
    public interface ConfigurationRepository : IGameComponent
    {
        /// <summary>
        /// Saves a configuration to the player's save directory
        /// </summary>
        /// <param name="name"></param>
        void Save(string name);

        /// <summary>
        /// Loads a configuration from the player's save directory
        /// </summary>
        /// <param name="path"></param>
        void Load(string path);

        /// <summary>
        /// Grabs a configuration and returns it
        /// </summary>
        /// <typeparam name="T">The type of the configuration</typeparam>
        /// <param name="name">the name of the file</param>
        /// <returns>the configuration</returns>
        IniFile GetConfiguration(string name);

        /// <summary>
        /// Adds a configuration to the repository
        /// </summary>
        /// <typeparam name="T">Type of configuration</typeparam>
        /// <param name="configuration">the configuration object</param>
        void AddConfiguration(IniFile configuration, string path, bool saveImmediately);
    }

    /// <summary>
    /// Holds actual configuration settings
    /// </summary>
    public abstract class Configuration
    {
        private IniFile file;
        private string name;

        /// <summary>
        /// Initializes a new instance of Configuration
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <param name="loadedFile"></param>
        internal Configuration(string friendlyName, IniFile loadedFile)
        {
            this.name = friendlyName;
            this.file = loadedFile;
        }

        /// <summary>
        /// Initializes a new instance of Configuration
        /// </summary>
        /// <param name="friendlyName"></param>
        internal Configuration(string friendlyName)
        {
            this.name = friendlyName;
            this.file = new IniFile();
        }

        /// <summary>
        /// Get one of the sections of this configuration
        /// </summary>
        /// <param name="sectionName">the name of the section to retrieve</param>
        /// <returns>an IniFileSection</returns>
        public IniFileSection this[string sectionName]
        {
            get { return this.file[sectionName]; }
        }
    }
}
