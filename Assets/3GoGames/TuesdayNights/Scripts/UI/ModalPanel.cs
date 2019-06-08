using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.Collections;

public class EventButtonDetails
{
    public string buttonTitle;
    public Sprite buttonBackground;
    public UnityAction action;
}

public class ModalPanelDetails
{
    public string title;
    public string message;
    public Sprite iconImage;
    public Sprite panelBackgroundImage;
    public EventButtonDetails button1Details;
    public EventButtonDetails button2Details;
    public EventButtonDetails button3Details;
    public EventButtonDetails button4Details;
}

public class ModalPanel : MonoBehaviour
{
    public Text message;
    public Image iconImage;
    public Button button1;
    public Button button2;
    public Button button3;

    public Text button1Text;
    public Text button2Text;
    public Text button3Text;

    public GameObject modalPanelObject;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        ClosePanel();

        // Set static reference.

        ModalPanelHandle.RegisterHandleMain(this);
    }

    void OnDestroy()
    {
        ModalPanelHandle.UnregisterHandleMain(this);
    }

    // BUSINESS LOGIC

    public void OpenPanel(ModalPanelDetails details)
    {
        if (modalPanelObject == null)
            return;

        modalPanelObject.SetActive(true);

        // DIAGLOG MESSAGE

        if (message != null)
        {
            message.text = details.message;
        }

        // DIALOG ICON

        if (iconImage != null)
        {
            if (details.iconImage)
            {
                iconImage.sprite = details.iconImage;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }

        // BUTTON 1

        if (button1 != null)
        {
            if (details.button1Details != null)
            {
                button1.onClick.RemoveAllListeners();
                button1.onClick.AddListener(details.button1Details.action);
                button1.onClick.AddListener(ClosePanel);
                button1.gameObject.SetActive(true);

                if (button1Text != null)
                {
                    button1Text.text = details.button1Details.buttonTitle;
                }
            }
            else
            {
                button1.gameObject.SetActive(false);
            }
        }

        // BUTTON 2

        if (button2 != null)
        {
            if (details.button2Details != null)
            {
                button2.onClick.RemoveAllListeners();
                button2.onClick.AddListener(details.button2Details.action);
                button2.onClick.AddListener(ClosePanel);
                button2.gameObject.SetActive(true);

                if (button2Text != null)
                {
                    button2Text.text = details.button2Details.buttonTitle;
                }
            }
            else
            {
                button2.gameObject.SetActive(false);
            }
        }

        // BUTTON 3

        if (button3 != null)
        {
            if (details.button3Details != null)
            {
                button3.onClick.RemoveAllListeners();
                button3.onClick.AddListener(details.button3Details.action);
                button3.onClick.AddListener(ClosePanel);
                button3.gameObject.SetActive(true);

                if (button2Text != null)
                {
                    button3Text.text = details.button3Details.buttonTitle;
                }
            }
            else
            {
                button3.gameObject.SetActive(false);
            }
        }
    }

    // INTERNALS

    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}