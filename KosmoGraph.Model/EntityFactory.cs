namespace KosmoGraph.Model
{
    using System;

    internal sealed class EntityFactory : IModelItemFactory<Entity>
    {
        #region IModelItemFactory<Entity> Members

        public Entity CreateNew(Action<Entity> setup)
        {
            var tmp = new Entity();
            (setup ?? delegate { })(tmp);
            return tmp;
        }

        #endregion 
    }
}