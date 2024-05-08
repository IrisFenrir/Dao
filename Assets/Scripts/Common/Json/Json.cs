using System.Collections.Generic;

namespace Dao
{
    public class Json
    {
        public enum DataType
        {
            None,
            Number,
            Boolean,
            String,
            Array,
            Object
        }

        private string m_value;
        private DataType m_type;
        private List<Json> m_array;
        private Dictionary<string, Json> m_map;

        public List<Json> Array => m_array;
        public Dictionary<string, Json> Map => m_map;

        public Json() { }
        public Json(DataType type)
        {
            SetDataType(type);
            if (type == DataType.Array)
                m_array = new List<Json>();
            else if (type == DataType.Object)
                m_map = new Dictionary<string, Json>();
        }

        public string GetValue()
        {
            return m_value;
        }

        private void SetValue(string value)
        {
            m_value = value;
        }

        public DataType GetDataType()
        {
            return m_type;
        }

        private void SetDataType(DataType type)
        {
            m_type = type;
        }

        public void Add(Json json)
        {
            SetDataType(DataType.Array);
            m_array ??= new List<Json>();
            m_array.Add(json);
        }

        #region 赋值和读取
        public static implicit operator Json(int value)
        {
            Json json = new Json();
            json.SetValue(value.ToString());
            json.SetDataType(DataType.Number);
            return json;
        }
        public static implicit operator int(Json json)
        {
            if (json == null || json.GetDataType() != DataType.Number)
                return default;
            return int.Parse(json.GetValue());
        }

        public static implicit operator Json(float value)
        {
            Json json = new Json();
            json.SetValue(value.ToString());
            json.SetDataType(DataType.Number);
            return json;
        }
        public static implicit operator float(Json json)
        {
            if (json == null || json.GetDataType() != DataType.Number)
                return default;
            return float.Parse(json.GetValue());
        }

        public static implicit operator Json(double value)
        {
            Json json = new Json();
            json.SetValue(value.ToString());
            json.SetDataType(DataType.Number);
            return json;
        }
        public static implicit operator double(Json json)
        {
            if (json == null || json.GetDataType() != DataType.Number)
                return default;
            return double.Parse(json.GetValue());
        }

        public static implicit operator Json(bool value)
        {
            Json json = new Json();
            json.SetValue(value.ToString().ToLower());
            json.SetDataType(DataType.Boolean);
            return json;
        }
        public static implicit operator bool(Json json)
        {
            if (json == null || json.GetDataType() != DataType.Boolean)
                return default;
            return bool.Parse(json.GetValue());
        }

        public static implicit operator Json(string value)
        {
            Json json = new Json();
            json.SetValue('"' + value + '"');
            json.SetDataType(DataType.String);
            return json;
        }
        public static implicit operator string(Json json)
        {
            if (json == null || json.GetDataType() != DataType.String)
                return default;
            return json.GetValue()[1..^1];
        }

        
        public Json this[int index]
        {
            get => m_array[index];
            set => m_array[index] = value;
        }

        public Json this[string key]
        {
            get => m_map[key];
            set
            {
                SetDataType(DataType.Object);
                m_map ??= new Dictionary<string, Json>();
                if (m_map.ContainsKey(key))
                    m_map[key] = value;
                else
                    m_map.Add(key, value);
            }
        }
        #endregion
    }
}
