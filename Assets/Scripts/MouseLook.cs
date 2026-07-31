using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance { get; private set; }

    [SerializeField]
    Transform playerBody;
    [SerializeField]
    float mouseSensitivity = 100f;
    [SerializeField] Transform spine1;
    [SerializeField] Transform spine2;
    [SerializeField] Transform neck;
    [SerializeField] Transform rightShoulder;
    [SerializeField] Transform leftShoulder;
    private float spine1StartRotation;
    private float spine2StartRotation;
    private float neckStartRotation;
    private float leftShoulderStartRotation;
    private float rightShoulderStartRotation;

    private float _xRotation = 0f;
    private float _mouseX = 0f;
    private float _mouseY = 0f;
    private bool _canRotate = true;
    void Start()
    {
        spine1StartRotation = spine1.localEulerAngles.x;
        spine2StartRotation = spine2.localEulerAngles.x;
        neckStartRotation = neck.localEulerAngles.x;
        leftShoulderStartRotation = leftShoulder.localEulerAngles.z;
        rightShoulderStartRotation = rightShoulder.localEulerAngles.z;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // ��������� �������� ������
        if (_canRotate)
        {
            _mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            _mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _xRotation -= _mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            playerBody.Rotate(Vector3.up * _mouseX);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, playerBody.eulerAngles.y, 0);
            spine1.localRotation = Quaternion.Euler(spine1StartRotation + _xRotation / 3, spine1.localEulerAngles.y, spine1.localEulerAngles.z);
            spine2.localRotation = Quaternion.Euler(spine2StartRotation + _xRotation / 3, spine2.localEulerAngles.y, spine2.localEulerAngles.z);
            neck.localRotation = Quaternion.Euler(neckStartRotation + _xRotation / 3, neck.localEulerAngles.y, neck.localEulerAngles.z);
            rightShoulder.localRotation = Quaternion.Euler(rightShoulder.localEulerAngles.x, rightShoulder.localEulerAngles.y, rightShoulderStartRotation - _xRotation / 3);
            leftShoulder.localRotation = Quaternion.Euler(leftShoulder.localEulerAngles.x, leftShoulder.localEulerAngles.y, leftShoulderStartRotation + _xRotation / 3);
            // Debug.Log(transform.eulerAngles.x);
        }
    }
    public void EnableRotate(bool enable)
    {
        _canRotate = enable;
    }
}
