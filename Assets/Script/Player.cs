using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController characterController;
    public float speed = 2.0f;  // �⺻ �ӵ�
    public float runSpeedMultiplier = 1.5f;  // �޸��� �ӵ� ����
    public float rotationSpeed = 5.0f;  // ȸ�� �ӵ�
    public float mouseSensitivity = 100f;  // ���콺 ����

    public float xRotationMin = -90f;  // ī�޶� X�� �ּ� ȸ�� ����
    public float xRotationMax = 45f;   // ī�޶� X�� �ִ� ȸ�� ����

    Animator animator;
    bool isWalk = false;

    public AudioClip footstepSound;  // �߼Ҹ� ����� Ŭ��

    Transform cameraTransform;
    float xRotation = 0f;  // ī�޶��� X�� ȸ�� ��

    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;  // ���콺 Ŀ���� ��� ���·� ����
    }

    void Update()
    {
        Walk();
        Rotate();
    }

    void Walk()
    {
        isWalk = false;
        float currentSpeed = speed;

        // Shift Ű�� ������ �� �޸���
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= runSpeedMultiplier;  // �޸��� �ӵ� ����
            animator.SetBool("isRun", true);  // �޸��� �ִϸ��̼�
        }
        else
        {
            animator.SetBool("isRun", false);  // �޸��� �ִϸ��̼� ��Ȱ��ȭ
        }

        // W, A, S, D Ű �Է¿� ���� �̵�
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += this.transform.forward;
            isWalk = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= this.transform.forward;
            isWalk = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= this.transform.right;
            isWalk = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += this.transform.right;
            isWalk = true;
        }

        characterController.Move(moveDirection.normalized * Time.deltaTime * currentSpeed);

        if (isWalk)
        {
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
    }

    void Rotate()
    {
        // ���콺 �Է� ó��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ĳ������ Y�� ȸ��
        transform.Rotate(Vector3.up * mouseX);

        // ī�޶��� X�� ȸ�� ����
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void OnFootstep()
    {
        if (footstepSound != null)
        {
            AudioSource.PlayClipAtPoint(footstepSound, Camera.main.transform.position);
        }
    }
}
