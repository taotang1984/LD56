using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
public class GameManager : SingletonMonobehaviour<GameManager>
{
    public Player player;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveClip;
    public AudioClip collectClip;
    public AudioClip transferClip;
    public AudioClip backgroundMusic;

    [Header("UI")]

    public TextMeshProUGUI maxEnergyText;
    public TextMeshProUGUI currentEnergyText;
    public TextMeshProUGUI goalResourceText;
    public TextMeshProUGUI currentResourceText;
    public Scrollbar energyBar;
    public Scrollbar pointBar;
    public List<Image> resourceImages;
    public GameObject gameWinPanel;
    public GameObject gameOverPanel;
    public GameObject gameInfoPanel;

    [Header("Game settings")]
    public int goalPoints;
    public int currentPoints;

    public List<ResourceSO> storedResources = new List<ResourceSO>();
    public int maxEnergy;
    public int currentEnergy;

    private MainControl mainControl;
    
    [Header("Location settings")] 
    public Location currentLocation;
    private void Start() {
        mainControl = FindObjectOfType<MainControl>();
        goalPoints = mainControl.goalPoints;
        currentLocation = LocationGenerator.Instance.baseLocation;
        currentEnergy = maxEnergy;
        currentPoints = 0;
        UpdateLocationAttainableLines();
        UpdateEnergyPointsUIInfo();
        foreach (var image in resourceImages)
        {
            image.gameObject.SetActive(false);
        }
        gameInfoPanel.SetActive(true);
    }

    private void UpdateEnergyPointsUIInfo()
    {
        maxEnergyText.text = maxEnergy.ToString();
        currentEnergyText.text = currentEnergy.ToString();
        energyBar.size = (float)currentEnergy / maxEnergy;

        goalResourceText.text = goalPoints.ToString();
        currentResourceText.text = currentPoints.ToString();
        pointBar.size = (float)currentPoints / goalPoints;
        
    }

    public void UpdateLocationAttainableLines(){
        // hide all other lines
        LocationGenerator.Instance.baseLocation.Attainable = false;

        foreach(var location in LocationGenerator.Instance.locations)
        {
            location.Attainable = false;
            location.gameObject.SetActive(true);
            location.connectedLines.ForEach(line => line.gameObject.SetActive(true));
        }

        // show current location and lines
        currentLocation.gameObject.SetActive(true);
        currentLocation.connectedLines.ForEach(line => line.gameObject.SetActive(true));
        foreach(var location in currentLocation.connectedLocations)
        {
            location.Attainable = true;
            location.gameObject.SetActive(true);
        }
    }


    public void MovePlayer(Location location, bool isTeleport = false)
    {
        if (currentEnergy <= 0)
        {
            // cannot move

            gameOverPanel.SetActive(true);
            return;
        }
        var targetPosition = location.transform.position;
        float rotation = Utility.GetRotationFromTwoVectors(player.transform.position, location.transform.position);
        audioSource.PlayOneShot(moveClip);
        player.transform.DORotate(new Vector3(0,0,rotation-90), 0.1f).OnComplete(() => {
            player.transform.DOMove(location.transform.position, 0.2f).OnComplete(() => {
                Camera.main.transform.DOMove(location.transform.position + new Vector3(0, 0, -10), 0.2f);
            });
        });
        
        currentLocation = location;
        if (!isTeleport)
            currentEnergy -= 1;

        StartCoroutine(UpdateLocationCoroutine(location));
    }

    private IEnumerator UpdateLocationCoroutine(Location location)
    {
        yield return new WaitForSeconds(0.3f);

        if(location.resource == LocationGenerator.Instance.itemLibrary[0])
        {
            player.GetEnergy(location);
        }
        else if(location.resource == LocationGenerator.Instance.itemLibrary[1])
        {
            player.Teleport(location);
        }
        else if(!location.isBase)
        {
            player.GetResource(location);
        }
        else
        {
            player.TransferResource((BaseLocation)location);
            UpdateResourceUIInfo();
        }
        UpdateLocationAttainableLines();
        UpdateEnergyPointsUIInfo();
        yield return null;
    }

    private void UpdateResourceUIInfo()
    {
        UpdateResourceUIImange();
        StartCoroutine(UpdateResourcePointsCoroutine());

    }
    
    private IEnumerator UpdateResourcePointsCoroutine()
    {
        yield return new WaitForSeconds(1f);
        //caculate resource points
        if(storedResources.Count == 2)
        {
            if(storedResources[0]!= storedResources[1])
            {
                currentPoints += storedResources[0].resourcePoints;
                storedResources.RemoveAt(0);
            }
        }
        else if(storedResources.Count == 3)
        {
            if(storedResources[0] == storedResources[1])
            {
                if (storedResources[1] == storedResources[2])
                {
                    currentPoints += storedResources[0].resourcePoints * 3 * 2;
                    storedResources.Clear();
                }
                else
                {
                    currentPoints += storedResources[0].resourcePoints * 2;
                    storedResources.RemoveAt(0);
                    storedResources.RemoveAt(0);
                }
            }
        }
        if(currentPoints >= goalPoints)
        {
            GameWin();
        }
        UpdateResourceUIImange();
        UpdateEnergyPointsUIInfo();
    }

    private void UpdateResourceUIImange()
    {
        foreach(var image in resourceImages)
        {
            image.gameObject.SetActive(false);
        }
        for (int i = 0; i < storedResources.Count; i++)
        {
            resourceImages[i].sprite = storedResources[i].resourceSprite;
            resourceImages[i].gameObject.SetActive(true);            
        }

    }

    public void RestartGame()
    {
        mainControl.RestartGame();
    }
    public void NextLevel()
    {
        mainControl.NextLevel();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void GameWin()
    {
        gameWinPanel.SetActive(true);
    }

    public void ShowGameInfo()
    {
        gameInfoPanel.SetActive(true);
    }

    public void CloseGameInfo()
    {
        gameInfoPanel.SetActive(false);
    }

}
