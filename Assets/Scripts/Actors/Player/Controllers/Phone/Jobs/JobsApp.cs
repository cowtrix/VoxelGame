using Jerbs;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Utilities;

namespace Phone
{
	public class JobsApp : PhoneApp
    {
        public Jerb CurrentJerb { get; set; }

        public float MaxSearchDistance = 1000;
		public PhoneAppJobsEntry EntryPrefab;
		public Transform EntryContainer;

        private List<PhoneAppJobsEntry> m_entries = new List<PhoneAppJobsEntry>();

        protected override void OnEnable()
		{
			Refresh();
			base.OnEnable();
		}

		public void Refresh()
		{
			int counter = 0;
			foreach(var job in Jerbs.Jerb.Instances)
			{
				var distance = Mathf.FloorToInt((job.GetCurrentPosition() - transform.position).magnitude);
				if(distance > MaxSearchDistance)
				{
					continue;
				}
				PhoneAppJobsEntry entry = null;
				if (m_entries.Count <= counter)
				{
					entry = Instantiate(EntryPrefab.gameObject).GetComponent<PhoneAppJobsEntry>();
					entry.transform.SetParent(EntryContainer);
					entry.transform.Reset();
					m_entries.Add(entry);
				}
				else
				{
					entry = m_entries[counter];
				}
				entry.gameObject.SetActive(true);
				entry.SetData(job);
				counter++;
			}
			for(var i = counter; i < m_entries.Count; ++i)
			{
				m_entries[i].gameObject.SetActive(false);
			}
		}
	}
}