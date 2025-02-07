using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; 

public class TransparentDetector : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float transparentAmount = 0.8f;
    [SerializeField] private float fadeTime = 0.4f;

    private Tilemap tilemap;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>()) 
        {
            if (spriteRenderer)
            {
                StartCoroutine(FadeOutCoroutine(spriteRenderer, fadeTime, spriteRenderer.color.a, transparentAmount));
            }
            else if (tilemap)
            {
                StartCoroutine(FadeOutCoroutine(tilemap, fadeTime, tilemap.color.a, transparentAmount));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.GetComponent<PlayerController>()) 
        {
            if (spriteRenderer)
            {
                StartCoroutine(FadeOutCoroutine(spriteRenderer, fadeTime, spriteRenderer.color.a, 1f));
            }
            else if (tilemap)
            {
                StartCoroutine(FadeOutCoroutine(tilemap, fadeTime, tilemap.color.a, 1f));
            }
        }
    }

    private IEnumerator FadeOutCoroutine(SpriteRenderer spriteRenderer, float fadeTime, float startAlpha, float endAlpha)
    {
        float time = 0;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, endAlpha);
    }

    private IEnumerator FadeOutCoroutine(Tilemap tilemap, float fadeTime, float startAlpha, float endAlpha)
    {
        float time = 0;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeTime);
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha);
            yield return null;
        }
        tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, endAlpha);
    }
}
