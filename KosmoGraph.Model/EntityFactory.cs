namespace KosmoGraph.Model
{
    using System;

    public sealed class EntityFactory
    {
        public static Entity CreateNew()
        {
            return new Entity();
        }

        public static Entity CreateNew(Action<Entity> setup)
        {
            var tmp = new Entity();
            (setup ?? delegate { })(tmp);
            return tmp;
        }
    }
}