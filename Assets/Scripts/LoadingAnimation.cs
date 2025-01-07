using UnityEngine;
using UnityEngine.Events;

public class LoadingAnimation : MonoBehaviour
{
    public static LoadingAnimation Instance;
    
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onSetActive;
    [SerializeField] private UnityEvent onSetInactive;

    private void Awake()
    {
        Instance = this;
    }

    public void Set(bool active)
    {
        animator.SetBool("loading", active);
        
        if (active)
            onSetActive.Invoke();
        else
            onSetInactive.Invoke();
    }
}
