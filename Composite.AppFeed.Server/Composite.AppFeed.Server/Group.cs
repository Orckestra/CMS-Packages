using System;
using System.Runtime.Serialization;

namespace Composite.AppFeed.Server
{
	[DataContract]
	public class Group
	{
		private string _groupViewImage = null;
		private string _backupGroupViewImage = null;

		public Group()
		{
			_backupGroupViewImage = ImageUtilities.RandomGenericImage(ImageType.GroupView);
		}
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Name { get; set; }

		public string ImageId
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.GroupViewImage = ImageUtilities.GetImagePathFromId(value, Settings.GroupViewImageWidth, Settings.GroupViewImageHeight, "fill");

					if (string.IsNullOrEmpty(this.Image))
					{
						this.Image = ImageUtilities.GetImagePathFromId(value, Settings.PrimaryImageWidth, Settings.PrimaryImageHeight, "fit");
					}
				}
			}
		}

		[DataMember]
		public string Image { get; set; }

		[DataMember]
		public string GroupViewImage
		{
			get
			{
				if (_groupViewImage != null)
				{
					return _groupViewImage;
				}

				return _backupGroupViewImage;
			}
			set
			{
				_groupViewImage = value;
			}
		}

		[DataMember]
		public string SubTitle { get; set; }

		[DataMember]
		public string Title { get; set; }

		public int Priority { get; set; }


		[DataMember]
		public string Key
		{
			get
			{
				return string.Format("{0}:{1}", this.Priority.ToString("000"), this.Name);
			}
			internal set { throw new NotImplementedException("Do not set the key"); }
		}
	}
}