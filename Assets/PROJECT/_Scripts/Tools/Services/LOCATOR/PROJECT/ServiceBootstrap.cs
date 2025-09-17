using Service.Locator;
using UnityEngine;


namespace Service.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public class ServiceBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            //ServiceLocator.Register<IMyService>(new MyService());

            ServiceLocator.InitializeAll();
            // if (SceneServiceLocator.Current) SceneServiceLocator.Current.InitializeAll();
        }

        private void OnApplicationQuit()
        {
            ServiceLocator.Clear(dispose: true);
        }
    }

}
