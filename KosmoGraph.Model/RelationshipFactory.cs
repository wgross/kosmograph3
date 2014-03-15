namespace KosmoGraph.Model
{
    using System;

    public sealed class RelationshipFactory : IModelItemFactory<Relationship>
    {
        #region IModelItemFactory<Relationship> Members

        public Relationship CreateNew(Action<Relationship> initializeWith)
        {
            var tmp = new Relationship { Identity = Guid.NewGuid() };
            (initializeWith ?? delegate { })(tmp);
            return tmp;
        }

        #endregion
    }

    public static class RelationshipFactoryExtensions
    {
        public static Relationship CreateNewPartial(this IModelItemFactory<Relationship> thisFactory, Entity fromEntity)
        {
            return thisFactory.CreateNew(r => 
            {
                r.SetSource(fromEntity);
            });
        }

        public static Relationship CreateNew(this IModelItemFactory<Relationship> thisFactory, Entity fromEntity, Entity toEntity)
        {
            return thisFactory.CreateNew(r=>
            {
                r.SetSource(fromEntity);
                r.SetDestination(toEntity);
            });
        }
    }
}