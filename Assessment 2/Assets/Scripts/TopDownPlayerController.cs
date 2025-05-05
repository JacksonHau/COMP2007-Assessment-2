using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;
    private Animator animator;
    private Camera cam;

    private bool isDead = false;
    private string currentAnim;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        PlayAnim("Player Idle");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        AimAtMouse(); // Add this!

        // Movement Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v);

        // Movement
        Vector3 moveDir = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * inputDir.normalized;
        controller.SimpleMove(moveDir * moveSpeed);

        if (moveDir != Vector3.zero)
        {
            PlayAnim("Player Walk");
        }
        else
        {
            PlayAnim("Player Idle");
        }
    }

    void AimAtMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f; // Stay flat

            if (lookDir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 20f);
            }
        }
    }

    public void Die()
    {
        isDead = true;
        controller.enabled = false;
        PlayAnim("Player Death");
    }

    void PlayAnim(string animName)
    {
        if (currentAnim == animName) return;
        animator.Play(animName);
        currentAnim = animName;
    }
}
