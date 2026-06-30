using UnityEngine;

public class doorcontroller : MonoBehaviour
{
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private Animator animator;

    private Vector3 defaultScale;



    public bool isLevel_Return = false;
    

    private void Start()
    {
       
    }
   

  //  void OnDisable()
   // {
    //    Debug.Log("GameObject Disabled");
   // }

    private void Update()
    {
        //if(isLevel_Return)
        //{
        //    CloseDoor();
        //    isLevel_Return=false;
        //}
        //if (gameObject.activeInHierarchy && !isLevel_Return)
        //{
        //    scale1.localScale = Vector3.one;
        //    scale2.localScale = new(1f, 1f, 1f);
        //    //CloseDoor();
        //    isLevel_Return = true;
        //    Debug.Log("visible");
        //}

            
        //}else if(!gameObject.activeInHierarchy)
        //{
        //    Debug.Log("Inactive");
        //    isLevel_Return = false;
        //}
        //if (gameObject.activeSelf && !isLevel_Return)
        //{
        //    CloseDoor();
        //    isLevel_Return = true;
        //    Debug.Log("Enabled");
        //}
       
    }

    private void Awake() {
        defaultScale = transform.localScale;
    }
    public void OpenDoor()
    {
        animator.SetBool("Open", true);

        doorCollider.enabled = false;
      
    }

    public void CloseDoor()
    {
        animator.SetBool("Open", false);

        doorCollider.enabled = true;
      
    }
}