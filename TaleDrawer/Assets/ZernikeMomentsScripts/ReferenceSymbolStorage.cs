using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
public static class ReferenceSymbolStorage
{
    [Serializable]
    private class ReferenceSymbolWrapper
    {
        public List<ReferenceSymbolGroup> symbols;
    }

    // JSON por defecto para crear el archivo si no existe
    private static readonly string DEFAULT_JSON_CONTENT = JsonUtility.ToJson(new ReferenceSymbolWrapper { symbols = new List<ReferenceSymbolGroup>() }, true);
    
    private static readonly string FILENAME = "symbols.json";
    private const string FILENAME_NO_EXTENSION = "symbols";

#if UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SyncFilesystem();
#endif
    private static string GetFilePath()
    {
        string directoryPath = Application.dataPath + "/Resources";
        // Asegurar que la carpeta exista antes de intentar cualquier I/O
        try { Directory.CreateDirectory(directoryPath); }
        catch (IOException) { /* Ignorar si ya existe */ }

        return Path.Combine(directoryPath, FILENAME);
    }

    // ----------------------------------------------------------------------------------
    //  FUNCIÓN DE GUARDADO (Ahora simplificada)
    // ----------------------------------------------------------------------------------
    public static void SaveSymbols(List<ReferenceSymbolGroup> symbols)
    {
        string filePath = GetFilePath();

        ReferenceSymbolWrapper wrapper = new ReferenceSymbolWrapper { symbols = symbols };
        string json = JsonUtility.ToJson(wrapper, true);

        // Escritura
        File.WriteAllText(filePath, json);

        // SINCRONIZACIÓN OBLIGATORIA
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

    }

    // ----------------------------------------------------------------------------------
    //  FUNCIÓN DE CARGA/CREACIÓN (El reemplazo del File.Exists)
    // ----------------------------------------------------------------------------------
    public static List<ReferenceSymbolGroup> LoadSymbols()
    {
        // Intentar cargar el JSON desde Resources
        TextAsset textAsset = Resources.Load<TextAsset>(FILENAME_NO_EXTENSION);

        if (textAsset == null)
        {
            Debug.LogError($"No se encontró el archivo {FILENAME} en Resources.");
            return new List<ReferenceSymbolGroup>();
        }

        string json = textAsset.text;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("El archivo JSON está vacío.");
            return new List<ReferenceSymbolGroup>();
        }

        try
        {
            ReferenceSymbolWrapper wrapper = JsonUtility.FromJson<ReferenceSymbolWrapper>(json);
            return wrapper?.symbols ?? new List<ReferenceSymbolGroup>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al deserializar JSON: {ex.Message}");
            return new List<ReferenceSymbolGroup>();
        }
    }

    // ----------------------------------------------------------------------------------
    //  FUNCIÓN DE AÑADIR (Ajustada para usar la nueva LoadSymbols y SaveSymbols)
    // ----------------------------------------------------------------------------------
    public static void AppendSymbol(ReferenceSymbol newSymbol)
    {
        // 1. Cargar el JSON (que lo crea si no existe)
        List<ReferenceSymbolGroup> current = LoadSymbols();

        // 2. Lógica de modificación (Mantenida sin cambios)
        ReferenceSymbolGroup existingGroup = current.FirstOrDefault(x => string.Equals(x.symbolName, newSymbol.symbolName, StringComparison.OrdinalIgnoreCase));

        if (existingGroup.symbolName != null)
        {
            existingGroup.symbols.Add(newSymbol);
        }
        else
        {
            ReferenceSymbolGroup newGroup = new ReferenceSymbolGroup
            {
                symbolName = newSymbol.symbolName,
                symbols = new List<ReferenceSymbol> { newSymbol }
            };
            current.Add(newGroup);
        }

        // 3. Guardar el contenido modificado
        SaveSymbols(current);
        Debug.Log("Appended symbol: " + newSymbol.symbolName);
    }

    public static bool JsonExistsInResources()
    {
        TextAsset asset = Resources.Load<TextAsset>(FILENAME_NO_EXTENSION);
        return asset != null;
    }
    // NOTAS: Las funciones LoadFromResources y LoadSymbols(string path) original se han fusionado o eliminado por simplicidad.
    // La función LoadOrCreateJson fue ELIMINADA y su lógica integrada en LoadSymbols.
}