using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private BigBot bigBot;
    [SerializeField] private SmallBot smallBot;
   
    private float _lastRotate;
    private Vector2 _lastMove = new Vector2();
    private bool _shoot;
    
    private delegate void SimpleDelegate();
    private event SimpleDelegate ToggleExpandEvent;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        BigBot.EndLevel += OnEndLevel;
    }
    private void OnDisable()
    {
        BigBot.EndLevel -= OnEndLevel;
    }

    private void OnEndLevel()
    {
        enabled = false;
    }

    private void FixedUpdate()
    {
        ToggleExpandEvent?.Invoke();
        ToggleExpandEvent = null;

        if (_shoot)
        {
            bigBot.Shoot();
            _shoot = false;
        }

        bigBot.Rotate(_lastRotate);
        smallBot.UpdateMove(_lastMove);
        _lastRotate = 0;
        _lastMove = Vector2.zero;
    }

    private void Update()
    {
        //BigBot Shoot
        if (Input.GetKey(KeyCode.W))
        {
            _shoot = true;
        }

        //BigBot Expand
        if (Input.GetKeyDown(KeyCode.S) && !bigBot.isAnimating())
        {
            ToggleExpandEvent = null;
            ToggleExpandEvent += bigBot.ToggleExpand;
        }
        if (Input.GetKeyUp(KeyCode.S) && !bigBot.isAnimating())
        {
            ToggleExpandEvent = null;
            ToggleExpandEvent += bigBot.ToggleExpand;
        }

        //BigBot Rotate
        if (Input.GetKey(KeyCode.A))
        {
            _lastRotate = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _lastRotate = -1;
        }

        //SmallBot Move
        if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)){
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _lastMove.y = 1;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _lastMove.y = -1;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _lastMove.x = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _lastMove.x = 1;
            }
        }
    }
}
