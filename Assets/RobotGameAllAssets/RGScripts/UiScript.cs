using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    public Text batteryPercentText;
    //public Image batteryLevel100;
    //public Image batteryLevel50;
    //public Image batteryLevel10;
    public GameObject BatteryDisplay;
    public int batteryPercent = 9;
    public int numberOfBatteries = 1;
    public int maxSolarCell = 1;
    public int drainSpeed = 1;
    public int cellWidthVariable;
    public GameObject SolarCellRef;
    public GameObject BatteryChargeRef;
    public List<GameObject> SolarCellList;
    public GameObject player;
    public float drainTime = 1.0f;

    public Slider _sliderBattery = null;
    public Image _imageBatteryColor = null;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxSolarCell; i++)
        {
            var cell = Instantiate(SolarCellRef);
            SolarCellList.Add(cell);
        }
        DisplayCellsOnCanvas();
        batteryPercentText.text = batteryPercent + "%";
        StartCoroutine(DrainBattery());
    }

    void DisplayCellsOnCanvas()
    {
        foreach (var cell in SolarCellList)
        {
            cell.transform.SetParent(BatteryChargeRef.transform);
            cell.transform.position = new Vector3(BatteryChargeRef.transform.position.x + (SolarCellList.IndexOf(cell) * cellWidthVariable), BatteryChargeRef.transform.position.y, 0);
        }
    }

    public void AddNewSolarCell()
    {
        var cell = Instantiate(SolarCellRef);
        SolarCellList.Add(cell);
    }

    // Update is called once per frame
    void Update()
    {
        while (SolarCellList.Count > maxSolarCell - 1)
        {
            Destroy(SolarCellList[0]);
            SolarCellList.RemoveAt(0);
        }

        DisplayCellsOnCanvas();

        _sliderBattery.value = batteryPercent * 0.01f;

        switch (true)
        {
            case var _ when batteryPercent < 10:
                //batteryPercentText.text = "0" + batteryPercent + "%";
                //batteryLevel50.enabled = false;
                //batteryLevel10.GetComponent<Image>().color = Color.red;
                batteryPercentText.text = "0" + batteryPercent;
                if (batteryPercent <= 0 && numberOfBatteries > 0)
                {
                    batteryPercent += 100;
                    numberOfBatteries--;
                    StartCoroutine(DrainBattery());
                } else if (batteryPercent <= 0 && numberOfBatteries == 0)
                {
                    //death logic here
                    print("Player Dies Here");
                    BatteryDisplay.SetActive(false);
                    this.gameObject.SetActive(false);
                }
                break;
            case var _ when batteryPercent >= 10:
                //batteryPercentText.text = batteryPercent + "%";
                if (batteryPercent > 100)
                {
                    batteryPercent = 100;
                }
                batteryPercentText.text = batteryPercent.ToString();
                break;

            default:
                break;
        }

        switch (true)
        {
            case var _ when batteryPercent < 10:
                _imageBatteryColor.color = Color.red;
                break;

            case var _ when batteryPercent >= 10 && batteryPercent < 50:
                _imageBatteryColor.color = Color.yellow;
                break;

            case var _ when batteryPercent >= 50:
                _imageBatteryColor.color = Color.green;
                break;
        }

        //switch (true)
        //{
        //    case var _ when batteryPercent > 50:
        //        batteryLevel50.enabled = true;
        //        batteryLevel100.enabled = true;
        //        batteryLevel10.GetComponent<Image>().color = Color.green;
        //        break;
        //    case var _ when batteryPercent > 10 && batteryPercent < 50:
        //        batteryLevel100.enabled = false;
        //        break;
        //    case var _ when batteryPercent < 10:
        //        batteryLevel50.enabled = false;
        //        batteryLevel10.GetComponent<Image>().color = Color.red;
        //        break;
        //    default:
        //        break;
        //}

        foreach (var cell in SolarCellList)
        {
            if (SolarCellList.IndexOf(cell) >= numberOfBatteries - 1)
            {
                cell.transform.Find("FillColor").GetComponent<Image>().color = Color.black;
            } else
            {
                cell.transform.Find("FillColor").GetComponent<Image>().color = Color.green;
            }
                
        }
    }

    IEnumerator DrainBattery()
    {
        while (batteryPercent > 0)
        {
            yield return new WaitForSeconds(drainTime);
            if (player.GetComponent<PlayerMovement>().velocity.x != 0)
            {
                batteryPercent -= drainSpeed;
            }
            
        }
    }
}
