using Service.Locator;
using UnityEngine;


namespace Service.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public class ServiceBootstrap : MonoBehaviour
    {
 
        [SerializeField] private HintView _hintView;

        private void Awake()
        {
        }

        private void OnApplicationQuit()
        {
            ServiceLocator.Clear();
        }
    }

}
