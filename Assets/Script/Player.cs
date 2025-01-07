using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController characterController;
    public float speed = 2.0f;  // 기본 속도
    public float runSpeedMultiplier = 1.5f;  // 달리기 속도 배율
    public float rotationSpeed = 5.0f;  // 회전 속도
    public float mouseSensitivity = 100f;  // 마우스 감도

    public float xRotationMin = -90f;  // 카메라 X축 최소 회전 각도
    public float xRotationMax = 45f;   // 카메라 X축 최대 회전 각도

    public float jumpHeight = 2.0f;  // 점프 높이
    public float gravity = -9.8f;   // 중력 값
    public float groundCheckDistance = 0.2f; // 땅과의 거리 확인

    private Vector3 velocity; // 현재 속도 벡터
    private bool isGrounded;  // 땅에 닿아 있는지 여부

    Animator animator;
    bool isWalk = false;

    public AudioClip footstepSound;  // 발소리 오디오 클립

    Transform cameraTransform;
    float xRotation = 0f;  // 카메라의 X축 회전 값

    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;  // 마우스 커서를 잠금 상태로 설정
    }

    void Update()
    {
        CheckGrounded();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 약간의 중력 효과를 주어 지면에 안정적으로 유지
        }

        Walk();
        Rotate();
        Jump();

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void CheckGrounded()
    {
        // Raycast를 사용하여 땅과의 접촉 감지
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + characterController.height / 2);
    }

    void Walk()
    {
        isWalk = false;
        float currentSpeed = speed;

        // Shift 키를 눌렀을 때 달리기
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= runSpeedMultiplier;  // 달리기 속도 적용
            animator.SetBool("isRun", true);  // 달리기 애니메이션
        }
        else
        {
            animator.SetBool("isRun", false);  // 달리기 애니메이션 비활성화
        }

        // W, A, S, D 키 입력에 따른 이동
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
        // 마우스 입력 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 캐릭터의 Y축 회전
        transform.Rotate(Vector3.up * mouseX);

        // 카메라의 X축 회전 제한
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // 점프 속도 계산
        }
    }

    public void OnFootstep()
    {
        if (footstepSound != null)
        {
            AudioSource.PlayClipAtPoint(footstepSound, Camera.main.transform.position);
        }
    }
}
