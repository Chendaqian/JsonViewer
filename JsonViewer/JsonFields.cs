using System.Collections;
using System.Collections.Generic;

namespace Json.Viewer
{
    public class JsonFields : IEnumerable<JsonObject>
    {
        private readonly List<JsonObject> _fields = new List<JsonObject>();
        private readonly Dictionary<string, JsonObject> _fieldsById = new Dictionary<string, JsonObject>();
        private readonly JsonObject _parent;

        public JsonFields(JsonObject parent)
        {
            _parent = parent;
        }

        public IEnumerator<JsonObject> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(JsonObject field)
        {
            field.Parent = _parent;
            _fields.Add(field);
            _fieldsById[field.Id] = field;
            _parent.Modified();
        }

        public int Count => _fields.Count;

        public JsonObject this[int index] => _fields[index];

        public JsonObject this[string id]
        {
            get
            {
                JsonObject result;
                if (_fieldsById.TryGetValue(id, out result))
                    return result;
                return null;
            }
        }

        public bool ContainId(string id)
        {
            return _fieldsById.ContainsKey(id);
        }
    }
}