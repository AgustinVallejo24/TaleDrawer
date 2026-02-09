using System.Collections; // <--- Necesario para IEnumerable
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewSerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
{
    [SerializeField]
    private List<SerializableKeyValuePair> keyValuePairs = new List<SerializableKeyValuePair>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    // Renombré la struct interna para evitar confusión con System.Collections.Generic.KeyValuePair
    [System.Serializable]
    private struct SerializableKeyValuePair
    {
        public TKey Key;
        public TValue Value;
    }

    // --- Lógica de Serialización (Tus métodos OnAfter/OnBefore) ---

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

    public void OnBeforeSerialize() { /* Mantener vacío según tu lógica */ }

    // --- Implementación de IEnumerable ---

    // Este es el que usa el foreach (devuelve el par clave-valor estándar de C#)
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    // Requisito de la interfaz IEnumerable antigua
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // --- Otros métodos de Diccionario ---

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set
        {
            dictionary[key] = value;
            SyncList(key, value);
        }
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
        keyValuePairs.Add(new SerializableKeyValuePair { Key = key, Value = value });
    }

    private void SyncList(TKey key, TValue value)
    {
        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
            {
                keyValuePairs[i] = new SerializableKeyValuePair { Key = key, Value = value };
                return;
            }
        }
        keyValuePairs.Add(new SerializableKeyValuePair { Key = key, Value = value });
    }

    // ... (Mantén tus métodos Remove y ContainsKey igual)
}