namespace Kosmograph.Cli
{
    using KosmoGraph.Persistence.MongoDb;
    using KosmoGraph.Services;
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Remove, "Entity")]
    public sealed class RemoveEntityCommand : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public Guid Id { get; set; }

        protected override void BeginProcessing()
        {
            this.entityRepository = new EntityRepository(this.DatabaseName);
            this.entityRelationshipService = new EntityRelationshipService(this.entityRepository, new RelationshipRepository(this.DatabaseName));
        }

        private EntityRepository entityRepository;

        private EntityRelationshipService entityRelationshipService;

        protected override void ProcessRecord()
        {
            var entity = this.entityRepository.FindByIdentity(this.Id);
            if (entity != null)
                this.entityRelationshipService.RemoveEntity(entity);
        }
    }
}
