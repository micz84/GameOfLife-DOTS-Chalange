using System.Globalization;
using Components;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace MonoBehaviours
{
    public class Manager:MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI simulationSizeValue;
        [SerializeField] private Slider simulationSize;
        [SerializeField] private Slider viewSize;
        [SerializeField] private TextMeshProUGUI viewSizeValue;
        [SerializeField] private TextMeshProUGUI viewSizeMaxValue;
        [SerializeField] private Button startSimulation;
        [SerializeField] private Button stopSimulation;
        [SerializeField] private Button exit;
        [SerializeField] private Slider seed;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI simulationSizePanel;
        [SerializeField] private TextMeshProUGUI viewSizePanel;

        private void Start()
        {
            infoPanel.SetActive(false);
            viewSizeValue.text = math.pow(2,viewSize.value).ToString(CultureInfo.InvariantCulture);
            simulationSizeValue.text = math.pow(2,simulationSize.value).ToString(CultureInfo.InvariantCulture);
            viewSizeMaxValue.text = math.pow(2,simulationSize.value).ToString(CultureInfo.InvariantCulture);
            
            
            viewSize.onValueChanged.AddListener(_ =>
            {
                viewSizeValue.text = math.pow(2,viewSize.value).ToString(CultureInfo.InvariantCulture);
            });
            
            simulationSize.onValueChanged.AddListener(_ =>
            {
                viewSize.maxValue = simulationSize.value;
                simulationSizeValue.text = math.pow(2,simulationSize.value).ToString(CultureInfo.InvariantCulture);
                viewSizeMaxValue.text = math.pow(2,simulationSize.value).ToString(CultureInfo.InvariantCulture);
            });
            startSimulation.onClick.AddListener(() =>
            {
                panel.SetActive(false);
                stopSimulation.gameObject.SetActive(true);
                var world = World.DefaultGameObjectInjectionWorld;
                var manager = world.EntityManager;
                var entity = manager.CreateEntity(ComponentType.ReadWrite<Config>());
                manager.SetName(entity, "Config");
                var viewWidth = (int) math.pow(2, viewSize.value);
                var simSize = (int)math.pow(2, simulationSize.value);
                manager.AddComponentData(entity, new Config()
                {
                    SimulationSize = simSize,
                    ViewSize = viewWidth,
                    Seed = (uint) seed.value
                });
                var startEntity = manager.CreateEntity(ComponentType.ReadWrite<StartSimulation>());
                manager.SetName(startEntity, "StartSimulation");
                
                manager.AddComponentData(entity, new SimulateTag());
                manager.AddComponentData(entity, new ViewPosition()
                {
                    Center = new int2(viewWidth/2, viewWidth/2)
                });
                infoPanel.SetActive(true);
                simulationSizePanel.text = $"{simSize}x{simSize}";
                UpdateCamera(viewWidth);
               
            });
            stopSimulation.onClick.AddListener(() =>
            {
                infoPanel.SetActive(false);
                panel.SetActive(true);
                stopSimulation.gameObject.SetActive(false);
                var world = World.DefaultGameObjectInjectionWorld;
                var manager = world.EntityManager;
                var entity = manager.CreateEntity(ComponentType.ReadWrite<StopSimulation>());
                manager.SetName(entity, "Stop");
            });
            
            exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        public void UpdateCamera(int viewWidth)
        {
            viewSizePanel.text = $"{viewWidth}x{viewWidth}";
            var halfViewSize = viewWidth / 2;
            mainCamera.transform.position = new Vector3(halfViewSize, halfViewSize, -10);
            mainCamera.orthographicSize = halfViewSize;
        }

        private void OnDestroy()
        {
            simulationSize.onValueChanged.RemoveAllListeners();
        }
    }
}