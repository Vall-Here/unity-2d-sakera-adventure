using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public bool gettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = 0.5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackForce)
    {
        gettingKnockedBack = true;
        Vector2 difference = (transform.position - damageSource.position).normalized;
        // Debug.Log("Knockback direction: " + difference);
        // rb.AddForce(knockBackForce * rb.mass * difference, ForceMode2D.Impulse);
        rb.AddForce(new Vector2(difference.x * knockBackForce * rb.mass, difference.y * knockBackForce * rb.mass), ForceMode2D.Impulse);

        StartCoroutine(KnockBackCoroutine());
    }


    private IEnumerator KnockBackCoroutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        gettingKnockedBack = false;
    }
}
