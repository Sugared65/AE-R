
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{

    [HideInInspector] public static Player player;

    bool walking
    {
        get { return r_walking; }

        set
        {
            r_walking = value;
            Cursor.visible = !value;
            if (value)
                splineAnimate.Play();
            else
                splineAnimate.Pause();
        }
    }
    bool r_walking;

    [HideInInspector] public SplineAnimate splineAnimate;

    void Awake()
    {
        player = this;
        splineAnimate = GetComponent<SplineAnimate>();
    }

    void Start()
    {
        walking = true;
    }

        public void Walk(InputAction.CallbackContext state)
        {
            if (state.performed && !walking)
                walking = true;
        }
        
        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Interactable")
                walking = false;
        }
    
        public void Interact(InputAction.CallbackContext state)
        {
            if (state.performed && !walking)
            {
                Ray r = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(r, out RaycastHit hit))
                    if (hit.collider.gameObject.GetComponent<Interactable>() != null)
                        hit.collider.gameObject.GetComponent<Interactable>().Interact();
            }
        }

}