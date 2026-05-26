using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _followPoint;

    [SerializeField] private float movementSpeed = 5.0f;

    private Rigidbody _playerRigidbody;
    private GameObject _playerModel;

    private Vector3 _lookDirection;

    private void Start()
    {
        this._playerRigidbody = this.GetComponent<Rigidbody>();
        this._playerModel = this.transform.GetChild(0).gameObject;

        this._followPoint.transform.parent = null;
    }

    private void Update()
    {
        // Inputs
        Vector3 forward = (this.transform.forward * Input.GetAxis("Vertical"));
        Vector3 sideways = (this.transform.right * Input.GetAxis("Horizontal"));

        // Calc followPoint
        Vector3 targetPosition = this.transform.position + (forward + sideways).normalized;
        this._followPoint.transform.position = Vector3.Slerp(this._followPoint.transform.position, targetPosition, .1f);

        // Apply velocity
        Vector3 movementDirection = (this._followPoint.transform.position - this.transform.position);
        this._playerRigidbody.linearVelocity = movementDirection * this.movementSpeed; // considerar normal dps

        // Model Rotation
        this._lookDirection = movementDirection.magnitude > 0.2f ? movementDirection.normalized : this._lookDirection;
        this._playerModel.transform.LookAt(this.transform.position + this._lookDirection + Vector3.up);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this._followPoint.transform.position, Vector3.up);
    }
#endif
}
