
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class Puzzle : MonoBehaviour
{

    [HideInInspector] public static Puzzle puzzle;

    void Awake()
    {
        puzzle = this;
        Application.targetFrameRate = 24;
    }

    void Start()
    {
        Hint();
        StartFire();

        chest.SetActive(false);
        Player.player.splineAnimate.MaxSpeed = freeMovement;
    }

    float freeMovement = 5f;

//Not one of my proudest works. My apologies~
    #region Torch
    bool torches = false;
    [SerializeField] Gradient gradient;

    public void Torch(ParticleSystem torch)
    {
        if (!torches)
        {
            var main = torch.main;
            main.startColor = Puzzle.puzzle.gradient.Evaluate(Random.Range(.0f, 1.0f));
        }
    }
    #endregion
    #region Fire
    [HideInInspector] public List<ParticleSystem> torchesDown;
    [HideInInspector] public List<ParticleSystem> torchesUp;
    List<Color> colorsRem = new List<Color>();
    
    public void Fire(GameObject fire)
    {
        if (!torches)
        {
            foreach (ParticleSystem x in Puzzle.puzzle.torchesUp)
            {
                if (!Puzzle.puzzle.colorsRem.Contains(x.main.startColor.color))
                {
                    fire.transform.DOShakePosition(0.3f, new Vector3(0.1f, 0.1f, 0.1f), 15, 1).SetLoops(1, LoopType.Restart);
                    return;
                }

            }
            UnlockChest();
            torches = true;
            foreach (ParticleSystem x in torchesUp)
            {
                var p = x.main;
                p.startColor = Color.white;
            }
            foreach (ParticleSystem x in torchesDown)
            {
                var n = x.main;
                n.startColor = Color.white;
            }

        }
    }

        async public void StartFire()
        {
            await UniTask.WaitUntil(() => torchesDown.Count != 0);
                foreach (ParticleSystem x in torchesDown)
                {
                    await UniTask.Delay(200);
                        var main = x.main;
                        Color y;
                            do y = gradient.Evaluate(Random.Range(0.0f, 1.0f));
                            while (colorsRem.Count != 0 && colorsRem.Contains(y));
                        main.startColor = y;
                        colorsRem.Add(y);
                }
        }
    #endregion
    #region Skull
    [HideInInspector] public List<GameObject> skulls;
    
    async public void Skull(GameObject obj)
    {
        skulls.Remove(obj);
        obj.transform.DORotate(Player.player.transform.position, 0.5f, RotateMode.Fast);
        
        await obj.transform.DOJump(Player.player.transform.position, 1, 1, 0.5f, false).AsyncWaitForCompletion();
            Destroy(obj);
    }
    #endregion
    #region Corpse
    bool allSkulls = false;
    public void Corpse(GameObject obj)
    {
        if (!allSkulls)
        {
            if (Puzzle.puzzle.skulls.Count == 0)
            {
                allSkulls = true;
                obj.transform.DORotate(new Vector3(0, 0, 70f), 2, RotateMode.Fast);
                UnlockChest();
            }
            else
                obj.transform.DOShakePosition(0.3f, new Vector3(0.1f, 0.1f, 0.1f), 15, 1).SetLoops(1, LoopType.Restart);
        }
    }
    #endregion
    #region Lift
    async public void Lift(GameObject obj)
    {
        obj.transform.DOMove(new Vector3(obj.transform.position.x, obj.transform.position.y + 1, obj.transform.position.z), 0.5f);
        await UniTask.WaitForSeconds(2);
        obj.transform.DOMove(new Vector3(obj.transform.position.x, obj.transform.position.y - 1, obj.transform.position.z), 0.5f);
    }
    #endregion
    #region Sword
    bool done = false;

    public void Sword(GameObject x)
    {
        if (sword)
        {
            x.transform.DORotate(new Vector3(0, 360, 0f), 2, RotateMode.FastBeyond360);
            if (!done)
                UnlockChest();
            done = true;
        }
        else
            x.transform.DOShakePosition(0.3f, new Vector3(0.1f, 0.1f, 0.1f), 15, 1).SetLoops(1, LoopType.Restart);
    }
    #endregion
    #region Door
    bool sword = false;
    int currentDoor;
    [SerializeField] AudioClip clip_guy;
    [SerializeField] AudioClip clip_thanks;
    [HideInInspector] public List<Interactable> doors;

    public void Door(Interactable door)
    {
        if (!sword)
        {
            if (doors[currentDoor] == door)
            {
                sword = true;

                AudioSource x = doors[currentDoor].source;
                x.clip = clip_thanks;
                x.Play();
            }
            else
            {
                Hint();
                foreach (Interactable y in doors)
                    y.source.Stop();
            }    
        }
        door.transform.DOShakePosition(0.3f, new Vector3(0.0f, 0.0f, 0.3f), 15, 1).SetLoops(1, LoopType.Restart);
    }
    async void Hint()
    {
        await UniTask.WaitForSeconds(15);
            foreach (Interactable y in doors)
                        y.source.Stop();

        currentDoor = Random.Range(0, doors.Count);
        AudioSource x = doors[currentDoor].source;
        x.clip = clip_guy;
        x.Play();

        await UniTask.WaitUntil(() => !x.isPlaying);
            if (!sword)
                Hint();
    }
    #endregion
    #region Chest
    async public void Chest(GameObject obj)
    {
        obj.transform.DOShakePosition(0.3f, new Vector3(0.1f, 1.0f, 0.1f), 30, 1).SetLoops(1, LoopType.Restart);

        await UniTask.WaitForSeconds(3);
            Application.Quit();
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
            #endif
    }
        int locks = 0;
        [SerializeField] GameObject chest;
        public void UnlockChest()
        {
            locks += 1;
            if (locks == 3)
                chest.SetActive(true);
        }
    #endregion
             
}
