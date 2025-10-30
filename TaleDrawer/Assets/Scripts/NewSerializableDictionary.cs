using System.Collections.Generic;
using UnityEngine;

// 1. Agrega la interfaz ISerializationCallbackReceiver
[System.Serializable]
public class NewSerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    // Esta es la lista que Unity puede serializar y mostrar en el inspector.
    // Es la "fuente de verdad" para la serialización.
    [SerializeField]
    private List<KeyValuePair> keyValuePairs = new List<KeyValuePair>();

    // Este es el diccionario real, que no se serializa pero usamos para acceso rápido.
    // Es un "caché" de la lista.
    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    // Estructura interna para guardar los pares clave-valor
    [System.Serializable]
    private struct KeyValuePair
    {
        public TKey Key;
        public TValue Value;
    }

    // --> MÉTODO PARA CARGAR
    // Se ejecuta después de que Unity ha deserializado los datos (ha llenado la lista)
    // Esto está CORRECTO.
    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        foreach (var pair in keyValuePairs)
        {
            // Ten cuidado con claves duplicadas en el inspector
            if (pair.Key != null && !dictionary.ContainsKey(pair.Key))
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    // --> MÉTODO PARA GUARDAR
    // ¡AQUÍ ESTABA EL PROBLEMA!
    // Lo vaciamos para permitir que el Inspector guarde sus cambios en 'keyValuePairs'
    public void OnBeforeSerialize()
    {
        // NO HACER NADA AQUÍ.
        // Si modificamos 'keyValuePairs' aquí, borraremos los cambios
        // hechos en el Inspector antes de que se puedan guardar.

        // La ÚNICA vez que necesitarías código aquí es si quieres
        // detectar duplicados en el Inspector y limpiarlos *antes* de guardar.
        // Por ahora, lo dejamos vacío por simplicidad.
    }

    // --- Métodos públicos para que la clase se comporte como un diccionario ---
    // AHORA DEBEN MODIFICAR AMBAS COLECCIONES (lista y diccionario)

    // Indexador para acceder a los elementos como miDiccionario[clave]
    public TValue this[TKey key]
    {
        get => dictionary[key];
        set
        {
            dictionary[key] = value;
            // Sincronizar la lista (lento, pero necesario para guardar)
            bool found = false;
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                // Usamos EqualityComparer para tipos genéricos
                if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
                {
                    keyValuePairs[i] = new KeyValuePair { Key = key, Value = value };
                    found = true;
                    break;
                }
            }
            // Si no existía, lo añadimos (comportamiento del indexador de Dictionary)
            if (!found)
            {
                keyValuePairs.Add(new KeyValuePair { Key = key, Value = value });
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value); // Esto lanza excepción si la clave ya existe (correcto)
        keyValuePairs.Add(new KeyValuePair { Key = key, Value = value }); // Sincronizar lista
    }

    public bool Remove(TKey key)
    {
        if (dictionary.Remove(key))
        {
            // Sincronizar lista (lento)
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
                {
                    keyValuePairs.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }
}