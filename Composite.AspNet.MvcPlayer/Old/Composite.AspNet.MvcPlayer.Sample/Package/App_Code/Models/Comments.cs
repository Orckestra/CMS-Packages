using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Comments
/// </summary>
public class Comments
{
	public static Comments Instanse = new Comments();

	private List<Comment> list = new List<Comment>();

	private Comments()
	{
	}

	public void AddComment(string name, string text, Guid pageId)
	{
		list.Add(new Comment() { Name = name, Text = text, Date = DateTime.Now, PageId = pageId});
	}

	public List<Comment> GetComments(Guid pageId)
	{
		return list.Where(c => c.PageId == pageId).ToList();
	}
}


public class Comment
{
	public string Name { get; set; }
	public string Text { get; set; }
	public DateTime Date { get; set; }
	public Guid PageId { get; set; }
}
	