using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    public bool CameraMove = false;

    [SerializeField] private float _smoothSpeed = 5f;

    private Camera    _cam;   
    private Transform _target; 

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _cam    = Camera.main;
        _target = null;   
        CameraMove = false; 
    }

    private void LateUpdate()
    {
        if (!CameraMove || _target == null || _cam == null) return;
        
        if (MapManager.Instance != null)
        {
            Bounds bounds = MapManager.Instance.MapBounds;
            
            float camH = _cam.orthographicSize;
            float camW = _cam.orthographicSize * _cam.aspect;
            
            float x = Mathf.Clamp(_target.position.x,
                bounds.min.x + camW, bounds.max.x - camW);
            float y = Mathf.Clamp(_target.position.y,
                bounds.min.y + camH, bounds.max.y - camH);

            Vector3 dest = new Vector3(x, y, _cam.transform.position.z);
            _cam.transform.position = Vector3.Lerp(
                _cam.transform.position, dest, _smoothSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 dest = new Vector3(
                _target.position.x,
                _target.position.y,
                _cam.transform.position.z);
            _cam.transform.position = Vector3.Lerp(
                _cam.transform.position, dest, _smoothSpeed * Time.deltaTime);
        }
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
