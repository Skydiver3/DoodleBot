using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private float despawnAfter = 4;
    [SerializeField] private GameObject manaItem;
    [SerializeField] private GameObject repairItem;

    private Items _type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _type == Items.Repair)
        {
            if (other.gameObject.GetComponent<SmallBot>() != null &&
                other.gameObject.GetComponent<SmallBot>().CollectPowerUp(_type))
            {
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Player") && _type == Items.Mana)
        {
            if (other.gameObject.GetComponent<SmallBot>() != null &&
                other.gameObject.GetComponent<SmallBot>().CollectPowerUp(_type))
            {
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        //pick random itemType 
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                _type = Items.Repair;
                repairItem.SetActive(true);
                break;
            case 1:
                _type = Items.Mana;
                manaItem.SetActive(true);
                break;
            default:
                _type = Items.Repair;
                repairItem.SetActive(true);
                break;
        }

        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnAfter);
        SpriteRenderer renderer = GetComponentsInChildren<SpriteRenderer>()[0];
        while (renderer.color.a > 0.01)
        {
            yield return null;
            renderer.color = new Color(255, 255, 255, renderer.color.a - 0.05f);
        }
        Destroy(gameObject);
    }

    public Items GetItemType()
    {
        return _type;
    }
}

public enum Items
{
    None, Repair, Mana
}