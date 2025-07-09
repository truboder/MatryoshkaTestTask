using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CookingPrototype.Controllers;

namespace CookingPrototype.UI 
{
	public sealed class StartWindow : MonoBehaviour
	{
		public TMP_Text GoalText    = null;
		public Button   PlayButton  = null;
		public Button   CloseButton = null;

		bool _isInit = false;

		void Init() 
		{
			if (_isInit)
			{
				return;
			}

			GameplayController gameplayController = GameplayController.Instance;

			PlayButton.onClick.AddListener(() => {
				Time.timeScale = 1f;
				gameplayController.Restart();
				Hide();
			});
			CloseButton.onClick.AddListener(gameplayController.CloseGame);

			_isInit = true;
		}

		public void Show()
		{
			if (!_isInit) 
			{
				Init();
			}

			GameplayController gameplayController = GameplayController.Instance;
			GoalText.text = $"Выполнить заказов: {gameplayController.OrdersTarget}";

			gameObject.SetActive(true);
		}

		public void Hide() 
		{
			gameObject.SetActive(false);
		}
	}
}