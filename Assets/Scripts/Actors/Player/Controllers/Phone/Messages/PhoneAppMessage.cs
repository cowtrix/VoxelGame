using System.Linq;
using TMPro;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public class PhoneAppMessage : ExtendedMonoBehaviour
	{
		public const int Margin = 32;

		public TextMeshProUGUI MessageText;
		public TextMeshProUGUI TimeText;
		public TextMeshProUGUI NameText;

		public HorizontalLayoutGroup LayoutGroup => GetComponent<HorizontalLayoutGroup>();

		public void SetData(MessagesApp app, MessagesApp.Conversation conversation, MessagesApp.Conversation.Message message)
		{
			if (message.Sender == null)
			{
				LayoutGroup.padding.left = Margin;
				LayoutGroup.padding.right = 0;
			}
			else
			{
				LayoutGroup.padding.right = Margin;
				LayoutGroup.padding.left = 0;
			}
			if (conversation.Contacts.Count <= 1)
			{
				NameText.gameObject.SetActive(false);
			}
			else
			{
				NameText.gameObject.SetActive(true);
				if (message.Sender == null)
				{
					NameText.text = "You";
				}
				else
				{
					NameText.text = conversation.Contacts.Single(c => message.Sender == c.ID).Name;
				}
			}
			MessageText.text = message.Content;
			TimeText.text = message.TimeReceived.GetTimeString();
		}
	}
}