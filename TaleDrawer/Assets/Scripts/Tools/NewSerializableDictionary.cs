using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewSerializableDictionary<TKey, TValue>
    : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
{
    [SerializeField]
    private List<SerializableKeyValuePair> keyValuePairs = new List<SerializableKeyValuePair>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    [System.Serializable]
    private struct SerializableKeyValuePair
    {
        public TKey Key;
        public TValue Value;
    }

    // --- Serialización ---

    public void OnAfterDeserialize()
    {
        dictionary.Clear();

        foreach (var pair in keyValuePairs)
        {
            if (pair.Key != null && !dictionary.ContainsKey(pair.Key))
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        // Intencionalmente vacío
    }

    // --- IEnumerable ---

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // --- Acceso tipo diccionario ---

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set
        {
            dictionary[key] = value;
            SyncList(key, value);
        }
    }

    public int Count => dictionary.Count;

    public ICollection<TKey> Keys => dictionary.Keys;
    public ICollection<TValue> Values => dictionary.Values;

    // --- Métodos básicos ---

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
        keyValuePairs.Add(new SerializableKeyValuePair { Key = key, Value = value });
    }

    public bool Remove(TKey key)
    {
        bool removedFromDict = dictionary.Remove(key);

        if (!removedFromDict)
            return false;

        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
            {
                keyValuePairs.RemoveAt(i);
                break;
            }
        }

        return true;
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public void Clear()
    {
        dictionary.Clear();
        keyValuePairs.Clear();
    }

    // --- Sincronización interna ---

    private void SyncList(TKey key, TValue value)
    {
        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
            {
                keyValuePairs[i] = new SerializableKeyValuePair
                {
                    Key = key,
                    Value = value
                };
                return;
            }
        }

        keyValuePairs.Add(new SerializableKeyValuePair { Key = key, Value = value });
    }
}
