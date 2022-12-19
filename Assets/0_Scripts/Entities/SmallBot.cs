using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Quaternion = System.Numerics.Quaternion;

public class SmallBot : MonoBehaviour
{
    public static event BigBot.VoidDelegate EndLevel;
    public static event BigBot.IntDelegate SmallBotHealthUpdate;

    // Variables
    [SerializeField] private Transform leftBottom;
    [SerializeField] private Transform rightTop;
    [SerializeField] private GameObject poweredUpAvatar;
    [SerializeField] private Transform lockPoint;
    [SerializeField] private float speed;
    [SerializeField] private float turningSpeed;
    private float _targetRotation;
    private Items _poweredUp = Items.None;
    private bool _lockedAtCenter = false;
    private bool _overBigBot = true;
    [SerializeField] private int _lives;
    [SerializeField] private int _max_lives;
    [SerializeField] private int _battery;
    [SerializeField] private float _healTimePerUnit;
    [SerializeField] private float _chargeTimePerUnit;
    private bool healing = false;
    private bool charging = false;

    public SmallBot()
    {
        BigBot.OnToggleExpand += ToggleLockInMiddle;   
    }

    ~SmallBot()
    {
        BigBot.OnToggleExpand -= ToggleLockInMiddle;
    }

    private void FixedUpdate()
    {
        if (_lockedAtCenter) {
            if (!healing) {
                StartCoroutine(HealSmallBot(_healTimePerUnit));
            }
            if (!charging)
            {
                StartCoroutine(ChargeBattery(_chargeTimePerUnit));
            }
        }
    }

    private IEnumerator HealSmallBot(float seconds)
    {
        healing = true;
        yield return new WaitForSeconds(seconds);
        UpdateLives(1);
        healing = false;
    }

    private IEnumerator ChargeBattery(float seconds)
    {
        charging = true;
        yield return new WaitForSeconds(seconds);
        charging = false;
    }

    public void ToggleLockInMiddle(bool expanding)
    {
        if (!_overBigBot)
        {
            return;
        }
        if (_lockedAtCenter && expanding)
        {
            _lockedAtCenter = false;
            BigBot.RetractedRotation -= rotateBy;
            return;
        }

        if (!_lockedAtCenter && !expanding)
        {
            transform.position = lockPoint.position;
            _lockedAtCenter = true;
            BigBot.RetractedRotation += rotateBy;
        }      
    }
    
    public void UpdateMove(Vector2 direction)
    {
        if (_lockedAtCenter)
        {
            return;
        }
        float currentRotation = transform.rotation.eulerAngles.z;
        if (direction != Vector2.zero)
        {
            _targetRotation = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 270;
            if (_targetRotation > 360)
            {
                _targetRotation -= 360;
            }

            if (_targetRotation-currentRotation>180)
            {
                _targetRotation -= 360;
            }
            else if (currentRotation-_targetRotation > 170)
            {
                _targetRotation += 360;
            }
        }

        float moveX = 0;
        float moveY = 0;
        if (gameObject.transform.position.x + direction.x * speed > leftBottom.position.x &&
            gameObject.transform.position.x + direction.x * speed < rightTop.position.x)
        {
            moveX = direction.x * speed;
        }
        if (gameObject.transform.position.y + direction.y * speed > leftBottom.position.y &&
            gameObject.transform.position.y + direction.y * speed < rightTop.position.y)
        {
            moveY = direction.y * speed;
        }
        transform.Translate(new Vector3(moveX, moveY, 0),Space.World);
        float angle = Mathf.Lerp(currentRotation,_targetRotation,turningSpeed);
        transform.rotation = UnityEngine.Quaternion.Euler(new Vector3(0,0,angle));
    }

    public void rotateBy(float angle) {
        transform.Rotate(new Vector3(0, 0, angle));
    }

    public bool CollectPowerUp(Items type)
    {
        if (_poweredUp != Items.None)
        {
            return false;
        }
        _poweredUp = type;
        poweredUpAvatar.SetActive(true);
        return true;
    }

    public void LosePowerUp()
    {
        _poweredUp = Items.None;
        poweredUpAvatar.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_poweredUp != Items.None)
            {
                if (_poweredUp == Items.Mana)
                {
                    other.GetComponent<BigBot>().EnableShockwave();
                }
                else
                {
                    other.GetComponent<BigBot>().UpdateLives(2);
                }
                LosePowerUp();
            }
            _overBigBot = true;
        }
        else if (other.CompareTag("Comet") && !_lockedAtCenter)
        {
            UpdateLives(-1);
        }
    }

    public void UpdateLives(int add)
    {
        if (_lives + add <= -1)
        {
            BigBot.EndLevel?.Invoke();
            return;
        }
        else if (_lives + add > _max_lives)
        {
            _lives = _max_lives;
            return;
        }
        else
        {
            _lives += add;
            SmallBotHealthUpdate?.Invoke(_lives);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _overBigBot = false;
        }
    }
}
