using System.Collections.Generic;
using UnityEngine;

// 1. Agrega la interfaz ISerializationCallbackReceiver
[System.Serializable]
public class NewSerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    // Esta es la lista que Unity puede serializar y mostrar en el inspector.
    [SerializeField]
    private List<KeyValuePair> keyValuePairs = new List<KeyValuePair>();

    // Este es el diccionario real, que no se serializa pero usamos para acceso r�pido.
    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    // Estructura interna para guardar los pares clave-valor
    [System.Serializable]
    private struct KeyValuePair
    {
        public TKey Key;
        public TValue Value;
    }

    // --> M�TODO PARA CARGAR
    // Se ejecuta despu�s de que Unity ha deserializado los datos (ha llenado la lista)
    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        foreach (var pair in keyValuePairs)
        {
            // Ten cuidado con claves duplicadas en el inspector
            if (!dictionary.ContainsKey(pair.Key))
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    // --> M�TODO PARA GUARDAR
    // Se ejecuta justo antes de que Unity vaya a serializar el objeto (guardar la escena/prefab)
    public void OnBeforeSerialize()
    {
        keyValuePairs.Clear();
        foreach (var pair in dictionary)
        {
            keyValuePairs.Add(new KeyValuePair { Key = pair.Key, Value = pair.Value });
        }
    }

    // --- M�todos p�blicos para que la clase se comporte como un diccionario ---

    // Indexador para acceder a los elementos como miDiccionario[clave]
    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
    }

    public bool Remove(TKey key)
    {
        return dictionary.Remove(key);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    // Y cualquier otro m�todo que necesites (TryGetValue, Count, etc.)
    public int Count => dictionary.Count;
}
