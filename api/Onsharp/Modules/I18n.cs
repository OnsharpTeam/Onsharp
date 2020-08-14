using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using Onsharp.IO;
using Onsharp.Plugins;

namespace Onsharp.Modules
{
    /// <summary>
    /// I18n is a utility class which allows multiple language support for your Onsharp plugins.
    /// I18n searches through projects which enabled this kind of module and registers every language automatically.
    /// Its possible to add new language codes later on manually.
    /// </summary>
    public class I18n
    {
        private readonly Dictionary<string, Dictionary<string, string>> _languagePacks;
        private readonly ILogger _logger;
        private readonly Plugin _plugin;
        private Dictionary<string, string> _currentLanguagePack;

        /// <summary>
        /// Gets the translated value of the given key back. If no language is selected or no value was found, the key is getting returned.
        /// </summary>
        /// <param name="key">The key of the wanted value</param>
        /// <param name="args">An argument list which will be passed into the value. The placeholder uses {0} beginning with 0 to pass the arguments</param>
        /// <returns>The translated message</returns>
        public string this[string key, params object[] args] => Get(key, args);

        internal I18n(ILogger logger, Assembly assembly, Plugin plugin)
        {
            _plugin = plugin;
            _logger = logger;
            _languagePacks = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                foreach (string resourceManageName in assembly.GetManifestResourceNames())
                {
                    ResourceManager manager = new ResourceManager(resourceManageName, assembly);
                    string typeName = resourceManageName.Substring(0, resourceManageName.IndexOf(".resource", StringComparison.Ordinal));
                    Type type = assembly.GetType(typeName);
                    if (type != null)
                    {
                        PropertyInfo[] resourceProperties =
                            type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (PropertyInfo resourceProperty in resourceProperties)
                        {
                            if (resourceProperty.PropertyType == typeof(string))
                            {
                                string code = resourceProperty.Name;
                                if (code.EndsWith("_lang"))
                                {
                                    code = code.Replace("_lang", "");
                                    if (resourceProperty.GetValue(null, null) is string value)
                                    {
                                        AddLanguage(code, value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while searching for languages in resources!");
            }
        }

        internal void Initialize()
        {
            if (_plugin.Meta.I18n == Mode.Configurable)
            {
                string path = Path.Combine(_plugin.Data.Directory.FullName, "langs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach (string code in _languagePacks.Keys)
                {
                    Dictionary<string, string> pack = _languagePacks[code];
                    StringBuilder content = new StringBuilder();
                    foreach (string key in pack.Keys)
                    {
                        content.AppendLine(key + "=" + pack[key]);
                    }
                    
                    File.WriteAllText(Path.Join(path, code + ".lang"), content.ToString());
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    if (Path.GetExtension(file) == ".lang")
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        AddLanguage(name, File.ReadAllText(file));
                    }
                }
            }
        }

        /// <summary>
        /// Removes the language pack by the given language code.
        /// </summary>
        /// <param name="code">The code of the language pack to be removed</param>
        public void RemoveLanguage(string code)
        {
            if (_languagePacks.ContainsKey(code))
            {
                _languagePacks.Remove(code);
            }
        }

        /// <summary>
        /// Manually adds the given content to the language list by the language code as key.
        /// </summary>
        /// <param name="code">The language code of the language (e.g. de_DE or en_US)</param>
        /// <param name="content">The content of the language pack in the simple format of KEY=VALUE</param>
        public void AddLanguage(string code, string content)
        {
            RemoveLanguage(code);
            if (!_languagePacks.ContainsKey(code))
            {
                try
                {
                    Dictionary<string, string> langPack = new Dictionary<string, string>();
                    foreach (string line in content.Split('\n'))
                    {
                        string[] parts = line.Trim().Split('=', 2);
                        langPack.Add(parts[0], parts[1]);
                    }

                    _languagePacks[code] = langPack;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred while parsing i18n language of code {CODE}!", code);
                }
            }
        }

        /// <summary>
        /// Selects the language pack by its given language code.
        /// </summary>
        /// <param name="code">The code of the language</param>
        public void SelectLanguage(string code)
        {
            _currentLanguagePack = _languagePacks.ContainsKey(code) ? _languagePacks[code] : null;
        }

        /// <summary>
        /// Gets the translated value of the given key back. If no language is selected or no value was found, the key is getting returned.
        /// </summary>
        /// <param name="key">The key of the wanted value</param>
        /// <param name="args">An argument list which will be passed into the value. The placeholder uses {0} beginning with 0 to pass the arguments</param>
        /// <returns>The translated message</returns>
        public string Get(string key, params object[] args)
        {
            return string.Format(GetRawValue(key), args);
        }

        private string GetRawValue(string key)
        {
            if (_currentLanguagePack == null) return key;
            if (!_currentLanguagePack.ContainsKey(key)) return key;
            return _currentLanguagePack[key];
        }

        /// <summary>
        /// The mode specifies how the i18n works in basic functionality.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// When set to disabled, the i18n module is disabled and the variable is null.
            /// </summary>
            Disabled,
            /// <summary>
            /// When set to internal, only internally configuration of the i18n module is allowed.
            /// </summary>
            Internal,
            /// <summary>
            /// When set to configurable, lang files are generated and the users are allowed to set them up.
            /// </summary>
            Configurable
        }
    }
}