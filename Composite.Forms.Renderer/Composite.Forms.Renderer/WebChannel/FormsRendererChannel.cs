namespace Composite.C1Console.Forms.WebChannel
{
	/// <summary>
	/// Summary description for FormsRendererChannel
	/// </summary>
	public class FormsRendererChannel : IFormChannelIdentifier
	{
		private static IFormChannelIdentifier _instance = new FormsRendererChannel();

		public static IFormChannelIdentifier Identifier { get { return _instance; } }

		private FormsRendererChannel() { }

		public string ChannelName { get { return "AspNet.FormsRenderer"; } }

		public override string ToString()
		{
			throw new System.Exception("Treated as string here!");
		}
	}
}