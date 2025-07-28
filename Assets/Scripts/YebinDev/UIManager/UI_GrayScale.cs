using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GrayScale : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float duration = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void StartGrayScaleRoutine() {
        StartCoroutine(GrayscaleRoutine(duration,true));
    }

    public void Reset() {
        StartCoroutine(GrayscaleRoutine(duration,false));
    }

    private IEnumerator GrayscaleRoutine(float duration, bool isGrayScale) {
        float time = 0f;
        while (duration > time) {
            float durationFrame = Time.deltaTime;
            float ratio = time / durationFrame;
            float grayAmount = isGrayScale
                ? ratio  
                : 1-ratio;            
            
            SetGrayScale(grayAmount);
            time += durationFrame;
            yield return null;
        }
        SetGrayScale(isGrayScale ? 1:0);
    }

    public void SetGrayScale(float amount = 1) {
        spriteRenderer.material.SetFloat("_GrayscaleAmount",amount);

    }
}
