
namespace Kosmograph.Cli
{
    using KosmoGraph.Model;
    using KosmoGraph.Persistence.MongoDb;
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get,"Relationship")]
    [OutputType(typeof(Relationship))]
    public sealed class GetRelationshipCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName="byId")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "byEntity")]
        [ValidateNotNullOrEmpty()]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName="byId")]
        public Guid Id { get; set; }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName="byEntity")]
        public Entity Entity { get; set; }

        protected override void BeginProcessing()
        {
            this.relationshipRepository = new RelationshipRepository(this.DatabaseName);
        }

        private RelationshipRepository relationshipRepository;

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "byId")
                this.WriteObject(this.relationshipRepository.FindByIdentity(this.Id));
            else if (this.ParameterSetName == "byEntity")
                foreach (var relationship in this.relationshipRepository.FindByEntityIdentity(this.Entity.Id))
                    this.WriteObject(relationship);

        }
    }
}
