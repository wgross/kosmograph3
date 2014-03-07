namespace KosmoGraph.Model
{
    using System;

    public class PropertyDefinition : IHasName
    {
        public PropertyDefinition()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        #region Spawn a value instance for an AssignedTag

        public PropertyValue CreateNewPropertyValue(AssignedFacet assignedTag, Action<PropertyValue> initialize)
        {
            return PropertyValueFactory.CreateNew(assignedTag, this, initialize);
        }

        #endregion
    }
}