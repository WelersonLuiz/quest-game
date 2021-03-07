using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastController : MonoBehaviour
{
    [SerializeField] GameObject MainCanvas;
    GameObject currentToast;

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject LoadToast()
    {
        return Resources.Load("Prefabs/Toast") as GameObject;
    }

    void CallToast(string message, string toastType)
    {
        currentToast = LoadToast();
        currentToast.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/" + toastType);
        currentToast.transform.GetChild(0).GetComponent<Text>().text = message;
        currentToast = Instantiate(currentToast, MainCanvas.transform);
    }

    public void CallGoodToast(string message)
    {
        CallToast(message, "goodToast");
        StartCoroutine(DestroyToast());
    }

    public void CallBadToast(string message)
    {
        CallToast(message, "badToast");
        StartCoroutine(DestroyToast());
    }

    IEnumerator DestroyToast()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(currentToast.gameObject);
    }

}
