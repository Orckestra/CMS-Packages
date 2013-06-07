using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.Runtime;
using Composite.C1Console.Workflow;
using Composite.Data;
using Composite.Data.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Composite.Community.Blog
{

    #region Tag Validator

    public sealed class TagValidatorAttribute : ValueValidatorAttribute
    {
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new TagValidator();
        }
    }

    public sealed class TagValidator : Validator
    {
        public TagValidator()
            : base("Tag Validation", "Tag")
        {
        }

        protected override string DefaultMessageTemplate
        {
            get { return "Tag Validation"; }
        }

        protected override void DoValidate(object objectToValidate, object currentTarget, string key,
                                           ValidationResults validationResults)
        {
            var tag = (string) objectToValidate;
            if (tag.Contains(","))
            {
                LogValidationResult(validationResults, "A comma is not supported", currentTarget, key);
                return;
            }
            using (var conn = new DataConnection())
            {
                List<Tags> tags = conn.Get<Tags>().Where(t => t.Tag == tag).ToList();
                if (tags.Any())
                {
                    Guid workflowInstanceId = WorkflowEnvironment.WorkflowInstanceId;
                    var id = (Guid) WorkflowFacade.GetFormData(workflowInstanceId).Bindings["Id"];
                    if (tags.Find(t => t.Id == id) == null)
                    {
                        LogValidationResult(validationResults, "The tag with the same name already exists",
                                            currentTarget, key);
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
            return new CodeAttributeDeclaration(new CodeTypeReference(typeof (TagValidatorAttribute)));
        }
    }

    public sealed class TagValidatorContainer
    {
        /// <summary>
        ///     Function to mapped to a C1 function in console
        /// </summary>
        /// <returns></returns>
        public static PropertyValidatorBuilder<string> TagValidator()
        {
            return new TagValidatorPropertyValidatorBuilder();
        }
    }

    #endregion
}