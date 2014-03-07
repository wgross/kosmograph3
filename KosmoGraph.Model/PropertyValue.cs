namespace KosmoGraph.Model
{
    using System;

    /// <summary>
    /// A property value contains of a property defineition reference id and a property value
    /// </summary>
    public class PropertyValue
    {
        public Guid DefinitionId { get; set; }

        public string Value { get; set; }
    }
}