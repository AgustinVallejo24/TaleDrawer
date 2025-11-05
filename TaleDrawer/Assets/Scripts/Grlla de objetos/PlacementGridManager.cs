using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEditor.PlayerSettings;

// Este script se encarga de la generación, actualización y chequeo de la grilla de colocación.
public class PlacementGridManager : MonoBehaviour
{
    // --- CONFIGURACIÓN DE LA GRILLA ---
    [Header("Configuración de Grilla")]
    [Tooltip("El tamaño de cada celda de la grilla (e.g., 1.0 para un tamaño estándar de celda).")]
    public float cellSize = 1f;

    [Tooltip("El número de celdas a generar en cada dirección (X e Y) alrededor del centro de la grilla/cámara.")]
    public int gridExtentx; // 15x15 = 225 puntos de grilla
    public int gridExtenty; 

    [Header("Configuración de Colisión")]
    [Tooltip("El radio del círculo de detección para chequear si el punto de grilla está libre.")]
    public float collisionCheckRadius = 0.1f;

    [Tooltip("Las capas que se consideran obstáculos (paredes, pisos, personaje principal, etc.).")]
    public LayerMask obstacleMask; // Configúralo en el Inspector de Unity

    [Tooltip("La distancia maxima desde la que puede colocarse en cada sector de la grilla")]
    [SerializeField] float _maxDistance = 2f;

    [SerializeField] GridPoint _pointPrefab;

    [SerializeField] Transform _initialTraget;
    [SerializeField] Camera _mainCam;
    
    // --- DATOS INTERNOS ---
    private List<GridPoint> placementPoints;
    private Vector2 gridCenter; // Generalmente, la posición de la cámara
    private bool isReady = false;

    // Obtener la instancia estática para que otros scripts la puedan acceder fácilmente
    public static PlacementGridManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gridExtentx = (int)Mathf.Round(_mainCam.orthographicSize * _mainCam.aspect);
            gridExtenty = (int)Mathf.Round(_mainCam.orthographicSize);
            gridCenter = _initialTraget.position;
            transform.position = gridCenter;
            placementPoints = new List<GridPoint>();
            GenerateGridPoints();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // El centro de la grilla debe seguir la cámara o el objeto de referencia de la zona actual
        // En este ejemplo, el centro es la posición del objeto que tiene este script (recomendado: la cámara principal).
        /*gridCenter = transform.position;*/
    }

    // Genera una lista inicial de puntos de colocación. Solo se hace una vez al inicio.
    private void GenerateGridPoints()
    {
        if(_pointPrefab != null)
        {
            // Limpiamos la lista por si acaso
            placementPoints.Clear();

            GameObject gridContainer = new GameObject("GridPointsContainer");
            gridContainer.transform.SetParent(this.transform);
            gridContainer.transform.localPosition = Vector3.zero;
            gridContainer.transform.localRotation = Quaternion.identity;

            // Iteramos a través de la extensión definida
            for (int x = -gridExtentx; x <= gridExtentx; x++)
            {
                for (int y = -gridExtenty; y <= gridExtenty; y++)
                {
                    // Calcula la posición relativa del punto
                    Vector2 localPoint = new Vector2(x * cellSize, y * cellSize);

                    GridPoint newPoint = Instantiate(_pointPrefab, gridContainer.transform);

                    newPoint.transform.localPosition = localPoint;

                    newPoint.SetAvailabilityColor(IsPointValid(CustomTools.ToVector2(newPoint.transform.position)));

                    placementPoints.Add(newPoint);
                }
            }
            isReady = true;
        }
        else
        {
            Debug.LogError("Te falta el prefab loco");
        }
        
       
    }

    // =========================================================================
    // FUNCIÓN CLAVE 1: Comprueba si un punto de la grilla está libre de obstáculos
    // =========================================================================
    public bool IsPointValid(Vector2 worldPosition)
    {
        // Physics2D.OverlapCircle comprueba si algún Collider en la capa obstacleMask
        // se superpone con un círculo en worldPosition con el radio collisionCheckRadius.

        // Si OverlapCircle devuelve 'null', significa que no hay colisiones (el punto es válido/libre).
        return Physics2D.OverlapCircle(worldPosition, collisionCheckRadius, obstacleMask) == null;
    }

    // =========================================================================
    // FUNCIÓN CLAVE 2: Encuentra el punto de colocación válido más cercano a una posición dada
    // =========================================================================
    /// <summary>
    /// El item 1 de la tupla es para saber si se encontro un punto cercano, el item 2 es la posición mas cercana, y el item 3 es el punto de la grilla.
    /// 
    /// El vector que se pide es el de la pocisión del objeto que se esta moviendo. El float que se le puede agregar es por si necesitamos una distancia maxima diferente.
    /// </summary>
    /// <param name="dragPosition"></param>
    /// <returns></returns>
    public Tuple<bool, Vector2, GridPoint> FindNearestValidPlacement(Vector2 dragPosition, float maxDist = 0)
    {
        if(maxDist == 0)
        {
            maxDist = _maxDistance;
        }

        GridPoint point = null;
        Tuple<bool, Vector2, GridPoint> bestPosition = Tuple.Create(false, dragPosition, point); // Valor por defecto si no se encuentra nada válido

        if (!isReady) return bestPosition; // Retorna la posición de arrastre si la grilla no está lista

        // Iteramos sobre todos los puntos de la grilla generados
        /*foreach (Vector2 localPoint in placementPoints)
        {
            // La posición real en el mundo es el centro de la grilla + el desplazamiento local
            Vector2 worldPoint = gridCenter + localPoint;

            // 1. CHEQUEO DE VALIDACIÓN: ¿Está libre de obstáculos?
            if (IsPointValid(worldPoint))
            {
                // 2. CHEQUEO DE DISTANCIA: Si es válido, ¿es el más cercano a la posición de arrastre actual?
                float distance = Vector3.Distance(worldPoint, dragPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestPosition = worldPoint;
                }
            }
        }*/

        /*var newList = placementPoints.Aggregate(FList.Create<Tuple<float,bool,Vector2, GridPoint>>(), (acum, current) =>
        {
            Vector2 pos = CustomTools.ToVector2(current.transform.position);
            
            if (IsPointValid(pos + gridCenter))
            {
                float dist = Vector2.Distance(pos, dragPosition);

                if(dist < maxDist)
                {
                    return acum + Tuple.Create(dist, true, pos, current);
                }
                else
                {
                    return acum;
                }
                
            }
            else
            {
                return acum;
            }
            

        }).OrderBy(x => x.Item1).Select(x => Tuple.Create(x.Item2, x.Item3, x.Item4));*/

        bestPosition = placementPoints.Aggregate(FList.Create<Tuple<bool, Vector2, GridPoint>>(), (acum, current) =>
        {
            Vector2 pos = CustomTools.ToVector2(current.transform.position);
            bool valid = IsPointValid(pos);

            return acum + Tuple.Create(valid, pos, current);
        }).OrderBy(x => Vector2.Distance(x.Item2, dragPosition)).First();

        /*if (newList.Any())
        {
            bestPosition = newList.First();
        } */       // Devolvemos el mejor punto encontrado. Si no encuentra ninguno, devuelve la posición de arrastre.
         
        return bestPosition;
    }

    private void MoveGrid(Transform pos)
    {
        transform.position = new Vector3(pos.position.x, pos.position.y, 0);
        gridCenter = transform.position;
    }

    public void RefreshGridAvailability()
    {
        foreach (GridPoint point in placementPoints)
        {
            point.SetAvailabilityColor(IsPointValid(point.transform.position));
        }
    }

    public void ChangeZone(Transform pos)
    {
        StartCoroutine(ChangeZ(pos));
    }
    public IEnumerator ChangeZ(Transform pos)
    {
        MoveGrid(pos);

        yield return new WaitForSeconds(0.1f);

        RefreshGridAvailability();
    }

    public void SetGridVisibility(bool visible)
    {
        foreach(GridPoint point in placementPoints)
        {
            point.SetVisibility(visible);
        }
    }

    // Opcional: Para depurar, dibuja los puntos de la grilla en el editor de Unity
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && placementPoints != null)
        {
            foreach (GridPoint localPoint in placementPoints)
            {
                Vector2 worldPoint = CustomTools.ToVector2(localPoint.transform.position);

                // Muestra en color verde los puntos válidos y en rojo los inválidos
                if (IsPointValid(worldPoint))
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireSphere(worldPoint, collisionCheckRadius);
            }
        }
    }
}

