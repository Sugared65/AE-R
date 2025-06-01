
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{

    Renderer render;
    ParticleSystem torch;

    public enum WhatKind { Nothing, Torch, Fire, Skull, Corpse, Lift, Sword, Door, Chest }
    public WhatKind whatKind;

    void Awake()
    {
        References();
    }
        async void References()
        {
            render = GetComponent<Renderer>();
            source = GetComponent<AudioSource>();
            await UniTask.WaitUntil(() => Puzzle.puzzle != null);
            if (whatKind == WhatKind.Fire)
                foreach (ParticleSystem x in GetComponentsInChildren<ParticleSystem>())
                    Puzzle.puzzle.torchesDown.Add(x);
        }

    void Start()
    {
        SetLists();
    }
        void SetLists()
        {
            if (whatKind == WhatKind.Torch)
            {
                torch = GetComponentInChildren<ParticleSystem>();
                Puzzle.puzzle.torchesUp.Add(torch);
            }
            else if (whatKind == WhatKind.Skull)
                Puzzle.puzzle.skulls.Add(gameObject);
            else if (whatKind == WhatKind.Door)
                Puzzle.puzzle.doors.Add(this);
        }

        void OnMouseEnter()
        {
            if (whatKind == WhatKind.Fire || whatKind == WhatKind.Sword || whatKind == WhatKind.Corpse)
                render.material.DOColor(Color.yellow, 0.5f);
            else
                render.material.DOColor(Color.red, 0.5f);
        }

        void OnMouseExit()
        {
            render.material.DOColor(Color.white, 0.5f);
        }

    [SerializeField] AudioClip clip;
    [HideInInspector] public AudioSource source;
    bool done = false;
    public void Interact()
    {
        source.clip = clip;
        source.Play();

        if (whatKind == WhatKind.Nothing)
            Debug.Log("Interactable does nothing.");
        else if (whatKind == WhatKind.Torch)
            Puzzle.puzzle.Torch(torch);
        else if (whatKind == WhatKind.Fire)
            Puzzle.puzzle.Fire(gameObject);
        else if (whatKind == WhatKind.Skull && !done)
        {
            done = true;
            Puzzle.puzzle.Skull(gameObject);
        }
        else if (whatKind == WhatKind.Corpse)
            Puzzle.puzzle.Corpse(gameObject);
        else if (whatKind == WhatKind.Lift)
            Puzzle.puzzle.Lift(gameObject);
        else if (whatKind == WhatKind.Sword)
            Puzzle.puzzle.Sword(gameObject);
        else if (whatKind == WhatKind.Door)
            Puzzle.puzzle.Door(this);
        else if (whatKind == WhatKind.Chest)
            Puzzle.puzzle.Chest(gameObject);
    }

}