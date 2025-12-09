using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Collections;
using System;
//using System.Diagnostics;
using UnityEngine.UI;
public class DrawingTest : MonoBehaviour
{


    [SerializeField] LineRenderer lRPrefab;
    [SerializeField] LineRenderer lRPrefabRT;
    [SerializeField] RenderTexture renderTexture;
    public TMP_Text resultText;
    public ZernikeManager zRecognizer;

    public Camera cam;
    private List<Vector2> currentPoints = new List<Vector2>(); 
    private List<Vector2> currentPointsCentroid = new List<Vector2>();
    private List<Vector2> currentStrokePoints = new List<Vector2>();
    public bool isDrawing = false;

    private List<LineRenderer> _lineRenderers = new List<LineRenderer>(0);
    private List<LineRenderer> _renderTexturelineRenderers = new List<LineRenderer>(0);
    private LineRenderer currentLR;
    List<List<Vector2>> listaDeListas = new List<List<Vector2>>();
    private int _linerendererIndex;
    private List<int> _strokesPointsCount = new List<int>();

    [SerializeField] LayerMask _uiMask;
    public bool startDrawing;
    public bool startTimer = false;
    public float recognitionTime;
    float timer;
    public Image fillImage;

    public Vector2 currentCentroid;
    private void Start()
    {
        //cam = Camera.main;
        var lineR = Instantiate(lRPrefab, transform);
        _lineRenderers.Add(lineR);
        currentLR = lineR;
    }
    void Update()
    {

        if (startTimer)
        {
            if (timer < recognitionTime)
            {
                timer += Time.unscaledDeltaTime * 10;
                fillImage.fillAmount = timer / recognitionTime;
            }
            else
            {
                timer = 0;
                startTimer = false;
                fillImage.fillAmount = 0;
                OnConfirmDrawing();
            }
        }


    }
    public void OnDraw(Vector2 position)
    {
        // Debug.Log("TOCOBOTON");

        if (isDrawing)
        {
            Vector2 pos = cam.ScreenToWorldPoint(position);
            if (currentPoints.Count == 0 || Vector2.Distance(currentPoints[^1], pos) > 0.001f)
            {
                AddSmoothedPoint(pos);
                currentPointsCentroid.Add(Camera.main.ScreenToWorldPoint(position));

            }

        }
        else
        {
            if (CustomTools.IsTouchOverUI(position))
            {
                //Debug.Log("TOCOBOTON");
                return;
            }

            startTimer = false;
            timer = 0;
            fillImage.fillAmount = 0;


            startDrawing = true;
            if (GameManager.instance != null && GameManager.instance.currentState != SceneStates.Drawing)
            {
                GameManager.instance.StateChanger(SceneStates.Drawing);
            }
            isDrawing = true;
            currentStrokePoints.Clear();
        }
    }
    public void OnEndDrag()
    {
        if (isDrawing)
        {
            isDrawing = false;
            startDrawing = false;
            startTimer = true;

            if (_linerendererIndex >= _lineRenderers.Count) return;


            if (GetLineLength(_lineRenderers[_linerendererIndex]) < 1.5f)
            {
                // listaDeListas[linerendererIndex] = GestureProcessor.Normalize(listaDeListas[linerendererIndex]);
                foreach (var item in currentStrokePoints)
                {
                    if (currentPoints.Contains(item))
                    {
                        currentPoints.Remove(item);
                    }
                }
                currentStrokePoints.Clear();
                _lineRenderers[_linerendererIndex].positionCount = 0;
                return;
            }
            listaDeListas.Add(new List<Vector2>());
             listaDeListas[_linerendererIndex] = GestureProcessor.Normalize(listaDeListas[_linerendererIndex]);
            _strokesPointsCount.Add(currentStrokePoints.Count);
            currentStrokePoints.Clear();
            var lineR = Instantiate(lRPrefab, transform);
            _lineRenderers.Add(lineR);
            _linerendererIndex++;
            currentLR = lineR;
            //if (_linerendererIndex < _lineRenderers.Count)
            //{
            //    _linerendererIndex++;
            //}

        }
    }


    bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current != null &&
               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }



    void AddSmoothedPoint(Vector3 newPoint)
    {
        currentStrokePoints.Add(newPoint);
        currentPoints.Add(newPoint);
        // listaDeListas[linerendererIndex].Add(newPoint);
        int count = currentStrokePoints.Count;

        if (count == 1)
        {
            // primer punto del trazo inicializamos el LineRenderer

            _lineRenderers[_linerendererIndex].positionCount = 1;
            _lineRenderers[_linerendererIndex].SetPosition(0, newPoint);
            return;
        }

        if (count < 4)
        {
            // si todavía no hay suficientes puntos para interpolar, solo agrego directo
            _lineRenderers[_linerendererIndex].positionCount++;
            _lineRenderers[_linerendererIndex].SetPosition(_lineRenderers[_linerendererIndex].positionCount - 1, newPoint);
            return;
        }

        // últimos 4 puntos para Catmull-Rom
        Vector3 p0 = currentStrokePoints[count - 4];
        Vector3 p1 = currentStrokePoints[count - 3];
        Vector3 p2 = currentStrokePoints[count - 2];
        Vector3 p3 = currentStrokePoints[count - 1];

        int subdivisions = 4; // más subdivisiones = más suave
        for (int j = 1; j <= subdivisions; j++) // arranco en 1 para no repetir p1
        {
            float t = j / (float)subdivisions;
            Vector3 point = 0.5f * (
                (2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
            );

            _lineRenderers[_linerendererIndex].positionCount++;
            _lineRenderers[_linerendererIndex].SetPosition(_lineRenderers[_linerendererIndex].positionCount - 1, point);

        }


    }




    public ReferenceSymbol ReturnNewSymbol(string symbolName, bool shouldRender)
    {
        if (currentPoints.Any())
        {
            var normalizedPositions = DrawNormalizer.Normalize(currentPoints);
            for (int i = 0; i < _strokesPointsCount.Count; i++)
            {
                if (shouldRender)
                {
                    _renderTexturelineRenderers.Add(Instantiate(lRPrefabRT, transform));
                    _renderTexturelineRenderers[i].positionCount = _strokesPointsCount[i];
                }

                if (_strokesPointsCount[i] == 0) continue;
                for (int j = 0; j < _strokesPointsCount[i]; j++)
                {
                    listaDeListas[i].Add(normalizedPositions[j]);
                    if (shouldRender)
                    {
                        _renderTexturelineRenderers[i].SetPosition(j, normalizedPositions[j]);
                    }

                }
                normalizedPositions = normalizedPositions.Skip(_strokesPointsCount[i]).ToList();
            }



            return zRecognizer.ReturnNewSymbol(listaDeListas, _lineRenderers.Count - 1, symbolName, "symbolID");
        }
        else
        {
            return null;
        }
    }
    float GetLineLength(LineRenderer line)
    {
        float length = 0f;
        for (int i = 1; i < line.positionCount; i++)
        {
            length += Vector3.Distance(line.GetPosition(i - 1), line.GetPosition(i));
        }
        return length;
    }
    public void OnConfirmDrawing()
    {

        if (currentPoints.Any())
        {
            currentCentroid = CalcularCentroGeometrico(currentPointsCentroid);
            var normalizedPositions = DrawNormalizer.Normalize(currentPoints);
            for (int i = 0; i < _strokesPointsCount.Count; i++)
            {
                if (_strokesPointsCount[i] == 0) continue;
                for (int j = 0; j < _strokesPointsCount[i]; j++)
                {
                    listaDeListas[i].Add(normalizedPositions[j]);

                }
                normalizedPositions = normalizedPositions.Skip(_strokesPointsCount[i]).ToList();
            }

            

            zRecognizer.OnDrawingFinished(listaDeListas, _lineRenderers.Count - 1); ;
            currentPoints.Clear();
            _strokesPointsCount.Clear();
            foreach (var item in listaDeListas)
            {
                item.Clear();
            }
            _linerendererIndex = 0;
            _lineRenderers[0].positionCount = 0;
            var count = _lineRenderers.Count;
            for (int i = 1; i < count; i++)
            {
                Destroy(_lineRenderers[1].gameObject);
                _lineRenderers.RemoveAt(1);
            }

            currentLR = _lineRenderers[0];
        }
        else
        {
            resultText.text = "Trazo demasiado corto";
            currentPoints.Clear();
            _strokesPointsCount.Clear();
            foreach (var item in listaDeListas)
            {
                item.Clear();
            }
            _linerendererIndex = 0;
            _lineRenderers[0].positionCount = 0;
            foreach (var item in _lineRenderers.Skip(1))
            {
                _lineRenderers.Remove(item);
                Destroy(item.gameObject);
            }
            currentLR = _lineRenderers[0];
        }
    }

   
    public Vector2 CalcularCentroGeometrico(List<Vector2> puntos)
    {
        if (puntos == null || puntos.Count == 0)
        {
            return Vector2.zero; // O manejar el error apropiadamente
        }

        // 1. Inicializar los valores de Bounding Box.
        // Se inicializan con el primer punto.
        Vector2 minBounds = puntos[0];
        Vector2 maxBounds = puntos[0];

        // 2. Iterar sobre todos los puntos para encontrar el min y max.
        for (int i = 1; i < puntos.Count; i++)
        {
            Vector2 p = puntos[i];

            // Encontrar el valor mínimo para cada eje
            minBounds.x = Mathf.Min(minBounds.x, p.x);
            minBounds.y = Mathf.Min(minBounds.y, p.y);

            // Encontrar el valor máximo para cada eje
            maxBounds.x = Mathf.Max(maxBounds.x, p.x);
            maxBounds.y = Mathf.Max(maxBounds.y, p.y);

        }

        // 3. El centro es el punto medio entre min y max.
        // Vector3.Lerp(a, b, 0.5f) también es una forma limpia de hacer esto.
        Vector2 centroGeometrico = (minBounds + maxBounds) / 2f;
        DebuggearBoundingBox(minBounds, maxBounds, 0f);
        return centroGeometrico;
    }

    public void DebuggearBoundingBox(Vector2 minBounds, Vector2 maxBounds, float zAltura = 0f)
    {
        // Convertir los límites 2D a esquinas 3D para dibujar
        Vector3 cornerA = new Vector3(minBounds.x, minBounds.y, zAltura); // Abajo-Izquierda
        Vector3 cornerB = new Vector3(maxBounds.x, minBounds.y, zAltura); // Abajo-Derecha
        Vector3 cornerC = new Vector3(maxBounds.x, maxBounds.y, zAltura); // Arriba-Derecha
        Vector3 cornerD = new Vector3(minBounds.x, maxBounds.y, zAltura); // Arriba-Izquierda

        // Dibujar las 4 líneas del rectángulo

        // Línea 1: Abajo-Izquierda (A) a Abajo-Derecha (B)
        Debug.DrawLine(cornerA, cornerB, Color.yellow, 3f, false);

        // Línea 2: Abajo-Derecha (B) a Arriba-Derecha (C)
        Debug.DrawLine(cornerB, cornerC, Color.yellow, 3f, false);

        // Línea 3: Arriba-Derecha (C) a Arriba-Izquierda (D)
        Debug.DrawLine(cornerC, cornerD, Color.yellow, 3f, false);

        // Línea 4: Arriba-Izquierda (D) a Abajo-Izquierda (A)
        Debug.DrawLine(cornerD, cornerA, Color.yellow, 3f, false);

        // Opcional: Dibujar el Centroide Calculado (el centro del cuadrado)
        Vector2 centro = (minBounds + maxBounds) / 2f;
        Vector3 centro3D = new Vector3(centro.x, centro.y, zAltura);

        // Dibujar una "cruz" en el centroide
        Debug.DrawLine(centro3D - Vector3.right * 0.1f, centro3D + Vector3.right * 0.1f, Color.red, 0f, false);
        Debug.DrawLine(centro3D - Vector3.up * 0.1f, centro3D + Vector3.up * 0.1f, Color.red, 0f, false);
    }
    Vector3 CalcularCentroBoundingBox(List<LineRenderer> lineas)
    {
        bool any = false;
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity, minZ = float.PositiveInfinity;
        float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity, maxZ = float.NegativeInfinity;

        foreach (var lr in lineas)
        {
            if (lr == null) continue;
            int n = lr.positionCount;
            if (n == 0) continue;

            Vector3[] pts = new Vector3[n];
            lr.GetPositions(pts);
            for (int i = 0; i < n; i++)
            {
                Vector3 p = lr.useWorldSpace ? pts[i] : lr.transform.TransformPoint(pts[i]);
                any = true;
                if (p.x < minX) minX = p.x;
                if (p.y < minY) minY = p.y;
                if (p.z < minZ) minZ = p.z;
                if (p.x > maxX) maxX = p.x;
                if (p.y > maxY) maxY = p.y;
                if (p.z > maxZ) maxZ = p.z;
            }
        }

        if (!any) return Vector3.zero;
        return new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, (minZ + maxZ) / 2f);
    }

    public  Vector3 CentroPonderadoPorLongitud(List<LineRenderer> lineRenderers)
    {
        Vector3 sumMidTimesLen = Vector3.zero;
        float totalLen = 0f;

        foreach (var lr in lineRenderers)
        {
            if (lr == null) continue;
            int n = lr.positionCount;
            if (n < 2) continue;

            Vector3[] pts = new Vector3[n];
            lr.GetPositions(pts);
            for (int i = 0; i < n - 1; i++)
            {
                Vector3 p0 = lr.useWorldSpace ? pts[i] : lr.transform.TransformPoint(pts[i]);
                Vector3 p1 = lr.useWorldSpace ? pts[i + 1] : lr.transform.TransformPoint(pts[i + 1]);
                float segLen = Vector3.Distance(p0, p1);
                if (segLen <= 0f) continue;
                Vector3 mid = (p0 + p1) * 0.5f;
                sumMidTimesLen += mid * segLen;
                totalLen += segLen;
            }
        }

        if (totalLen == 0f) return Vector3.zero;
        return sumMidTimesLen / totalLen;
    }
    public void ClearAllLineRenderers(bool clearRenderDraw)
    {
        currentPoints.Clear();
        _strokesPointsCount.Clear();
        foreach (var item in listaDeListas)
        {
            item.Clear();
        }
        _linerendererIndex = 0;
        _lineRenderers[0].positionCount = 0;
        var count = _lineRenderers.Count;
        for (int i = 1; i < count; i++)
        {
            Destroy(_lineRenderers[1].gameObject);
            _lineRenderers.RemoveAt(1);
        }

        currentLR = _lineRenderers[0];
        if (clearRenderDraw)
        {
            foreach (var item in _renderTexturelineRenderers)
            {
                Destroy(item.gameObject);
            }
            _renderTexturelineRenderers.Clear();
        }


    }

}
