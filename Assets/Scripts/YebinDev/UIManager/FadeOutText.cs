using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeOutText : MonoBehaviour
{
    Text text;

    private void Start() {
        StartCoroutine("DestroyText");
        text = GetComponent<Text>();
    }

     IEnumerator FadeOut()
    {
        while(text.color.a > 0)
        {
            Color color = text.color;
            color.a -= Time.deltaTime;
            text.color = color;
            yield return null;
        }
    }

    IEnumerator DestroyText()
    {
        yield return new WaitForSeconds(4f);

        StartCoroutine("FadeOut");

        Destroy(gameObject, 2f);
    }
}
