using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float flashSpeed = 0.5f;
    
    private Coroutine _coroutine;

    public void Flash()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        image.enabled = true;
        image.color = new Color(image.color.r, image.color.g, image.color.b);
        _coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
            yield return null;
        }
        
        image.enabled = false;
    }
}
