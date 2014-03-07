namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Facet : IHasName
    {
        #region Construction and initialization of this instance 

        public Facet()
        {
            this.Id = Guid.NewGuid();
        }

        #endregion

        #region IHasIdentity<Guid> Members

        public Guid Id
        {
            get;
            set;
        }

        #endregion IHasIdentity<Guid> Members

        #region IHasName Members

        public string Name
        {
            get;
            set;
        }

        #endregion IHasName Members

        #region A Facet provides a collection of property definitions

        public PropertyDefinition CreateNewPropertyDefinition(Action<PropertyDefinition> initialize)
        {
            var tmp = PropertyDefinitionFactory.CreateNew(this);
            (initialize ?? delegate { })(tmp);
            return tmp;
        }
        
        public IEnumerable<PropertyDefinition> Properties 
        {
            get
            {   
                return this.properties;
            }
            private set
            {
                this.properties.Clear();
                this.properties.AddRange(value ?? Enumerable.Empty<PropertyDefinition>());
            }
        }

        private readonly List<PropertyDefinition> properties = new List<PropertyDefinition>();

        public PropertyDefinition Add(PropertyDefinition propertyDefinitionToAdd)
        {
            if (propertyDefinitionToAdd == null)
                throw new ArgumentNullException("propertyDefinitionToAdd");

            var duplicate = this.Properties.FirstOrDefault(pd => pd.Equals(propertyDefinitionToAdd) || pd.Id == propertyDefinitionToAdd.Id);
            if (duplicate != null)
                return duplicate;

            if (this.Properties.Any(pd => pd.Name == propertyDefinitionToAdd.Name))
                throw new InvalidOperationException(string.Format("Property definition with same name was already added '{0}'", propertyDefinitionToAdd.Name));

            this.properties.Add(propertyDefinitionToAdd);
            return propertyDefinitionToAdd;
        }

        public bool Remove(PropertyDefinition propertyDefinitionToRemove)
        {
            if (propertyDefinitionToRemove == null)
                throw new ArgumentNullException("propertyDefinitionToRemove");

            return this.properties.Remove(propertyDefinitionToRemove);
        }
        
        #endregion 
    }
}