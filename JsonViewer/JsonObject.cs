using System.Diagnostics;

namespace Json.Viewer
{
    [DebuggerDisplay("Text = {" + nameof(Text) + "}")]
    public class JsonObject
    {
        private string _id;
        private object _value;
        private JsonType _jsonType;
        private readonly JsonFields _fields;
        private JsonObject _parent;
        private string _text;

        public JsonObject()
        {
            _fields = new JsonFields(this);
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public object Value
        {
            get => _value;
            set => _value = value;
        }

        public JsonType JsonType
        {
            get => _jsonType;
            set => _jsonType = value;
        }

        public JsonObject Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public string Text
        {
            get
            {
                if (_text == null)
                {
                    if (JsonType == JsonType.Value)
                    {
                        string val = null; // (Value == null ? "<null>" : Value.ToString());
                        if (Value == null)
                            val = "<null>";
                        else
                        {
                            string s = Value.ToString();
                            if (s.Length > 5000)
                                s = s.Substring(0, 5000);
                            val = s;
                        }
                        if (Value is string)
                            val = "\"" + val + "\"";
                        _text = string.Format("{0} : {1}", Id, val);
                    }
                    else
                        _text = Id;
                }
                return _text;
            }
        }

        public JsonFields Fields => _fields;

        internal void Modified()
        {
            _text = null;
        }

        public bool ContainsFields(params string[] ids)
        {
            foreach (string s in ids)
            {
                if (!_fields.ContainId(s))
                    return false;
            }
            return true;
        }

        public bool ContainsField(string id, JsonType type)
        {
            JsonObject field = Fields[id];
            return field != null && field.JsonType == type;
        }
    }
}