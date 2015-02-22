using System;
using System.Collections.Generic;
using System.IO;
using Core.Initialization.INi;
using Microsoft.Xna.Framework;

namespace Core.Data
{
    /// <summary>
    /// Holds settings for the engine and handles IO
    /// Stores them in memory until disposed
    /// </summary>
    public class EngineConfigurationRepository : ConfigurationRepository
    {
        private Dictionary<string, IniFile> configs;
        private Dictionary<string, string> paths;
        private EngineContext context;

        /// <summary>
        /// Initializes a new instance of EngineConfigurationRepository
        /// </summary>
        /// <param name="game"></param>
        public EngineConfigurationRepository(EngineContext game) 
        {
            configs = new Dictionary<string, IniFile>();
            paths = new Dictionary<string, string>();
            context = game;
            game.Components.Add(this);
        }

        /// <summary>
        /// Initializes this repository
        /// </summary>
        public void Initialize()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory + "\\Config\\");
            string rootPath = Environment.CurrentDirectory;
            foreach (var file in info.EnumerateFiles("*.ini", SearchOption.AllDirectories))
            {
                var ini = IniFile.FromStream(new IniFileReader(TitleContainer.OpenStream(file.FullName.Substring(rootPath.Length + 1))));
                configs.Add(file.Name.Substring(0, file.Name.LastIndexOf(".")), ini);
                paths.Add(file.Name, file.FullName);
            }
        }

        /// <summary>
        /// Saves one of the ini files
        /// </summary>
        /// <param name="name"></param>
        public void Save(string name)
        {
            var ini = configs[name];

            var fileStream = new IniFileWriter(TitleContainer.OpenStream(paths[name]));

            fileStream.WriteIniFile(ini);
        }

        /// <summary>
        /// Loads INI files
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            var file = new FileInfo(path);

            if (file.Exists)
            {
                var ini = IniFile.FromStream(new IniFileReader(TitleContainer.OpenStream(file.FullName)));
                configs.Add(file.Name.Substring(0, file.Name.LastIndexOf(".", 0, StringComparison.OrdinalIgnoreCase)), ini);
                paths.Add(file.Name, file.FullName);
            }
        }

        /// <summary>
        /// Retrieves an ini file for you to use
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IniFile GetConfiguration(string name)
        {
            return configs[name];
        }

        /// <summary>
        /// Adds an ini file to the configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <param name="saveImmediately"></param>
        internal void AddConfiguration(IniFile configuration, string path, bool saveImmediately)
        {
            var file = new FileInfo(path);

            if(!file.Exists)
                file.Create();

            var ini = IniFile.FromStream(new IniFileReader(TitleContainer.OpenStream(file.FullName)));
            configs.Add(file.Name, ini);
            paths.Add(file.Name, file.FullName);
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Game engine reference
        /// </summary>
        public EngineContext Engine
        { get { return this.context; } }


        void ConfigurationRepository.AddConfiguration(IniFile configuration, string path, bool saveImmediately)
        {
            throw new NotImplementedException();
        }
    }
}
