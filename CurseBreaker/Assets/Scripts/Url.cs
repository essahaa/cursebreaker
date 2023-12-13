using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Url : MonoBehaviour
{
   public void OpenTermsOfService()
    {
        Application.OpenURL("https://1drv.ms/b/s!As9lMVGyrtP8aa76tMSFn6AIRwY?e=BGBoKb");
    }
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://1drv.ms/b/s!As9lMVGyrtP8ahCQZ5D9W3JS5Es?e=8EmaYJ");
    }
}
