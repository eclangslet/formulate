﻿namespace formulate.app.Converters
{

    // Namespaces.
    using Helpers;
    using Persistence;
    using Resolvers;
    using System;
    using Types;
    using Umbraco.Core;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;


    /// <summary>
    /// Converts property values to ConfiguredFormInfo.
    /// </summary>
    [PropertyValueType(typeof(ConfiguredFormInfo))]
    public class ConfiguredFormConverter : PropertyValueConverterBase
    {

        #region Properties

        private IConfiguredFormPersistence ConfiguredForms { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfiguredFormConverter()
        {
            ConfiguredForms = ConfiguredFormPersistence.Current.Manager;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Indicates whether or not this is a converter for the specified property type.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <returns>
        /// True, if this can convert the property type; otherwise, false.
        /// </returns>
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return "Formulate.ConfiguredFormPicker".InvariantEquals(propertyType.PropertyEditorAlias);
        }


        /// <summary>
        /// Converts the raw property data to ConfiguredFormInfo.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="source">
        /// The property data.
        /// </param>
        /// <param name="preview">
        /// Is this preview data?
        /// </param>
        /// <returns>
        /// The ConfiguredFormInfo.
        /// </returns>
        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source,
            bool preview)
        {

            // Variables.
            var deserialized = default(dynamic);


            // Source is typically a string, but may already be deserialized (e.g.,
            // when used with Umbraco grid and LeBlender).
            if (source == null || source is string)
            {

                // Source is a string, so deserialize it.
                var strSource = source as string;
                strSource = string.IsNullOrWhiteSpace(strSource)
                    ? "{id:null}"
                    : strSource;
                deserialized = JsonHelper.Deserialize<dynamic>(strSource);

            }
            else
            {

                // Source is not a string, so it was likely already deserialized.
                deserialized = source as dynamic;

            }


            // Variables.
            var id = deserialized.id.Value as string;
            var guid = string.IsNullOrWhiteSpace(id)
                ? null
                : GuidHelper.GetGuid(id) as Guid?;
            var conForm = guid.HasValue
                ? ConfiguredForms.Retrieve(guid.Value)
                : null;


            // Return configuration.
            return new ConfiguredFormInfo()
            {
                Configuration = conForm?.Path[conForm.Path.Length - 1],
                FormId = conForm?.Path[conForm.Path.Length - 2],
                LayoutId = conForm?.LayoutId,
                TemplateId = conForm?.TemplateId
            };

        }

        #endregion

    }

}