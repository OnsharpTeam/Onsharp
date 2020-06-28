using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Onsharp.Native
{
     internal class ReturnData
    {
        internal bool IsFailed { get; }

        internal JToken this[string name] => _content[name];

        private readonly JObject _content;
        private readonly string _serializedContent;

        internal ReturnData(string data)
        {
            _serializedContent = data;
            if (string.IsNullOrEmpty(data))
            {
                IsFailed = true;
            }
            else
            {
                try
                {
                    IsFailed = false;
                    _content = JObject.Parse(data);
                }
                catch (Exception ex)
                {
                    IsFailed = true;
                    Bridge.Logger.Error(ex, "An error occurred while JSON deserialize on \"{DATA}\"", data);
                }
            }
        }

        internal T[] Values<T>(string name)
        {
            if (IsFailed) return default;
            JArray array = _content[name] as JArray;
            return array?.Select(token => token.ToObject<T>()).ToArray();
        }

        internal string[] ValuesAsStrings(string name)
        {
            object[] objects = Values<object>(name);
            if (objects != null)
            {
                string[] array = new string[objects.Length];
                for (int i = 0; i < objects.Length; i++)
                {
                    array[i] = objects[i].ToString();
                }

                return array;
            }

            return null;
        }

        internal T Value<T>(string name = null)
        {
            if (IsFailed) return default;
            return name == null ? _content.Value<T>() : _content.Value<T>(name);
        }

        internal T Value<T>(string name, out bool fail)
        {
            fail = false;
            if (IsFailed)
            {
                fail = true;
                return default;
            }
            return name == null ? _content.Value<T>() : _content.Value<T>(name);
        }

        internal T Deserialize<T>()
        {
            return JsonConvert.DeserializeObject<T>(_serializedContent);
        }
    }
}