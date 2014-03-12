namespace KosmoGraph.Desktop.View
{
    using KosmoGraph.Desktop.ViewModel;
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Services;

    public static class EntityRelationshipViewModelFactoryExtensions
    {
        public static EntityRelationshipViewModel CreateNewFromDatabaseName(this EntityRelationshipViewModelFactory thisEntityRelationshipViewModelFactory, string databaseName)
        {
            return thisEntityRelationshipViewModelFactory.CreateNew(
                new EntityRelationshipService(
                        new EntityRepository(databaseName),
                        new RelationshipRepository(databaseName)),
                new FacetService(
                        new FacetRepository(databaseName)));

            //var e = this.Model.CreateNewEntity("new");
            //e.Top = this.ActualHeight / 2;
            //e.Left = this.ActualWidth / 2;

            //this.Model.Add(e);
        }
    }
}
