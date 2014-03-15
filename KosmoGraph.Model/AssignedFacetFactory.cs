namespace KosmoGraph.Model
{
    using System;

    public sealed class AssignedFacetFactory 
    {
        public static AssignedFacet CreateNew(Entity entity, Facet assignedTag)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (assignedTag == null)
                throw new ArgumentNullException("assignedTag");

            return new AssignedFacet
            {
                FacetId = assignedTag.Id
            };
        }

        public static AssignedFacet CreateNew(Entity entity, Facet assignedTag, Action<AssignedFacet> initialize)
        {
            var tmp = CreateNew(entity, assignedTag);
            (initialize ?? delegate { })(tmp);
            return tmp;
        }

        public static AssignedFacet CreateNew(Relationship relationship, Facet assignedTag)
        {
            if (relationship == null)
                throw new ArgumentNullException("relationship");

            if (assignedTag == null)
                throw new ArgumentNullException("assignedTag");

            return new AssignedFacet
            {
                FacetId = assignedTag.Id
            };
        }

        public static AssignedFacet CreateNew(Relationship relationship, Facet assignedTag, Action<AssignedFacet> initialize)
        {
            var tmp = CreateNew(relationship, assignedTag);
            (initialize ?? delegate { })(tmp);
            return tmp;
        }
    }
}