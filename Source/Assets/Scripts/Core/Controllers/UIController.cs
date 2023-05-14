using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using RoguelikeEcs.Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RoguelikeEcs.Core
{
	public sealed class UIController
	{
		private readonly World _world;
		private readonly IReadOnlyList<WaveInfo> _waveInfo;
		private readonly Dictionary<string, Sprite> _enemyTextures;
		private readonly Sprite _missingTexture;

		private readonly Canvas _canvas;
		private readonly Image _fade;
		private readonly GameObject _pausePanel;
		private readonly GameObject _gameOverPanel;

		private readonly GameObject _hoverPanel;
		private readonly TMP_Text _hoverText;
		private readonly GameObject _unlockPanel;
		private readonly TMP_Text _unlockText;

		private readonly TMP_Text _statsText;
		private readonly GameObject _mechanicsHoverPanel;
		private readonly TMP_Text[] _mechanicsHoverTexts;
		private readonly GameObject _shop;
		private readonly Vector3 _shopOriginalPos;
		private readonly GameObject _upgrades;
		private readonly TMP_Text[] _upgradeNames;
		private readonly TMP_Text[] _upgradeCostTexts;
		private readonly float[] _upgradePrices;
		private readonly HoverOver[] _upgradeHoverOver;
		private readonly Button[] _upgradeButtons;
		private readonly Image[] _upgradePanelImages;
		private readonly GameObject _allUpgradesPanel;
		private readonly GameObject _nextWavePanel;
		private readonly Image[] _nextWavePanelImages;
		private readonly TMP_Text[] _nextWavePanelTexts;

		//private readonly Tween<Vector3>[] _upgradeButtonAffordableTweens;

		private readonly TMP_Text _countdown;

		//private readonly TMP_Text _scoreText;
		private readonly TMP_Text _goldText;
		private readonly TMP_Text _waveNumberText;
		private readonly TMP_Text _waveTimeText;
		private readonly TMP_Text _streakText;

		private UpgradeType[] _currentUpgrades;
		private bool _upgradesLocked;

		public UIController(World world, WaveData waveData)
		{
			_world = world;
			_waveInfo = waveData.GetWaves();

			_enemyTextures = new Dictionary<string, Sprite>();
			foreach (var enemyTexture in Resources.LoadAll<Sprite>("Textures"))
				_enemyTextures.Add(enemyTexture.name, enemyTexture);
			_missingTexture = Resources.Load<Sprite>("Textures/MissingTexture");

			_canvas = Object.FindObjectOfType<Canvas>();
			_fade = _canvas.transform.Find("Fade").GetComponent<Image>();
			_fade.gameObject.SetActive(false);

			_pausePanel = _canvas.transform.Find("PausePanel").gameObject;
			_pausePanel.transform.Find("Resume").GetComponent<Button>().onClick.AddListener(() =>
			{
				_pausePanel.SetActive(false);
				_world.Publish(new PlayerResumeEvent());
			});
			_pausePanel.transform.Find("MainMenu").GetComponent<Button>().onClick.AddListener(() =>
			{
				_world.Publish(new GameDataUpdatedEvent());
				_world.Publish(new SceneActionEvent(SceneType.MainMenu));
			});
			_pausePanel.transform.Find("Quit").GetComponent<Button>().onClick.AddListener(() => _world.Publish(new QuitActionEvent()));
			_pausePanel.SetActive(false);

			_gameOverPanel = _canvas.transform.Find("GameOverPanel").gameObject;
			_gameOverPanel.transform.Find("TryAgain").GetComponent<Button>().onClick.AddListener(() =>
			{
				_world.Publish(new SceneActionEvent(SceneType.Game));
				//Avoids game pausing on new game, for some reason one frame of the old world sneaks in here
				_world.GetEntities().With<IsPlayerComp>().AsSet().GetFirst().Get<HealthComp>().Reset();
			});
			_gameOverPanel.transform.Find("MainMenu").GetComponent<Button>().onClick.AddListener(() =>
			{
				_world.Publish(new SceneActionEvent(SceneType.MainMenu));
			});
			_gameOverPanel.transform.Find("Quit").GetComponent<Button>().onClick.AddListener(() => _world.Publish(new QuitActionEvent()));
			_statsText = _gameOverPanel.transform.Find("StatsPanel").Find("StatsText").GetComponentInChildren<TMP_Text>();
			_gameOverPanel.SetActive(false);

			_hoverPanel = _canvas.transform.Find("HoverPanel").gameObject;
			_hoverText = _hoverPanel.GetComponentInChildren<TMP_Text>();
			_hoverPanel.SetActive(false);

			_unlockPanel = _canvas.transform.Find("UnlockPanel").gameObject;
			_unlockText = _unlockPanel.GetComponentInChildren<TMP_Text>();

			_mechanicsHoverPanel = _canvas.transform.Find("MechanicsHoverPanel").gameObject;
			_mechanicsHoverTexts = _mechanicsHoverPanel.GetComponentsInChildren<TMP_Text>();
			_mechanicsHoverPanel.SetActive(false);

			_shop = _canvas.transform.Find("Shop").gameObject;
			_shopOriginalPos = _shop.transform.localPosition;
			_upgrades = _shop.transform.Find("Upgrades").gameObject;
			int upgradesCount = _upgrades.transform.childCount;
			_upgradeNames = new TMP_Text[upgradesCount];
			_upgradeCostTexts = new TMP_Text[upgradesCount];
			_upgradePrices = new float[upgradesCount];
			_upgradeHoverOver = new HoverOver[upgradesCount];
			_upgradeButtons = new Button[upgradesCount];
			_upgradePanelImages = new Image[upgradesCount];
			//_upgradeButtonAffordableTweens = new Tween<Vector3>[upgradesCount];
			for (int i = 0; i < upgradesCount; i++)
			{
				var upgradeData = _upgrades.transform.GetChild(i);
				var hoverOver = upgradeData.gameObject.GetComponent<HoverOver>();
				hoverOver.Setup(_world);
				_upgradeHoverOver[i] = hoverOver;
				_upgradeNames[i] = upgradeData.Find("UpgradeName").GetComponent<TMP_Text>();
				_upgradeCostTexts[i] = upgradeData.Find("UpgradeCost").GetComponent<TMP_Text>();

				var upgradeButton = upgradeData.Find("UpgradeButton").GetComponent<Button>();
				var upgradeText = upgradeButton.GetComponentInChildren<TMP_Text>();
				int index = i;
				upgradeButton.onClick.AddListener(() =>
				{
					float upgradePrice = _upgradePrices[index];
					ref var gold = ref world.Get<GoldComp>();
					if (gold.Value < upgradePrice)
					{
						upgradeButton.TweenError();
						_goldText.TweenFlashColor(Color.red);
						return;
					}

					_world.Get<ShopUpgradesComp>().IsBought[index] = true;
					UpdateGoldText(-upgradePrice);
					gold.Value -= upgradePrice;
					_world.Publish(new UpgradeChosenEvent(_currentUpgrades![index]));
					upgradeButton.TweenFlashColor(Color.green, 0.4f);
					upgradeButton.TweenFlashScale(1.2f, 0.4f);
					upgradeButton.interactable = false;
					upgradeText.text = "Bought";

					//UpdateAffordableUpgradeTweens();
				});

				//_upgradeButtonAffordableTweens[i] = upgradeButton.TweenLocalScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f)
				//	.SetEase(EaseType.SineOut).SetPingPong().SetInfinite();
				//_upgradeButtonAffordableTweens[i].SetPaused(true);

				_upgradeButtons[i] = upgradeButton;
				_upgradePanelImages[i] = upgradeData.Find("UpgradePanel").GetComponent<Image>();
			}

			_allUpgradesPanel = _shop.transform.Find("AllUpgrades").gameObject;
			_allUpgradesPanel.SetActive(false);

			var showUpgradesButton = _shop.transform.Find("ShowUpgrades").GetComponent<Button>();
			showUpgradesButton.onClick.AddListener(() =>
			{
				bool active = _allUpgradesPanel.activeSelf;
				showUpgradesButton.GetComponentInChildren<TMP_Text>().text = active ? "Show Upgrades" : "Hide Upgrades";
				_allUpgradesPanel.SetActive(!active);
				if (!active)
				{
					_allUpgradesPanel.transform.Find("Upgrades").GetComponent<TMP_Text>().text =
						string.Join("\n", _world.Get<UpgradesComp>().UpgradesData.Select(x => x.Key + ": " + x.Value));
				}
			});

			var enrageWaveButton = _shop.transform.Find("EnrageWave").GetComponent<Button>();
			enrageWaveButton.interactable = false; //TODO
			enrageWaveButton.onClick.AddListener(() =>
			{
				//TODO Gold Multiplier 1.5f
				bool enraged = _world.Get<DifficultyComp>().ToggleEnrage(); //TODO Old wave info with old enrage bool
				enrageWaveButton.GetComponent<Image>().color = enraged ? Color.red : Color.white; //TODO Tween
			});

			_shop.transform.Find("NextWave").GetComponent<Button>().onClick.AddListener(() =>
			{
				OnMechanicsStopHover(default);
				_fade.gameObject.TweenDisappearCanvasGroup().SetOnComplete(() => _fade.gameObject.SetActive(false));
				_shop.TweenCanvasGroupAlpha(0f, 0.6f)
					.TweenLocalPositionY(_shopOriginalPos.y + 1000, 0.4f).SetEase(EaseType.BackIn)
					.SetOnComplete(() =>
					{
						_shop.SetActive(false);
						_shop.transform.localPosition = _shopOriginalPos;

						const string waveTimeText = "Wave time: ";
						_waveTimeText.TweenValueFloat(WaveSystem.Interval, 0.8f,
							f => _waveTimeText!.text = waveTimeText + f.ToString("F1"));
						_countdown!.gameObject.SetActive(true);
						_countdown.gameObject
							.TweenAppearCanvasGroupCancel().TweenLocalScale(new Vector3(0.3f, 0.3f, 0.3f), 1f).SetEase(EaseType.CubicOut)
							.TweenDisappearCanvasGroup().SetDelay(0.55f)
							.SetOnComplete(() =>
							{
								_world.Get<WaveInfoComp>().WaveFinished = false;
								_countdown!.gameObject.SetActive(false);
								_countdown.gameObject.transform.localScale = Vector3.one;
								TimeTween.Tween(1f, 1.2f);
							});
						_world.Publish(new ResumeActionEvent());
					});
			});

			var lockText = _shop.transform.Find("Lock").GetComponentInChildren<TMP_Text>();
			var lockButton = _shop.transform.Find("Lock").GetComponent<Button>();
			lockButton.onClick.AddListener(() =>
			{
				_upgradesLocked = !_upgradesLocked;
				lockText.text = _upgradesLocked ? "Unlock" : "Lock";

				for (int i = 0; i < _upgradeButtons.Length; i++)
					_upgradeButtons[i].TweenChanged(_upgradesLocked);

				lockButton.TweenToggleEnabled(_upgradesLocked);
			});

			_shop.transform.Find("RefreshCost").GetComponent<TMP_Text>().text = "50 gold";
			var refreshButton = _shop.transform.Find("Refresh").GetComponent<Button>();
			refreshButton.onClick.AddListener(() =>
			{
				ref var gold = ref world.Get<GoldComp>();
				if (gold.Value < 50f)
				{
					_goldText.TweenFlashColor(Color.red);
					refreshButton.TweenError();
					return;
				}

				if (_upgradesLocked)
				{
					lockButton.TweenError();
					return;
				}

				UpdateGoldText(-50f);
				gold.Value -= 50f;

				_shop.TweenLocalScale(Vector3.one * 1.2f, 0.3f).SetEase(EaseType.CubicIn)
					.SetOnComplete(() => _world.Publish(new ShopRefreshEvent()))
					.TweenLocalScale(Vector3.one, 0.3f).SetEase(EaseType.CubicOut).SetDelay(0.3f);
			});

			_nextWavePanel = _shop.transform.Find("NextWaveInfo").gameObject;
			int nextWavePanelChildCount = _nextWavePanel.transform.childCount;
			_nextWavePanelImages = new Image[nextWavePanelChildCount];
			_nextWavePanelTexts = new TMP_Text[nextWavePanelChildCount];
			for (int i = 0; i < nextWavePanelChildCount; i++)
			{
				var image = _nextWavePanel.transform.GetChild(i).GetComponent<Image>();
				image.gameObject.SetActive(false);
				image.gameObject.GetComponent<HoverOver>().Setup(world);
				_nextWavePanelImages[i] = image;
				_nextWavePanelTexts[i] = image.GetComponentInChildren<TMP_Text>();
			}

			_shop.SetActive(false);

			var tutorial = _canvas.transform.Find("Tutorial");
			tutorial.TweenCanvasGroupAlpha(0f, 2.5f).SetEase(EaseType.CubicIn).SetDelay(0.5f)
				.SetOnComplete(() => tutorial.gameObject.SetActive(false));

			_countdown = _canvas.transform.Find("Countdown").GetComponent<TMP_Text>();
			_countdown.gameObject.SetActive(false);

			//_scoreText = _canvas.transform.Find("Score").GetComponent<TMP_Text>();
			_goldText = _canvas.transform.Find("Gold").GetComponent<TMP_Text>();
			_waveNumberText = _canvas.transform.Find("WaveNumber").GetComponent<TMP_Text>();
			_waveTimeText = _canvas.transform.Find("WaveTime").GetComponent<TMP_Text>();
			_streakText = _canvas.transform.Find("Streak").GetComponent<TMP_Text>();

			_currentUpgrades = new UpgradeType[upgradesCount];

			_world.Subscribe<PauseChangedEvent>(OnGamePaused);
			_world.Subscribe<EnemyHoverOverEvent>(OnEnemyHover);
			_world.Subscribe<EnemyStopHoverEvent>(OnEnemyStopHover);
			_world.Subscribe<UnlockEvent>(UpdateUnlockPanel);
			_world.Subscribe<MechanicsHoverOverEvent>(OnMechanicsHover);
			_world.Subscribe<MechanicsStopHoverEvent>(OnMechanicsStopHover);
			_world.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
			_world.Subscribe<NewUpgradesEvent>(OnNewUpgrades);
			_world.Subscribe<ScoreUpdatedEvent>(OnScoreUpdated);
			_world.Subscribe<GameOverEvent>(OnGameOver);
		}

		public void Update(float delta)
		{
			ref readonly var waveInfo = ref _world.Get<WaveInfoComp>();
			_waveNumberText.text = $"Wave: {waveInfo.WaveNumber + 1}";
			_waveTimeText.text = $"Wave time: {waveInfo.Time:F1}";
			ref readonly var killStreakComp = ref _world.Get<KillStreakComp>();
			//TODO RichText color timeLeft
			_streakText.text = $"Streak: {killStreakComp.Streak}, Time: {killStreakComp.TimeLeft:F1}";
		}

		private void OnGamePaused(in PauseChangedEvent message)
		{
			if (message.PlayerPaused)
			{
				_fade.gameObject.SetActive(true);
				_pausePanel.SetActive(true);
				_fade.gameObject.TweenAppearCanvasGroup().SetFrom(0f)
					.SetOnCancel(() => _fade.gameObject.GetComponent<CanvasGroup>().alpha = 1f);
				_pausePanel.TweenAppearCanvasGroupCancel();
			}
			else
			{
				_fade.gameObject.SetActive(false);
				_pausePanel.SetActive(false);
			}
		}

		private void OnEnemyHover(in EnemyHoverOverEvent message)
		{
			_hoverPanel.SetActive(true);
			_hoverPanel.TweenAppearCanvasGroupCancel();

			_hoverPanel.transform.position = message.Position + new Vector3(150f, -90f);
			_hoverText.text = message.EnemyType.ToString();
			//var enemyInfo = Wiki.GetEnemyInfo(message.EnemyType);
			//_enemyHoverText.text = enemyInfo;
		}

		private void OnEnemyStopHover(in EnemyStopHoverEvent message)
		{
			_hoverPanel.TweenDisappearCanvasGroup()
				.SetOnCancel(() => _hoverPanel.GetComponent<CanvasGroup>().alpha = 0f)
				.SetOnComplete(() => _hoverPanel.SetActive(false));
		}

		private void UpdateUnlockPanel(in UnlockEvent message)
		{
			if (_unlockPanel.ContainsTween())
			{
				_unlockText.text += ", " + message.Info.DisplayString;
				return;
			}

			_unlockText.text = $"Unlocked:\n{message.Info.DisplayString}";
			var position = _unlockPanel.transform.localPosition;
			_unlockPanel
				.TweenLocalPositionY(position.y + 225f, 0.8f).SetEase(EaseType.BackOut).UseScaledTime()
				.TweenCanvasGroupAlpha(1f, 1.4f).SetEase(EaseType.BackOut).SetFrom(0f).UseScaledTime()
				.TweenLocalPositionY(position.y, 0.5f).SetEase(EaseType.BackIn).SetDelay(5f).UseScaledTime()
				.TweenCanvasGroupAlpha(0f, 0.6f).SetDelay(5f).UseScaledTime();
		}

		private void OnMechanicsHover(in MechanicsHoverOverEvent message)
		{
			if (message.MechanicTypes == null || message.MechanicTypes.Length == 0)
			{
				_mechanicsHoverPanel.SetActive(false);
				return;
			}

			_mechanicsHoverPanel.SetActive(true);
			_mechanicsHoverPanel.TweenAppearCanvasGroupCancel();

			//_hoverPanel.transform.position = message.Position - new Vector2(-300f, 300f);
			for (int i = 0; i < message.MechanicTypes.Length; i++)
			{
				if (i >= _mechanicsHoverTexts.Length)
					break;

				var mechanicInfo = Wiki.GetMechanicInfo(message.MechanicTypes[i]);
				_mechanicsHoverTexts[i].text = $"{mechanicInfo.Name}\n{mechanicInfo.Description}";
				_mechanicsHoverTexts[i].enabled = true;
			}

			for (int i = message.MechanicTypes.Length; i < _mechanicsHoverTexts.Length; i++)
				_mechanicsHoverTexts[i].enabled = false;
		}

		private void OnMechanicsStopHover(in MechanicsStopHoverEvent message)
		{
			_mechanicsHoverPanel.TweenDisappearCanvasGroup()
				.SetOnCancel(() => _mechanicsHoverPanel.GetComponent<CanvasGroup>().alpha = 0f)
				.SetOnComplete(() => _mechanicsHoverPanel.SetActive(false));
		}

		private void OnWaveCompleted(in WaveCompletedEvent message)
		{
			_fade.gameObject.SetActive(true);
			_fade.gameObject.TweenAppearCanvasGroupCancel();

			int waveNumber = _world.Get<WaveInfoComp>().WaveNumber;
			var waveInfo = _waveInfo[waveNumber];
			for (int i = 0; i < waveInfo.EnemySpawnData.Count; i++)
			{
				if (i >= _nextWavePanelImages.Length)
					break;

				var enemySpawnData = waveInfo.EnemySpawnData[i];
				string enemyName = enemySpawnData.Type.ToString();
				bool isBoss = false;
				if (enemyName.Contains("Boss"))
				{
					enemyName = enemyName.Replace("Boss", "");
					isBoss = true;
				}

				_nextWavePanelImages[i].sprite = _enemyTextures.TryGetValue(enemyName, out var texture) ? texture : _enemyTextures["Enemy"];
				_nextWavePanelImages[i].gameObject.GetComponent<HoverOver>().Set(enemySpawnData.Type);

				switch (enemySpawnData)
				{
					case EnemySpawnData data:
						_nextWavePanelImages[i].transform.localScale =
							Vector3.one * Mathf.Lerp(0.6f, 1.4f, data.SpawnWeight / waveInfo.TotalSpawnWeight);
						_nextWavePanelTexts[i].text = "";
						break;
					case EnemyTimeSpawnData data:
						_nextWavePanelImages[i].transform.localScale = Vector3.one;
						_nextWavePanelTexts[i].text = isBoss ? $"B x{data.Count}" : $"x{data.Count}";
						break;
				}

				_nextWavePanelImages[i].gameObject.SetActive(true);
			}

			for (int i = waveInfo.EnemySpawnData.Count; i < _nextWavePanelImages.Length; i++)
			{
				_nextWavePanelImages[i].gameObject.SetActive(false);
				_nextWavePanelTexts[i].text = string.Empty;
			}
		}

		private void OnNewUpgrades(in NewUpgradesEvent message)
		{
			_shop.SetActive(true);
			if (!message.IsRefresh)
			{
				_shop.transform.localPosition = new Vector3(_shopOriginalPos.x, _shopOriginalPos.y - 1000f, _shopOriginalPos.z);
				_shop.TweenLocalPositionY(_shopOriginalPos.y, 0.6f).SetEase(EaseType.BackOut)
					.TweenCanvasGroupAlpha(1f, 0.6f).SetEase(EaseType.SineOut);
			}

			if (_upgradesLocked)
				return;

			_currentUpgrades = message.Upgrades.Select(u => u.Type).ToArray();
			bool[] boughtUpgrades = _world.Get<ShopUpgradesComp>().IsBought;
			for (int i = 0; i < _upgradeButtons.Length; i++)
			{
				bool bought = boughtUpgrades[i];
				_upgradeButtons[i].interactable = !bought;
				_upgradeButtons[i].GetComponentInChildren<TMP_Text>().text = bought ? "Bought" : "Buy";
			}

			//ref var gold = ref _world.Get<GoldComp>();
			for (int i = 0; i < message.Upgrades.Length; i++)
			{
				var upgrade = message.Upgrades[i];
				float upgradePrice = upgrade.Rarity.GetPrice();
				_upgradeNames[i].text = upgrade.Name;
				_upgradePrices[i] = upgradePrice;
				//_upgradeButtonAffordableTweens[i].SetPaused(upgradePrice > gold.Value);

				_upgradeCostTexts[i].text = upgradePrice.ToString("F0") + " gold";
				_upgradeHoverOver[i].Set(upgrade.Mechanics);
				_upgradePanelImages[i].color = upgrade.Rarity.GetColor();
			}
		}

		private void OnScoreUpdated(in ScoreUpdatedEvent message)
		{
			//_scoreText.text = $"Score: {message.Score:F0}";
			UpdateGoldText(message.GoldChange);
		}

		private void OnGameOver(in GameOverEvent message)
		{
			_gameOverPanel.SetActive(true);
			_gameOverPanel.TweenAppearCanvasGroupCancel();
			_fade.gameObject.SetActive(true);
			_fade.gameObject.TweenAppearCanvasGroupCancel();
			ref readonly var score = ref _world.Get<ScoreComp>();
			_statsText.text =
				$"Time: {Time.timeSinceLevelLoad:F1}\nKills: {score.TotalKills:F0}\nBosses killed: {score.TotalBossesKilled:F0}\n" +
				$"Damage dealt: {score.TotalDamageDealt:F0}\nDamage taken: {score.TotalDamageTaken:F0}\n" +
				$"Highest streak: {score.HighestKillStreakEver:F0}\n";
		}

		private void UpdateGoldText(float change)
		{
			const string goldText = "Gold: ";
			ref readonly var gold = ref _world.Get<GoldComp>();

			if (change == 0)
				_goldText.text = goldText + gold.Value.ToString("F0");
			else
				_goldText
					.TweenValueFloat(gold.Value + change, 0.5f, f => _goldText!.text = goldText + f.ToString("F0")).SetEase(EaseType.SineIn)
					.SetFrom(gold.Value)
					.TweenGraphicColor(Color.yellow, 0.1f).SetFrom(Color.white).SetLoopCount(2).SetPingPong()
					.TweenGraphicColor(Color.white, 0.1f).SetDelay(0.1f * 4);
		}

		private void UpdateAffordableUpgradeTweens()
		{
			for (int i = 0; i < _upgradeButtons.Length; i++)
			{
				ref readonly var gold = ref _world.Get<GoldComp>();

				//_upgradeButtonAffordableTweens[i].SetPaused(_upgradePrices[i] > gold.Value || !_upgradeButtons[i].interactable);
			}
		}
	}
}