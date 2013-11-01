using System;
using Composite.Functions;

public partial class $safeitemrootname$ : Composite.AspNet.UserControlFunction
{
    public override string FunctionDescription
    {
        get { return "A demo function that outputs a hello message."; }
    }

    [FunctionParameter(Label="Whom to greet", Help="Specify a name or thing to greet.", DefaultValue = "World")]
    public string Name { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}