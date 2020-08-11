using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using Onsharp.IO;

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
        private Dictionary<string, string> _currentLanguagePack;

        /// <summary>
        /// Gets the translated value of the given key back. If no language is selected or no value was found, the key is getting returned.
        /// </summary>
        /// <param name="key">The key of the wanted value</param>
        /// <param name="args">An argument list which will be passed into the value. The placeholder uses {0} beginning with 0 to pass the arguments</param>
        /// <returns>The translated message</returns>
        public string this[string key, params object[] args] => Get(key, args);

        internal I18n(ILogger logger, Assembly assembly)
        {
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

        /// <summary>
        /// Manually adds the given content to the language list by the language code as key.
        /// </summary>
        /// <param name="code">The language code of the language (e.g. de_DE or en_US)</param>
        /// <param name="content">The content of the language pack in the simple format of KEY=VALUE</param>
        public void AddLanguage(string code, string content)
        {
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
    }
}