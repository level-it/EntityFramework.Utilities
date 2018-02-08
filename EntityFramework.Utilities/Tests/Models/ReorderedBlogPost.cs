using System;

namespace Tests.FakeDomain.Models
{
	public class ReorderedBlogPost
	{
		public int ID { get; set; }
		public string ShortTitle { get; set; }
		public DateTime Created { get; set; }
		public string Title { get; set; } //<--- Reversed order of this and created for Batch Insert testing
		public int Reads { get; set; }
		public AuthorInfo Author { get; set; }

	}
}
