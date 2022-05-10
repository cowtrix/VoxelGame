using Actors;
using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Interaction
{
	public class ProceduralAd : RenderBehaviour
	{
		[System.Serializable]
		public class AdConfiguration
		{
			public string ProductName = "Unknown Product";
			public string Tagline = "Knowing is half the battle!";
			public int Cost = 1;
			public float AdDuration = 5;
			public Sprite BackSprite;
			public Color NameColor, TaglineColor, BackgroundColor, ForegroundColor, PriceColor, PricetagColor;
		}

		public List<AdConfiguration> Ads = new List<AdConfiguration>();

		public RectTransform Container;

		[Header("Price")]
		public Sprite[] SpriteBackgrounds;
		public Image PriceRect;
		public Text PriceLabel;
		public float PriceTransitionSpeed = 1;

		[Header("Product")]
		public Text ProductName;
		public TextScroller Tagline;

		[Header("Background")]
		public Image Background;
		public Image BackSprite;
		public float BackPulseSpeed = 1;

		private AdConfiguration m_currentAd;
		private IEnumerator m_coroutine;

		protected override void Start()
		{
			base.Start();
			m_coroutine = PlayAds();
			m_coroutine.MoveNext();
		}

		IEnumerator PlayAds()
		{
			while (true)
			{
				Random.InitState((int)Time.time + GetHashCode());
				var randomOrder = Ads.OrderBy(a => Random.value).ToList();
				foreach (var ad in randomOrder)
				{
					m_currentAd = ad;
					yield return StartCoroutine(PlayAd(ad));
				}
				yield return null;
			}
		}

		IEnumerator PlayAd(AdConfiguration ad)
		{
			//Random.InitState(ad.GetHashCode());
			Background.color = ad.BackgroundColor;
			BackSprite.color = ad.ForegroundColor;
			BackSprite.sprite = ad.BackSprite;
			BackSprite.enabled = BackSprite.sprite;
			BackSprite.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10, 10));

			PriceRect.sprite = SpriteBackgrounds.Random();
			PriceRect.color = ad.PriceColor;

			var adSize = Container.rect.size;
			PriceRect.transform.localPosition = Random.onUnitSphere.xy().normalized * adSize.x;
			PriceRect.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10, 10));
			PriceRect.transform.localScale = Vector3.one * Random.Range(.8f, 1.1f);

			var targetPricePosition = Container.rect.center + new Vector2(Random.Range(-adSize.x / 3f, adSize.x / 3f), Random.Range(-adSize.y / 4f, adSize.y / 4f));

			PriceLabel.text = $"{ad.Cost}c" + (Random.value > .5f ? "!" : "");
			PriceLabel.color = ad.PricetagColor;

			ProductName.transform.localPosition = Container.rect.center + new Vector2(Random.Range(-adSize.x / 4f, adSize.x / 4f), Random.Range(-adSize.y / 4f, adSize.y / 4f)); ;
			ProductName.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10, 10));
			ProductName.text = LanguageUtility.Translate(ad.ProductName).SafeSubstring(0, 6);
			Tagline.Content = LanguageUtility.Translate(ad.Tagline);

			var timeStarted = Time.time;
			var bump = Random.value;
			while (Time.time < timeStarted + ad.AdDuration + bump)
			{
				BackSprite.transform.localScale = Vector3.one * 2 + Vector3.one * Mathf.Sin(Time.time * BackPulseSpeed);

				if (ad.Cost > 0)
				{
					PriceRect.transform.localPosition = Vector3.MoveTowards(PriceRect.transform.localPosition, targetPricePosition, PriceTransitionSpeed * Time.deltaTime);
					if (PriceRect.transform.localPosition.xy() == targetPricePosition)
					{
						targetPricePosition = Container.rect.center + new Vector2(Random.Range(-adSize.x / 3f, adSize.x / 3f), Random.Range(-adSize.y / 4f, adSize.y / 4f));
					}
				}
				yield return null;
			}
		}

		protected override void UpdateOnScreen()
		{
			m_coroutine.MoveNext();
		}
	}
}