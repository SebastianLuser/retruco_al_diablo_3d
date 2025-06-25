using UnityEngine;

public class CameraPerspectiveManager : MonoBehaviour
{
    public Transform perspective1;
    public Transform perspective2;
    
    public float transitionSpeed = 2f;
    
    private Transform targetPerspective;
    private Animator camAnim;
    
    void Start()
    {
        if(perspective2 != null)
            targetPerspective = perspective2;
        
        camAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            targetPerspective = perspective1;
            camAnim.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            targetPerspective = perspective2;
            camAnim.enabled = false;
        }
        
        if (targetPerspective != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPerspective.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPerspective.rotation, Time.deltaTime * transitionSpeed);
        }
    }

    public void EnableCamAnimation()
    {
        camAnim.enabled = true;
    }
}