using Jerbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

namespace Phone
{
	public class PhoneAppJobsEntry : ExtendedMonoBehaviour
	{
		public Image JobIcon;
		public Text JobDescription, JobDistance, JobReward;
		public Button Button => GetComponent<Button>();
		public bool Expanded { get; private set; }

		public Jerb Job { get; private set; }

		public void SetData(Jerb job)
		{
			Job = job;
			Invalidate();
		}

		public void Invalidate()
		{
			if(Job == null)
			{
				gameObject.SetActive(false);
				return;
			}
			JobIcon.sprite = Job.JobInfo.Icon;
			JobDescription.text = $"<b>{Job.JobInfo.Name}</b>\n{Job.JobInfo.Description}";
			JobReward.text = Job.JobInfo.Reward;
		}

		public void ToggleExpanded()
		{
			Expanded = !Expanded;
		}

		private void Update()
		{
			if (Job == null)
			{
				gameObject.SetActive(false);
				return;
			}
			Button.enabled = !Expanded;
			JobDistance.text = $"{Mathf.FloorToInt((Job.GetCurrentPosition() - transform.position).magnitude)}m";
		}
	}
}