using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
   
    public string AreaTransitionName { get; private set; }

    public void SetAreaTransitionName(string areaTransitionName){
        this.AreaTransitionName = areaTransitionName;
    }

   
}
