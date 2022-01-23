using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Voxul.Utilities;

namespace Phone
{
	public class MessagesApp : PhoneApp
	{
		[Serializable]
		public class Conversation
		{
			[Serializable]
			public class Message
			{
				public GameDateTime TimeReceived;
				public string Content;
				public string Sender;
				public string ConversationID;
			}

			[Serializable]
			public class Contact
			{
				public string Name;
				public string ID;

				public override bool Equals(object obj)
				{
					return obj is Contact contact &&
						   ID == contact.ID;
				}

				public override int GetHashCode()
				{
					return 1213502048 + EqualityComparer<string>.Default.GetHashCode(ID);
				}

				public static bool operator ==(Contact left, Contact right)
				{
					return EqualityComparer<Contact>.Default.Equals(left, right);
				}

				public static bool operator !=(Contact left, Contact right)
				{
					return !(left == right);
				}

				public override string ToString() => string.IsNullOrEmpty(Name) ? ID : Name;
			}

			public Sprite Icon;
			public string Name;
			public string ID;
			public GameDateTime LastOpened;
			public List<Message> Messages = new List<Message>();
			public List<Contact> Contacts = new List<Contact>();

			public string GetTitle()
			{
				if (!string.IsNullOrEmpty(Name))
				{
					return Name;
				}
				if (Contacts.Count == 1)
				{
					var c = Contacts[0];
					return c.Name ?? c.ID;
				}
				return string.Join(", ", Contacts.Select(c => c.Name));
			}

			public string GetPreviewString() => $"<b>{GetTitle()}</b>:   {Messages?.LastOrDefault()?.Content.SafeSubstring(0, 50)}";
		}

		public IEnumerable<Conversation.Contact> AllContacts =>
			Conversations.SelectMany(c => c.Contacts).Distinct();

		public Conversation.Contact GetContactByID(string id) =>
			AllContacts.FirstOrDefault(c => c.ID == id);

		public PhoneAppMessageChatGroup ChatGroupPrefab;
		public Transform ChatGroupContainer;
		private List<PhoneAppMessageChatGroup> m_conversations = new List<PhoneAppMessageChatGroup>();

		public Sprite DefaultIcon;

		public GameObject ConversationPanel;
		public Transform MessagesContainer;
		public PhoneAppMessage MessagePrefab;
		public Image CurrentConversationIcon;
		public Text CurrentConversationName;
		private List<PhoneAppMessage> m_messages = new List<PhoneAppMessage>();
		public ScrollRect ScrollRect;
		public List<Conversation> Conversations;
		private Conversation m_currentConversation;

		private void Start()
		{
			Invalidate();
		}

		public void CloseConversation()
		{
			m_currentConversation = null;
			Invalidate();
		}

		public override void OnOpen(PhoneController phoneController)
		{
			Invalidate();
			base.OnOpen(phoneController);
		}

		public void Invalidate()
		{
			for (int i = 0; i < Conversations.Count; i++)
			{
				var conversation = Conversations[i];
				PhoneAppMessageChatGroup group = null;
				if (m_conversations.Count <= i)
				{
					group = Instantiate(ChatGroupPrefab.gameObject)
						.GetComponent<PhoneAppMessageChatGroup>();
					group.transform.SetParent(ChatGroupContainer);
					group.transform.Reset();
					m_conversations.Add(group);
				}
				else
				{
					group = m_conversations[i];
				}
				group.gameObject.SetActive(true);
				group.SetData(this, conversation);
			}
			for (var i = Conversations.Count; i < m_conversations.Count; ++i)
			{
				m_conversations[i].gameObject.SetActive(false);
			}

			if (m_currentConversation == null)
			{
				ConversationPanel.gameObject.SetActive(false);
				return;
			}
			ConversationPanel.SetActive(true);
			CurrentConversationIcon.sprite = m_currentConversation.Icon;
			CurrentConversationName.text = m_currentConversation.GetTitle();
			for (int i = 0; i < m_currentConversation.Messages.Count; i++)
			{
				var message = m_currentConversation.Messages[i];
				PhoneAppMessage messagePrefab = null;
				if (m_messages.Count <= i)
				{
					messagePrefab = Instantiate(MessagePrefab.gameObject).GetComponent<PhoneAppMessage>();
					messagePrefab.transform.SetParent(MessagesContainer);
					messagePrefab.transform.Reset();
					m_messages.Add(messagePrefab);
				}
				else
				{
					messagePrefab = m_messages[i];
				}
				//messagePrefab.transform.SetSiblingIndex(i + 1);
				messagePrefab.SetData(this, m_currentConversation, message);
			}
			for (var i = m_currentConversation.Messages.Count; i < m_messages.Count; ++i)
			{
				m_messages[i].gameObject.SetActive(false);
			}
		}

		public void OpenConversation(Conversation c)
		{
			m_currentConversation = c;
			Invalidate();
		}

		public void ReceiveMessage(Conversation.Message message)
		{
			Conversation convo = Conversations.FirstOrDefault(c => c.ID == (message.ConversationID ?? message.Sender));
			if (convo == null)
			{
				convo = new Conversation
				{
					Contacts = new List<Conversation.Contact>
					{
						new Conversation.Contact
						{
							ID = message.Sender,
							Name = GetContactByID(message.Sender)?.Name
						}
					},
					Messages = new List<Conversation.Message>
					{
						message,
					},
					Icon = DefaultIcon,
					ID = message.ConversationID ?? message.Sender,
				};
				Conversations.Add(convo);
			}
			if (!convo.Contacts.Any(c => c.ID == message.Sender))
			{
				convo.Contacts.Add(GetContactByID(message.Sender) ?? new Conversation.Contact { ID = message.Sender });
			}
			convo.Messages.Add(message);
			var sender = convo.Contacts.First(c => c.ID == message.Sender);
			SendNotification($"<b>{sender}</b>: {message.Content.SafeSubstring(0, 50)}");
			Invalidate();
			Canvas.ForceUpdateCanvases();
			ScrollRect.normalizedPosition = Vector2.zero;
		}

		public override string GetSaveData() => JsonUtility.ToJson(m_conversations);

		public override void LoadSaveData(string data)
		{
			m_conversations = JsonUtility.FromJson<List<PhoneAppMessageChatGroup>>(data.ToString());
		}
	}
}