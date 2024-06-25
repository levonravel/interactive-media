using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuantumInterface
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        // Save the dictionary to lists
        public void OnBeforeSerialize()
        {
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // Load dictionary from lists
        public void OnAfterDeserialize()
        {
            if (keys.Count != values.Count)
            {
                throw new Exception($"The number of keys ({keys.Count}) and values ({values.Count}) does not match.");
            }

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }
    }
}