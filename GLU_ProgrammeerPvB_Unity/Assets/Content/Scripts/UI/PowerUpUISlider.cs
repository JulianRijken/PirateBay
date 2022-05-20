using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows a quick widget with info on the powerUp that is picked up and and duration  
/// </summary>
public class PowerUpUISlider : MonoBehaviour
{
    private Slider _slider;
    [SerializeField] private AnimationCurve _scaleCurve;
    [SerializeField] private Sprite[] _type;
    [SerializeField] private Image _frame;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        transform.localScale = Vector3.zero;
    }

    public void Hide()
    {
        transform.localScale = Vector3.zero;
    }
    
    /// <summary>
    /// Shows the UI widgit
    /// </summary>
    /// <param name="type"></param>
    /// <param name="time"></param>
    public void Show(EffectType type, float time)
    {
        _frame.sprite = _type[(int)type];

        gameObject.SetActive(true);
        
        StopAllCoroutines();
        StartCoroutine(ShowEnumerator(time));
    }

    /// <summary>
    /// Shows and scales UI element
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ShowEnumerator(float time)
    {
        var timer = 0f;
        
        while (timer <= time)
        {
            timer += Time.deltaTime;

            var alpha = Mathf.Clamp01(timer / time);
            
            transform.localScale = Vector3.one * _scaleCurve.Evaluate(alpha);
            _slider.value = Mathf.Abs(1 - alpha);
            
            
            yield return null;
        }
    }
}
