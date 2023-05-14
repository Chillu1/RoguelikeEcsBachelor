using RoguelikeEcs.Core.Utilities;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoguelikeEcs.Core
{
	public sealed class MainMenuUI : MonoBehaviour
	{
		private Button _newGameButton;
		private Button _smallAreaButton;
		private Button _mediumAreaButton;
		private Button _largeAreaButton;
		private Button _easyDifficultyButton;
		private Button _normalDifficultyButton;
		private Button _hardDifficultyButton;
		private Button _impossibleDifficultyButton;

		private Button _loadGameButton;
		private Button _hellWaveButton;
		private Button _quitButton;

		private GameObject _hoverPanel;
		private TMP_Text _hoverText;

		private DataController _dataController;

		//private Transform[] _saves;

		private void Start()
		{
			var dataController = FindObjectOfType<DataInitializer>().DataController;
			_dataController = dataController;

			var canvas = FindObjectOfType<Canvas>().transform;
			var versionText = canvas.Find("Version").GetComponent<TMP_Text>();
			_newGameButton = canvas.Find("NewGame").GetComponent<Button>();
			_loadGameButton = canvas.Find("LoadGame").GetComponent<Button>();
			_hellWaveButton = canvas.Find("HellWave").GetComponent<Button>();
			_quitButton = canvas.Find("Quit").GetComponent<Button>();

			versionText.text = "V" + DataController.Version;

			_newGameButton.onClick.AddListener(() =>
			{
				var dataController = FindObjectOfType<DataInitializer>().DataController;
				dataController.CurrentSaveName = "";
				dataController.HellWave = false;
				SceneManager.LoadScene("GameplayScene");
			});

			var automaticAimToggle = canvas.Find("AutomaticAim").GetComponent<Toggle>();
			automaticAimToggle.isOn = dataController.AutomaticAim;
			automaticAimToggle.onValueChanged.AddListener((value) => dataController.AutomaticAim = value);

			_smallAreaButton = canvas.Find("SmallArea").GetComponent<Button>();
			_smallAreaButton.onClick.AddListener(() => dataController.AreaSize = AreaSizeType.Small);
			_mediumAreaButton = canvas.Find("MediumArea").GetComponent<Button>();
			_mediumAreaButton.onClick.AddListener(() => dataController.AreaSize = AreaSizeType.Medium);
			_largeAreaButton = canvas.Find("LargeArea").GetComponent<Button>();
			_largeAreaButton.onClick.AddListener(() => dataController.AreaSize = AreaSizeType.Large);

			switch (dataController.AreaSize)
			{
				case AreaSizeType.Small:
					_smallAreaButton.onClick.Invoke();
					break;
				case AreaSizeType.Medium:
					_mediumAreaButton.onClick.Invoke();
					break;
				case AreaSizeType.Large:
					_largeAreaButton.onClick.Invoke();
					break;
			}

			_easyDifficultyButton = canvas.Find("Easy").GetComponent<Button>();
			_easyDifficultyButton.onClick.AddListener(() => dataController.Difficulty = DifficultyType.Easy);
			_normalDifficultyButton = canvas.Find("Normal").GetComponent<Button>();
			_normalDifficultyButton.onClick.AddListener(() => dataController.Difficulty = DifficultyType.Normal);
			_hardDifficultyButton = canvas.Find("Hard").GetComponent<Button>();
			_hardDifficultyButton.onClick.AddListener(() => dataController.Difficulty = DifficultyType.Hard);
			_impossibleDifficultyButton = canvas.Find("Impossible").GetComponent<Button>();
			_impossibleDifficultyButton.onClick.AddListener(() => dataController.Difficulty = DifficultyType.Impossible);

			switch (dataController.Difficulty)
			{
				case DifficultyType.Easy:
					_easyDifficultyButton.onClick.Invoke();
					break;
				case DifficultyType.Normal:
					_normalDifficultyButton.onClick.Invoke();
					break;
				case DifficultyType.Hard:
					_hardDifficultyButton.onClick.Invoke();
					break;
				case DifficultyType.Impossible:
					_impossibleDifficultyButton.onClick.Invoke();
					break;
			}

			var charactersPanel = canvas.Find("Characters");
			int i = 0;
			foreach (var characterInfo in dataController.CharacterData.GetCharactersInfo())
			{
				var sprite = Resources.Load<Sprite>("Textures/Player" + characterInfo.Type);
				if (sprite == null)
					sprite = Resources.Load<Sprite>("Textures/PlayerDefault");
				var goChild = charactersPanel.GetChild(i);
				goChild.GetComponentInChildren<Image>().sprite = sprite;
				goChild.GetComponentInChildren<HoverOverMainMenu>().Setup(OnHover, OnStopHover, characterInfo);
				var button = goChild.GetComponentInChildren<Button>();
				button.GetComponent<Image>().sprite = sprite;
				var buttonSpriteState = button.spriteState;
				buttonSpriteState.disabledSprite = sprite;
				button.spriteState = buttonSpriteState;
				button.onClick.AddListener(() => dataController.PlayerCharacter = characterInfo.Type);
				button.interactable = dataController.GameData.UnlockedCharacters.Contains(characterInfo.Type) ||
				                      characterInfo.Type == CharacterType.Default;

				i++;
			}

			for (; i < charactersPanel.childCount; i++)
				charactersPanel.GetChild(i).gameObject.SetActive(false);

			_loadGameButton.interactable = SaveStateController.SaveExists(SaveStateController.DefaultSaveFileName);
			_loadGameButton.onClick.AddListener(() =>
			{
				var dataController = FindObjectOfType<DataInitializer>().DataController;
				if (!SaveStateController.SaveExists(SaveStateController.DefaultSaveFileName))
					return;

				dataController.CurrentSaveName = SaveStateController.GetLatestSaveName();
				dataController.HellWave = false;
				SceneManager.LoadScene("GameplayScene");
			});
			_hellWaveButton.onClick.AddListener(() =>
			{
				var dataController = FindObjectOfType<DataInitializer>().DataController;
				dataController.CurrentSaveName = "";
				dataController.HellWave = true;
				SceneManager.LoadScene("GameplayScene");
			});

			_quitButton.onClick.AddListener(() =>
			{
#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			});

			_hoverPanel = canvas.transform.Find("HoverPanel").gameObject;
			_hoverText = _hoverPanel.GetComponentInChildren<TMP_Text>();
			_hoverPanel.SetActive(false);
		}

		private void OnHover(Vector3 position, CharacterInfo characterInfo)
		{
			_hoverPanel.SetActive(true);
			_hoverPanel.TweenAppearCanvasGroupCancel();

			//if position is too close to the right side of the screen, move the panel to the left
			if (position.x > Screen.width - 350f)
				position.x -= 300f;
			_hoverPanel.transform.position = position + new Vector3(150f, -90f);
			bool unlocked = _dataController.GameData.UnlockedCharacters.Contains(characterInfo.Type);
			string text = characterInfo.Name + "\n";
			text += unlocked ? characterInfo.Description + "\n" + "Unlocked" : characterInfo.UnlockText;

			_hoverText.text = text;
		}

		private void OnStopHover()
		{
			_hoverPanel.TweenDisappearCanvasGroup()
				.SetOnCancel(() => _hoverPanel.GetComponent<CanvasGroup>().alpha = 0f)
				.SetOnComplete(() => _hoverPanel.SetActive(false));
		}
	}
}