// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemyPathFinding : MonoBehaviour
// {
//     // Start is called before the first frame update
//     [SerializeField] private float speed;

//     private Rigidbody2D rb;
//     private Vector2 movement;
//     private KnockBack knockBack;

//     private void Awake() {
//         knockBack = GetComponent<KnockBack>();
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void FixedUpdate() {
//         if (knockBack.gettingKnockedBack) {return;}
//         rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
//     }

//     public void MoveTo(Vector2 targetPosition)
//         {
//             // Debug: Melihat target posisi
//             // Debug.Log("Moving towards: " + targetPosition);

//             movement = targetPosition;
//             // Hitung arah gerakan
//             // Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
//             // transform.position = (Vector2)transform.position + direction * speed * Time.deltaTime;

//             // Debug: Lihat posisi saat ini
//             // Debug.Log("Current position: " + transform.position);
//         }


//     public void Stop() {
//         rb.velocity = Vector2.zero;
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private Vector2 movement;
    private KnockBack knockBack;

    private void Awake() {
        knockBack = GetComponent<KnockBack>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (knockBack.gettingKnockedBack) { return; }
        rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
    }

    public void MoveTo(Vector2 targetPosition) {
        Vector2 direction = (targetPosition - rb.position).normalized; 
        movement = direction;
    }

    public void Stop() {
        movement = Vector2.zero;
    }
}
