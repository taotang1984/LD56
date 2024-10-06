using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class Location : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isBase;

    public ResourceSO resource;
    public int resourcePoints = 0;
    public bool Attainable = false;

    public SpriteRenderer foodIcon;

    public SpriteRenderer circleSprite;

    public TextMeshPro number;
    public List<Location> connectedLocations = new();
    public List<Location> reachableLocations = new();
    public List<LineRenderer> connectedLines = new();

    public Vector3 routeScale;
    public Vector3 resourceScale;


    public void Init(ResourceSO resourceSO)
    {
        if(resourceSO == null)
        {
            transform.localScale = routeScale;
            return;
        }
        resource = resourceSO; 
        resourcePoints = resourceSO.resourcePoints;
        foodIcon.sprite = resourceSO.resourceSprite;
    }

    public virtual void UpdateUI()
    {
        if(resource)
            foodIcon.sprite = resource.resourceSprite;
        else foodIcon.sprite = null;
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(Attainable)
        {
            GameManager.Instance.MovePlayer(this, false);
            // move to this location
            
        }

        Attainable = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if(!isBase)
        {
            this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            circleSprite.color = new Color(0.2470588f, 2352941f, 0.3490196f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(!isBase)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            circleSprite.color = new Color(0.9058824f, 0.9372549f, 0.772549f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }

    }


}
