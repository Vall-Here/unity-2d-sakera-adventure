using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string areaTransitionName;

    private float waitToLoad = 1f;
   
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<PlayerController>()) {
            SceneManagement.Instance.SetAreaTransitionName(areaTransitionName);
            UIFade.Instance.FadeToBlack();
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene(){
        while (waitToLoad >= 0){
            waitToLoad -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
