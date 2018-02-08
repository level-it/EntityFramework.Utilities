using System.IO;
using ServiceStack.Text;

namespace Tests
{
	public static class ConnectionStringReader
	{
		private static ConnectionStrings connectionsStrings;

		public static ConnectionStrings ConnectionStrings
		{
			get
			{
				if (connectionsStrings == null)
				{
					using (var stream = File.OpenRead("connectionStrings.json"))
					{
						connectionsStrings = JsonSerializer.DeserializeFromStream<ConnectionStrings>(stream);
					}
				}
				return connectionsStrings;
			}
		}

	}
}
