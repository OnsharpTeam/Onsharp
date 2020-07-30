using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Onsharp.IO;

namespace Onsharp.Native
{
    /// <summary>
    /// The package provider offers the ability to generated LUA Onset packages before the server is started
    /// which than can be loaded and used at runtime.
    /// <br/><br/>
    /// Keep in mind: LUA scripts will be put into the root folder with the package JSON. Assets file can use subfolders.
    /// Please use <see cref="Path.Combine(string,string)"/> for cross platform support.
    /// </summary>
    public class PackageProvider
    {
        internal string Name { get; set; }
        
        internal string Author { get; set; }
        
        internal string Version { get; set; }

        private readonly Dictionary<Type, List<Tuple<string, byte[]>>> _data;
        
        /// <summary>
        /// The default constructor offering to pass or pass not a name.
        /// <param name="name">The name of the package. If not set, the meta display name will be used</param>
        /// <param name="version">The version of the package. If not set, the meta version will be used</param>
        /// <param name="author">The author of the package. If not set, the meta author will be used</param>
        /// </summary>
        public PackageProvider(string name = null, string version = null, string author = null)
        {
            Name = name;
            Version = version;
            Author = author;
            _data = new Dictionary<Type, List<Tuple<string, byte[]>>>
            {
                {Type.ServerScript, new List<Tuple<string, byte[]>>()},
                {Type.ClientScript, new List<Tuple<string, byte[]>>()},
                {Type.Asset, new List<Tuple<string, byte[]>>()}
            };
        }

        /// <summary>
        /// Exports the package to the folder on the given path.
        /// </summary>
        /// <param name="path">The path to the package folder</param>
        internal void Export(string path)
        {
            Initialize();
            FileSystemUtils.ClearFolder(path, true);
            Directory.CreateDirectory(path);
            Package package = new Package {Version = Version, Author = Author};
            foreach (Type type in _data.Keys)
            {
                foreach (Tuple<string, byte[]> data in _data[type])
                {
                    string newPath = Path.Combine(path, data.Item1);
                    if (type == Type.Asset)
                    {
                        package.Files.Add(data.Item1.Replace(Path.DirectorySeparatorChar, '/'));
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    }
                    else
                    {
                        (type == Type.ServerScript ? package.ServerScripts : package.ClientScripts).Add(data.Item1);
                    }
                    
                    File.WriteAllBytes(newPath, data.Item2);
                }
            }
            
            File.WriteAllText(Path.Combine(path, "package.json"), Json.ToJson(package, Json.Flag.Pretty));
        }

        /// <summary>
        /// Gets called when the package should be initialized before exporting it to the system. 
        /// </summary>
        protected virtual void Initialize()
        {
        }

        #region Adding Assets

        /// <summary>
        /// Adds an asset to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the asset. It must contain a valid extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="bytes">The data as a byte array</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddAsset(string name, byte[] bytes)
        {
            return AddData(Type.Asset, bytes, name);
        }

        /// <summary>
        /// Adds an asset to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the asset. It must contain a valid extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="content">The data as string</param>
        /// <param name="encoding">The charset which should be used for encoding</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddAsset(string name, string content, Encoding encoding)
        {
            return AddData(Type.Asset, encoding.GetBytes(content), name);
        }

        /// <summary>
        /// Adds an asset to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the asset. It must contain a valid extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="content">The data as string</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddAsset(string name, string content)
        {
            return AddAsset(name, content, Encoding.ASCII);
        }

        /// <summary>
        /// Downloads the data from the remote server from the given url and adds it as asset to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the asset. It must contain a valid extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="url">The url to the remote server</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddRemoteAsset(string name, string url)
        {
            using WebClient client = new WebClient();
            return AddAsset(name, client.DownloadData(url));
        }

        #endregion

        #region Adding Server Scripts

        /// <summary>
        /// Adds an server script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="bytes">The data as a byte array</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddServerScript(string name, byte[] bytes)
        {
            return AddData(Type.ServerScript, bytes, name);
        }

        /// <summary>
        /// Adds an server script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="content">The data as string</param>
        /// <param name="encoding">The charset which should be used for encoding</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddServerScript(string name, string content, Encoding encoding)
        {
            return AddData(Type.ServerScript, encoding.GetBytes(content), name);
        }

        /// <summary>
        /// Adds an server script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="content">The data as string</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddServerScript(string name, string content)
        {
            return AddServerScript(name, content, Encoding.ASCII);
        }

        /// <summary>
        /// Downloads the data from the remote server from the given url and adds it as server script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="url">The url to the remote server</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddRemoteServerScript(string name, string url)
        {
            using WebClient client = new WebClient();
            return AddServerScript(name, client.DownloadData(url));
        }

        #endregion

        #region Adding Client Scripts

        /// <summary>
        /// Adds an client script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="bytes">The data as a byte array</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddClientScript(string name, byte[] bytes)
        {
            return AddData(Type.ClientScript, bytes, name);
        }

        /// <summary>
        /// Adds an client script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="content">The data as string</param>
        /// <param name="encoding">The charset which should be used for encoding</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddClientScript(string name, string content, Encoding encoding)
        {
            return AddData(Type.ClientScript, encoding.GetBytes(content), name);
        }

        /// <summary>
        /// Adds an client script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="content">The data as string</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddClientScript(string name, string content)
        {
            return AddClientScript(name, content, Encoding.ASCII);
        }

        /// <summary>
        /// Downloads the data from the remote server from the given url and adds it as server script to the stack with the given name.
        /// </summary>
        /// <param name="name">The name of the server script. It must contain the LUA extension and can contain subdirectories. Use <see cref="Path.Combine(string,string)"/>to combine path cross platform</param>
        /// <param name="url">The url to the remote server</param>
        /// <returns>The current provider</returns>
        public PackageProvider AddRemoteClientScript(string name, string url)
        {
            using WebClient client = new WebClient();
            return AddClientScript(name, client.DownloadData(url));
        }

        #endregion

        /// <summary>
        /// Adds a data pair to the current stack of the provider.
        /// </summary>
        /// <param name="type">The type of the data pair</param>
        /// <param name="data">The data itself</param>
        /// <param name="ext">The name, extension and possible sub directories of the data</param>
        /// <returns>The current provider</returns>
        private PackageProvider AddData(Type type, byte[] data, string ext)
        {
            _data[type].Add(new Tuple<string, byte[]>(ext, data));
            return this;
        }

        /// <summary>
        /// The type is specifying the type of the value which will be added.
        /// </summary>
        private enum Type
        {
            /// <summary>
            /// A client LUA script
            /// </summary>
            ClientScript, 
            /// <summary>
            /// A server LUA script
            /// </summary>
            ServerScript, 
            /// <summary>
            /// Any other file which is getting transferred to the client
            /// </summary>
            Asset
        }

        /// <summary>
        /// A data class providing the needed information for the Onset server to load the package.
        /// </summary>
        private class Package
        {
            [JsonProperty("author")]
            public string Author { get; set; }
            
            [JsonProperty("version")]
            public string Version { get; set; }
            
            [JsonProperty("server_scripts")]
            public List<string> ServerScripts { get; set; } = new List<string>();
            
            [JsonProperty("client_scripts")]
            public List<string> ClientScripts { get; set; } = new List<string>();
            
            [JsonProperty("files")]
            public List<string> Files { get; set; } = new List<string>();
        }
    }
}