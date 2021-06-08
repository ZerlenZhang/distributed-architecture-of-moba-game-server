using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

namespace ReadyGamerOne.Data
{
    /// <summary>
    /// 可序列化字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable,
        ISerializationCallbackReceiver
    {

        #region 构造函数

        public SerializableDictionary()
            : base()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region  IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>  
        /// 从对象的 XML 表示形式生成该对象  
        /// </summary>  
        /// <param name="reader"></param>  
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        /// <summary>  
        /// 将对象转换为其 XML 表示形式  
        /// </summary>  
        /// <param name="writer"></param>  
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion

        #region ISerializationCallbackReceiver

        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            _keys.Capacity = this.Count;
            _values.Capacity = this.Count;
            foreach (var kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            int count = Mathf.Min(_keys.Count, _values.Count);
            for (int i = 0; i < count; ++i)
            {
                this.Add(_keys[i], _values[i]);

            }
        }

        #endregion
    }
}
