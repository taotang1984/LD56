using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool isFull = false;
    public ResourceSO currentResource;
    public SpriteRenderer resourceIcon;


    public void GetResource(Location location)
    {
        if (!isFull && location.resource != null)
        {
            currentResource = location.resource;
            resourceIcon.sprite = currentResource.resourceSprite;
            location.resource = null;
            
            location.UpdateUI();
            isFull = true;
            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.collectClip);
        }else
        {
            Debug.Log("Player is full");
        }
    }

    public void GetEnergy(Location location)
    {
        if (GameManager.Instance.currentEnergy < GameManager.Instance.maxEnergy)
        {
            GameManager.Instance.currentEnergy += location.resource.resourcePoints;
            if (GameManager.Instance.currentEnergy > GameManager.Instance.maxEnergy)
            {
                GameManager.Instance.currentEnergy = GameManager.Instance.maxEnergy;
            }
            location.resourcePoints = 0;
            location.resource = null;
            location.UpdateUI();
            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.collectClip);
        }
    }

    public void TransferResource(BaseLocation baseLocation)
    {
        if (isFull)
        {
            GameManager.Instance.storedResources.Add(currentResource);
            currentResource = null;
            resourceIcon.sprite = null;
            isFull = false;
            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.transferClip);

        }
    }

    public void Teleport(Location location)
    {
        GameManager.Instance.MovePlayer(LocationGenerator.Instance.baseLocation, true);
        location.resourcePoints = 0;
        location.resource = null;
        location.UpdateUI();

    }

    public void RefillEnergy()
    {
        GameManager.Instance.currentEnergy = GameManager.Instance.maxEnergy;
    }
}
