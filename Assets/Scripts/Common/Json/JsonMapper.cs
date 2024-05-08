using System.Text;

namespace Dao
{
    public static class JsonMapper
    {
        private static int m_readIndex;

        public static Json StringToJson(string str)
        {
            m_readIndex = 0;
            Json json = ProcessString(str);
            SkipWhiteSpace(str);
            if (m_readIndex != str.Length)
                return null;
            return json;
        }

        public static string JsonToString(Json json)
        {
            if (json == null) return null;
            return json.GetDataType() switch
            {
                Json.DataType.None or Json.DataType.Number or Json.DataType.Boolean or Json.DataType.String => json.GetValue(),
                Json.DataType.Array => ConvertArrayToString(json),
                Json.DataType.Object => ConvertObjectToString(json),
                _ => null,
            };
        }

        

        private static Json ProcessString(string str)
        {
            SkipWhiteSpace(str);
            if (m_readIndex == str.Length) return null;

            char currentChar = str[m_readIndex];
            if (char.IsDigit(currentChar) || currentChar == '-')
                return ConvertToNumber(str);
            else if (currentChar == 't' || currentChar == 'f')
                return ConvertToBoolean(str);
            else if (currentChar == '"')
                return ConvertToString(str);
            else if (currentChar == '[')
                return ConvertToArray(str);
            else if (currentChar == '{')
                return ConvertToObject(str);

            return null;
        }

        private static void SkipWhiteSpace(string str)
        {
            while (m_readIndex < str.Length && char.IsWhiteSpace(str[m_readIndex]))
                m_readIndex++;
        }

        private static Json ConvertToNumber(string str)
        {
            int start = m_readIndex;
            while (m_readIndex < str.Length && (char.IsDigit(str[m_readIndex]) || str[m_readIndex] == '-' || str[m_readIndex] == '+' || str[m_readIndex] == 'e' || str[m_readIndex] == 'E' || str[m_readIndex] == '.'))
                m_readIndex++;
            if (double.TryParse(str[start..m_readIndex], out double value))
                return value;
            return null;
        }

        private static Json ConvertToBoolean(string str)
        {
            if (m_readIndex + 4 <= str.Length && str.Substring(m_readIndex, 4) == "true")
            {
                m_readIndex += 4;
                return true;
            }
            else if (m_readIndex + 5 <= str.Length && str.Substring(m_readIndex, 5) == "false")
            {
                m_readIndex += 5;
                return false;
            }
            return null;
        }

        private static Json ConvertToString(string str)
        {
            int start = ++m_readIndex;
            while (str[m_readIndex] != '"')
            {
                if (m_readIndex < str.Length)
                    m_readIndex++;
                else
                    return null;
            }
            return str[start..m_readIndex++];
        }

        private static Json ConvertToArray(string str)
        {
            m_readIndex++;
            SkipWhiteSpace(str);
            if (m_readIndex == str.Length) return null;

            Json json = new Json(Json.DataType.Array);
            Json sub;
            while (str[m_readIndex] != ']')
            {
                sub = ProcessString(str);
                if (sub == null) return null;
                json.Add(sub);
                SkipWhiteSpace(str);
                if (m_readIndex == str.Length) return null;
                if (str[m_readIndex] == ',')
                    m_readIndex++;
            }
            m_readIndex++;
            return json;
        }

        private static Json ConvertToObject(string str)
        {
            m_readIndex++;
            SkipWhiteSpace(str);
            if (m_readIndex == str.Length) return null;

            Json json = new Json();
            Json sub;
            while (str[m_readIndex] != '}')
            {
                if (str[m_readIndex] != '"')
                    return null;
                string key = ConvertToString(str);
                SkipWhiteSpace(str);
                if (m_readIndex == str.Length) return null;
                if (str[m_readIndex] != ':')
                    return null;
                m_readIndex++;
                sub = ProcessString(str);
                if (sub == null)
                    return null;
                json[key] = sub;
                SkipWhiteSpace(str);
                if (m_readIndex == str.Length) return null;
                if (str[m_readIndex] == ',')
                    m_readIndex++;
                SkipWhiteSpace(str);
            }
            m_readIndex++;
            return json;
        }

        private static string ConvertArrayToString(Json json)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            for (int i = 0; i < json.Array.Count; i++)
            {
                builder.Append(JsonToString(json[i]));
                if (i < json.Array.Count - 1)
                    builder.Append(',');
            }
            builder.Append(']');
            return builder.ToString();
        }

        private static string ConvertObjectToString(Json json)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('{');
            int i = 0;
            foreach (var item in json.Map)
            {
                builder.Append('"');
                builder.Append(item.Key);
                builder.Append('"');
                builder.Append(':');
                builder.Append(JsonToString(item.Value));
                if (i < json.Map.Count - 1)
                {
                    builder.Append(',');
                    i++;
                }
            }
            builder.Append('}');
            return builder.ToString();
        }
    }
}
