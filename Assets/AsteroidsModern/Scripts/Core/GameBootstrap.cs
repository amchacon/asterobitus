using UnityEngine;

namespace AsteroidsModern.Core
{ 
    public class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            ConfigureApplication();
        }


        private void ConfigureApplication()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            
            Input.multiTouchEnabled = false;
            
            Physics2D.queriesStartInColliders = false;
            Physics2D.callbacksOnDisable = true;
        }
    }
}