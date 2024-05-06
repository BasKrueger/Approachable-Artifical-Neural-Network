using UnityEngine;
using UnityEngine.UI;

namespace ApproachableANN.Demo.Race
{
    /// <summary>
    /// Handles the UI events of the racing demo scene
    /// </summary>
    public sealed class RacingDemo : MonoBehaviour
    {
        private const float MAX_GAME_SPEED = 3;

        [Header("UI References")]

        [SerializeField]
        private Toggle trainWithBoundaries;
        [SerializeField]
        private Toggle trainWithStripes;
        [SerializeField]
        private Button showTrainButton;
        [SerializeField]
        private Button skipTrainButton;
        [SerializeField]
        private ANNUnit templateCar;

        [Header("Pre trained AIs")]

        [SerializeField]
        private ANNObject trained_none;
        [SerializeField]
        private ANNObject trained_Boundary;
        [SerializeField]
        private ANNObject trained_Stripes;
        [SerializeField]
        private ANNObject trained_Boundary_Stripes;

        private ANNTrainer trainer;
        private Boundary[] boundaries;
        private RewardBox[] rewardBoxes;
        private ANNUnit trainedCar;

        private void Awake()
        {
            trainer = GetComponentInChildren<ANNTrainer>();
            boundaries = GetComponentsInChildren<Boundary>();
            rewardBoxes = GetComponentsInChildren<RewardBox>();

            templateCar.gameObject.SetActive(false);

            trainWithBoundaries.onValueChanged.AddListener(OnToggleBoundary);
            trainWithStripes.onValueChanged.AddListener(OnToggleStripes);
            showTrainButton.onClick.AddListener(OnShowTraining);
            skipTrainButton.onClick.AddListener(OnSkipTraining);
        }

        private void Start()
        {
            OnSkipTraining();
        }

        private void OnToggleBoundary(bool state)
        {
            foreach (var boundary in boundaries)
            {
                boundary.active = state;
            }

            if (!skipTrainButton.interactable)
            {
                OnSkipTraining();
            }
        }

        private void OnToggleStripes(bool state)
        {
            foreach (var box in rewardBoxes)
            {
                box.gameObject.SetActive(state);
            }

            if (!skipTrainButton.interactable)
            {
                OnSkipTraining();
            }
        }

        private void OnShowTraining()
        {
            if (trainedCar != null)
            {
                Destroy(trainedCar.gameObject);
            }

            skipTrainButton.interactable = true;
            showTrainButton.interactable = false;

            trainer.Stop();
            trainer.Train();
        }

        private void OnSkipTraining()
        {
            if (trainedCar != null)
            {
                Destroy(trainedCar.gameObject);
            }

            skipTrainButton.interactable = false;
            showTrainButton.interactable = true;

            trainer.Stop();

            trainedCar = Instantiate(templateCar, templateCar.transform.parent);
            trainedCar.OverrideAI(new ArtificalNeuralNetwork(GetAIToLoad()));
            trainedCar.gameObject.SetActive(true);
        }

        public void OnSpeedSliderValueChanged(float value)
        {
            Time.timeScale = 1 + value * MAX_GAME_SPEED;
        }

        private ANNObject GetAIToLoad()
        {
            if (!trainWithBoundaries.isOn && !trainWithStripes.isOn)
            {
                return trained_none;
            }

            if (trainWithBoundaries.isOn && trainWithStripes.isOn)
            {
                return trained_Boundary_Stripes;
            }

            if (trainWithBoundaries.isOn)
            {
                return trained_Boundary;
            }

            if (trainWithStripes.isOn)
            {
                return trained_Stripes;
            }

            return null;
        }
    }

}
