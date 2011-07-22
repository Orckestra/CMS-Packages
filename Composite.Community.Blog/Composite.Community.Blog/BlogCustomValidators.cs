using System;
using Composite.Data.Validation;
using System.CodeDom;
using Composite.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Workflow.Runtime;
using Composite.C1Console.Workflow;

namespace Composite.Community.Blog
{

    #region Tag Validator
    public sealed class TagValidatorAttribute : Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidatorAttribute
    {
        protected override Microsoft.Practices.EnterpriseLibrary.Validation.Validator DoCreateValidator(Type targetType)
        {
            return new TagValidator();
        }
    }

    public sealed class TagValidator : Microsoft.Practices.EnterpriseLibrary.Validation.Validator
    {
        public TagValidator()
            : base("Tag Validation", "Tag")
        {
        }
        protected override string DefaultMessageTemplate
        {
            get { return "Tag Validation"; }
        }
        // This method does the actual validation
        protected override void DoValidate(object objectToValidate, object currentTarget, string key, Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults validationResults)
        {
            string tag = (string)objectToValidate;
            using (DataConnection conn = new DataConnection())
            {
                var tags = conn.Get<Tags>().Where(t => t.Tag == tag).ToList();
                if (tags.Count() > 0)
                {
                    Guid workflowInstanceId = WorkflowEnvironment.WorkflowInstanceId;
                    var id = (Guid)WorkflowFacade.GetFormData(workflowInstanceId).Bindings["Id"];
                    if (tags.Find(t => t.Id == id) == null)
                    {
                        LogValidationResult(validationResults, "The tag with the same name already exists", currentTarget, key);
                    }
                }
            }

        }
    }

    public sealed class TagValidatorPropertyValidatorBuilder : PropertyValidatorBuilder<string>
    {
        public override Attribute GetAttribute()
        {
            return new TagValidatorAttribute();
        }

        public override CodeAttributeDeclaration GetCodeAttributeDeclaration()
        {
            return new CodeAttributeDeclaration(new CodeTypeReference(typeof(TagValidatorAttribute)));
        }
    }

    public sealed class TagValidatorContainer
    {
        /// <summary>
        /// Function to mapped to a C1 function in console
        /// </summary>
        /// <returns></returns>
        public static PropertyValidatorBuilder<string> TagValidator()
        {
            return new TagValidatorPropertyValidatorBuilder();
        }
    }
    #endregion
}
