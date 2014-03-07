namespace KosmoGraph.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IEntityRepository
    {
        Entity Insert(Entity e);
        Entity Update(Entity e);
        void Remove(Entity e);

        Entity FindByIdentity(Guid id);

        IEnumerable<Entity> GetAll();
    }
}
