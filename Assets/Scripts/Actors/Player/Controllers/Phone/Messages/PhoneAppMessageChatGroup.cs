using Common;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public class PhoneAppMessageChatGroup : ExtendedMonoBehaviour
	{
		public TextMeshProUGUI Text;
		public Image IconImage;

		private MessagesApp.Conversation m_conversation;

		public Image Background;
		public Color SeenColor, UnseenColor;
		private Button Button => GetComponent<Button>();

		private void Update()
		{
			if (m_conversation.LastOpened < GameManager.Instance.CurrentTime)
			{
				Background.color = UnseenColor;
			}
			else
			{
				Background.color = SeenColor;
			}
		}

		public void SetData(MessagesApp app, MessagesApp.Conversation conversation)
		{
			Text.text = conversation.GetPreviewString();
			IconImage.sprite = conversation.Icon;
			m_conversation = conversation;

			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(() => app.OpenConversation(conversation));
		}
	}
}