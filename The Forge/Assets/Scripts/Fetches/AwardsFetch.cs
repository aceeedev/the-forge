using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using System.Collections;

public class AwardsFetch : MonoBehaviour
{

    public GameObject award1WinnerTextObject;
    public GameObject award2WinnerTextObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        award1WinnerTextObject = GameObject.FindGameObjectWithTag("Award1Winner");
        award2WinnerTextObject = GameObject.FindGameObjectWithTag("Award2Winner");
    }

    void Update()
    {
        if (GameManager.inst != null)
        {
            TextMeshProUGUI award1WinnerTextComponent = award1WinnerTextObject.GetComponent<TextMeshProUGUI>();
            if (award1WinnerTextComponent != null)
            {
                award1WinnerTextComponent.text = GameManager.inst.award1Winner;
            }
            TextMeshProUGUI award2WinnerTextComponent = award2WinnerTextObject.GetComponent<TextMeshProUGUI>();
            if (award2WinnerTextComponent != null)
            {
                award2WinnerTextComponent.text = GameManager.inst.award2Winner;
            }
        }
    }
}
