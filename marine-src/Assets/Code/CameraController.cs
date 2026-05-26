using System.Reflection;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0f, 85f)]
    [SerializeField] private float _maxLookDownAngle = 85f;
    private float MaxLookDownAngle { get { return this._maxLookDownAngle; } }
    [Range(0f, 85f)]
    [SerializeField] private float _maxLookUpAngle = 275f;
    private float MaxLookUpAngle { get { return 360f - this._maxLookUpAngle; } }
    [SerializeField] private Vector2 _sensitivity = new Vector2(50f, 50f); // TODO: botar como preferencia pro player editar
    [SerializeField] private Vector2 _positionOffset = new Vector2(.5f, .5f);
    [Range(0f, 90f)]
    [SerializeField] private float _rotationOffset = 0f;
    [SerializeField] private float _cameraDistance = 2.5f; // TODO: botar como preferencia pro player editar, talvez

    private CursorController _cursor;

    private GameObject _cameraGimblePrefab;
    private GameObject _cameraGimbleInstance;

    private CinemachineCamera _playerCamera;

    private bool _shouldRotateCamerae = false;

    private void Awake()
    {
        this._cameraGimblePrefab = Resources.Load<GameObject>("CameraGimble");
        this._cursor = GameObject.FindFirstObjectByType<CursorController>();
    }

    private void Start()
    {
        this.InitCameraGimble();
        this._cameraGimbleInstance.transform.localPosition = this._positionOffset;
        this._playerCamera = this._cameraGimbleInstance.transform.GetComponentInChildren<CinemachineCamera>();
        this._playerCamera.Prioritize();
    }

    private void Update()
    {
        // Inputs
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            this._shouldRotateCamerae = true;
            this._cursor.SetLock(true);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            this._shouldRotateCamerae = false;
            this._cursor.SetLock(false);
        }

        Vector2 mouseDelta = Input.mousePositionDelta;
        
        // Horizontal Rotation
        Vector3 playerRotation = this.transform.localRotation.eulerAngles;
        playerRotation.y += mouseDelta.x * Time.deltaTime * this._sensitivity.x;

        // Vertical Rotation
        Vector3 gimbleRotation = this._cameraGimbleInstance.transform.localRotation.eulerAngles;
        gimbleRotation.x -= mouseDelta.y * Time.deltaTime * this._sensitivity.y;
        gimbleRotation.y = 0f;
        gimbleRotation.z = 0f;

        // Clamps
        float maxLookDownAngle = this.MaxLookDownAngle;
        float maxLookUpAngle = this.MaxLookUpAngle;
        
        if (gimbleRotation.x <= 180f && gimbleRotation.x > maxLookDownAngle) gimbleRotation.x = this.MaxLookDownAngle;
        if (gimbleRotation.x > 180f && gimbleRotation.x < maxLookUpAngle) gimbleRotation.x = this.MaxLookUpAngle;

        gimbleRotation.x += this._rotationOffset;

        if (this._shouldRotateCamerae)
        {
            // Apply Rotations
            this.transform.localRotation = Quaternion.Euler(playerRotation);
            this._cameraGimbleInstance.transform.localRotation = Quaternion.Euler(gimbleRotation);
        }

        // Avoid Camera Clipping
        this._cameraGimbleInstance.transform.localScale = Vector3.one * this.CalculateCameraDistance();

        // Camera View Angle
        if (Physics.Raycast(Camera.main.ViewportPointToRay(this._cursor.ViewportPosition), out RaycastHit hitInfo, float.MaxValue, 1 << 31))
        {
            Vector3 cursorOffset = hitInfo.point - this._cameraGimbleInstance.transform.position;
            Vector3 lookDirection = this._cameraGimbleInstance.transform.position + (cursorOffset / 10f);
            this._playerCamera.transform.LookAt(lookDirection);
        }
    }

    private void InitCameraGimble()
    {
        if (this._cameraGimbleInstance) return;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject currentChild = this.transform.GetChild(i).gameObject;
            if (currentChild.name != "CameraGimble") continue;
            this._cameraGimbleInstance = currentChild;
            return;
        }

        if (this._cameraGimblePrefab)
        {
            this._cameraGimbleInstance = GameObject.Instantiate(this._cameraGimblePrefab, this.transform);
            return;
        }
#if UNITY_EDITOR
        Debug.Log("<color=#ff0000>[!] Prefab do CameraGimble nao encontrada na pasta Resources</color>");
#endif
    }

    private float CalculateCameraDistance()
    {
        float result = this._cameraDistance;

        if (Physics.Raycast(this._cameraGimbleInstance.transform.position, -this._cameraGimbleInstance.transform.forward, out RaycastHit hitInfo, this._cameraDistance))
        {
            result = hitInfo.distance;
        }

        return result * 2;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!this._cameraGimbleInstance)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                GameObject currentChild = this.transform.GetChild(i).gameObject;
                if (currentChild.name != "CameraGimble") continue;
                this._cameraGimbleInstance = currentChild;
                return;
            }

            if (!this._cameraGimbleInstance) Debug.Log($"<color=#ffff00>[?] CameraGimble nao encontrada em {this.gameObject.name}</color>");
        }

        if (this._cameraGimbleInstance)
        {
            this._cameraGimbleInstance.transform.localPosition = this._positionOffset;
            this._cameraGimbleInstance.transform.localRotation = Quaternion.Euler(this._rotationOffset, 0f, 0f);
            this._cameraGimbleInstance.transform.localScale = Vector3.one * this._cameraDistance * 2f;
        }
    }

    private void OnDrawGizmos()
    {
        //if (this._cameraGimbleInstance)
        //{
        //    Vector3 origin = this._cameraGimbleInstance.transform.position;

        //    float radiansA = (this.MaxLookUpAngle + this._rotationOffset) * Mathf.Deg2Rad;
        //    Vector3 directionA = new Vector3(0f, Mathf.Sin(radiansA), -Mathf.Cos(radiansA)) * this._cameraDistance;
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawRay(origin, directionA);

        //    float radiansB = (this.MaxLookDownAngle + this._rotationOffset) * Mathf.Deg2Rad;
        //    Vector3 directionB = new Vector3(0f, Mathf.Sin(radiansB), -Mathf.Cos(radiansB)) * this._cameraDistance;
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(origin, directionB);
        //}

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(this._playerCamera.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
#endif
}
