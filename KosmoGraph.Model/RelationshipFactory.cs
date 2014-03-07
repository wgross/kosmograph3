namespace KosmoGraph.Model
{
    using System;

    public sealed class RelationshipFactory
    {
        private static Relationship CreateNew()
        {
            return new Relationship() { Identity = Guid.NewGuid() };
        }

        public static Relationship CreateNew(Action<Relationship> setup)
        {
            var tmp = CreateNew();
            (setup ?? delegate { })(tmp);
            return tmp;
        }

        public static Relationship CreateNewPartial(Entity fromEntity)
        {
            return CreateNew(r => 
            {
                r.SetSource(fromEntity);
            });
        }

        public static Relationship CreateNew(Entity fromEntity, Entity toEntity)
        {
            return CreateNew(r=>
            {
                r.SetSource(fromEntity);
                r.SetDestination(toEntity);
            });
        } 
    }
}