using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : MonoBehaviour
{
    public delegate void VoidDelegate();
    public delegate void IntDelegate(int i);
    public static VoidDelegate EndLevel;
    public static event IntDelegate BigBotHealthUpdate;
    
    [SerializeField] private int _lives;
    [SerializeField] private int _max_lives;
    [SerializeField] private float rotationSpeed;

    [Header("Missiles")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private List<Transform> missileSpawnPoints;
    [SerializeField] private List<Transform> sparks;
    [SerializeField] private Transform missileParent;
    [SerializeField] private float shootInterval;

    [Header("Shockwave")]
    [SerializeField] private GameObject shockwave;
    [SerializeField] private GameObject shockwaveEnabled;
    [SerializeField] private GameObject shockwaveHalo;
    [SerializeField] private float shockwaveSpeed;
    [SerializeField] private float shockwaveMaxSize;
    [SerializeField] private float shockwaveCooldown;
    private bool _shockwaveEnabled = true;

    [Header("Ability")]
    [SerializeField] private Animator animator;
    [SerializeField] private float _retractingAnimationTime;
    [SerializeField] private float _expandingAnimationTime;
    public delegate void OnToggleExpandDelegate(bool expanded);
    public static event OnToggleExpandDelegate OnToggleExpand;
    private bool _retractable = true;
    private bool _expanded = true;
    private bool _animating = false;
    public delegate void RetractedRotationDelegate(float angle);
    public static event RetractedRotationDelegate RetractedRotation;

    private Coroutine _deactivateShooting;
    private AudioSource _shotSound;
    private AudioSource _shockwaveSound;

    public BigBot()
    {
        OnToggleExpand = null;
        RetractedRotation = null;
    }

    private void Start()
    {
        _shotSound = GetComponents<AudioSource>()[0];
        _shockwaveSound = GetComponents<AudioSource>()[1];
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Comet"))
        {
            if (_expanded)
            {
                UpdateLives(-1);
            }
        }
        else if (other.CompareTag("Player"))
        {
            
        }
    }

    public void UpdateLives(int add)
    {
        if (_lives + add <= -1)
        {
            EndLevel?.Invoke();
            return;
        }
        else if (_lives + add > _max_lives)
        {
            _lives = _max_lives;
            BigBotHealthUpdate?.Invoke(_lives);
            return;
        }
        else
        {
            _lives += add;
            BigBotHealthUpdate?.Invoke(_lives);
        }

        foreach (Transform spawnPoint in missileSpawnPoints)
        {
            spawnPoint.gameObject.SetActive(false);
        }
        for (int i = 0; i < _lives; i++)
        {
            int gunFactor = _max_lives / missileSpawnPoints.Count;
            missileSpawnPoints[i / gunFactor].gameObject.SetActive(true);
        }
    }

    public void EnableShockwave()
    {
        _shockwaveEnabled = true;
        shockwaveEnabled.SetActive(true);
        _retractable = true;
    }

    private void DisableShockwave()
    {
        _shockwaveEnabled = false;
        shockwaveEnabled.SetActive(false);
        _retractable = false;
    }

    public void ToggleExpand()
    {
        if (_expanded)
        {
            if (!_retractable)
            {
                return;
            }
            shockwaveEnabled.SetActive(false);
            shockwaveHalo.SetActive(true);    

            animator.SetTrigger("shrink");
            _expanded = false;
            OnToggleExpand?.Invoke(_expanded);
        }
        else
        {
            shockwaveHalo.SetActive(false);
            StartCoroutine(SendShockwave(shockwaveMaxSize, shockwaveSpeed));

            animator.SetTrigger("expand");
            _animating = true;
            StartCoroutine(OnExpanding());
        }
    }

    private IEnumerator OnExpanding()
    {
        OnToggleExpand?.Invoke(true);
        yield return new WaitForSeconds(_expandingAnimationTime);
        _expanded = true;
        _animating = false;
    }

    public bool isAnimating() {
        return _animating;
    }

    public void Rotate(float rotate)
    {
        transform.Rotate(new Vector3(0, 0, rotate * rotationSpeed));
        RetractedRotation?.Invoke(rotate * rotationSpeed);
    }

    public void Shoot()
    {
        if (_deactivateShooting != null || !_expanded)
        {
            return;
        }

        _deactivateShooting = StartCoroutine(DisableShootingForSeconds(shootInterval));
        foreach (Transform spawnPoint in missileSpawnPoints)
        {
            if (spawnPoint.gameObject.activeSelf)
            {
                GameObject missile = Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation, missileParent);
            }
        }

        for (int i = 0; i < missileSpawnPoints.Count; i++)
        {
            sparks[i].gameObject.SetActive(false);
            if (!missileSpawnPoints[i].gameObject.activeSelf)
            {
                sparks[i].gameObject.SetActive(true);
            }
        }

        if (_lives >= 1)
        {
            _shotSound.Play();
        }
    }

    private IEnumerator DisableShootingForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _deactivateShooting = null;
    }

    private IEnumerator SendShockwave(float targetSize, float speed)
    {
        if (_shockwaveEnabled)
        {
            shockwave.transform.localScale = new Vector3(1, 1, 1);
            shockwave.SetActive(true);
            _shockwaveSound.Play();
            yield return new WaitForSeconds(0.18f);
            while (shockwave.transform.localScale.x + speed < targetSize)
            {
                shockwave.transform.localScale += new Vector3(1, 1, 1) * speed;
                yield return new WaitForFixedUpdate();
            }

            shockwave.SetActive(false);
            DisableShockwave();
        }
    }
}
