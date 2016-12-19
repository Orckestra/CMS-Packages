using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Forms;
using Composite.Data;
using System.Web;
using System.Web.UI.WebControls;
using Composite.Plugins.Forms.WebChannel.UiControlFactories;
using Composite.Community.Blog;

public partial class BlogTagMultiSelector : UserControlBasedUiControl
{

    private List<string> _selectedKeys = new List<string>();
    private List<Tags> _tags = new List<Tags>();

    [FormsProperty()]
    [BindableProperty()]
    public string SelectedAsString { get; set; }

    public override void InitializeViewState()
    {

        _selectedKeys = SelectedAsString.Split(',').ToList();

        using (var con = new DataConnection())
        {
            var types = con.Get<TagType>().OrderBy(t => t.Name);
            _tags = con.Get<Tags>().ToList();
            tagTypesRepeater.DataSource = types;
            tagTypesRepeater.DataBind();
        }

    }

    public override void BindStateToControlProperties()
    {
        var result = new List<string>();

        for (var i = 0; i < tagTypesRepeater.Items.Count; i++)
        {
            var tagsRepeater = (Repeater) tagTypesRepeater.Items[i].FindControl("tagsRepeater");

            for (var j = 0; j < tagsRepeater.Items.Count; j++)
            {
                var control = tagsRepeater.Items[j].FindControl("CheckBox");

                var checkBox = (Composite.Core.WebClient.UiControlLib.CheckBox) control;

                if (checkBox.Checked) result.Add(HttpUtility.HtmlDecode(checkBox.Text));
            }
        }

        SelectedAsString = string.Join(",", result.ToArray());
    }

    protected bool IsChecked(string key)
    {
        return _selectedKeys.Contains(key);
    }


    protected List<Tags> GetTags(object item)
    {
        var tagType = (TagType) item;
        return _tags.Where(t => t.Type == tagType.Id).OrderBy(t => t.Position).ToList();
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}