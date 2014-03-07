
namespace Kosmograph.Cli
{
    using KosmoGraph.Persistence.MongoDb;
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Remove, "Relationship")]
    public sealed class RemoveRelationshipCommand : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public Guid Id { get; set; }

        protected override void BeginProcessing()
        {
            this.relationshipRepository = new RelationshipRepository(this.DatabaseName);            
        }

        private RelationshipRepository relationshipRepository;

        protected override void ProcessRecord()
        {
            var relationship = this.relationshipRepository.FindByIdentity(this.Id);
            if(relationship!=null)
                this.relationshipRepository.Remove(relationship);
        }
    }
}
