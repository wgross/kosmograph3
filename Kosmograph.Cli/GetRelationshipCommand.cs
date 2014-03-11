namespace Kosmograph.Cli
{
    using KosmoGraph.Model;
    using KosmoGraph.Persistence.MongoDb;
    using System;
    using System.Linq;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get,"Relationship")]
    [OutputType(typeof(Relationship))]
    public sealed class GetRelationshipCommand : PSCmdlet
    {
        #region Parameterset 'byId'

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "byId", ValueFromPipelineByPropertyName = true)]
        public Guid Id { get; set; }

        #endregion 
        
        #region Parameterset 'byPage'

        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "byPage", ValueFromPipeline = false, ValueFromPipelineByPropertyName = false)]
        public int Take { get; set; }

        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "byPage", ValueFromPipeline = false, ValueFromPipelineByPropertyName = false)]
        public int Skip { get; set; }

        #endregion 

        #region Parameterset 'byEntity'

        public Entity Entity { get;  set; }

        #endregion 

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [PSDefaultValue(Value = "kosmograph")]
        public string DatabaseName { get; set; }
    
        protected override void BeginProcessing()
        {
            this.relationshipRepository = new RelationshipRepository(this.DatabaseName);
        }

        private RelationshipRepository relationshipRepository;

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "byId")
            {
                this.WriteObject(this.relationshipRepository.FindByIdentity(this.Id));
            }
            else if (this.ParameterSetName == "byEntity")
            {
                foreach (var relationship in this.relationshipRepository.FindByEntityIdentity(this.Entity.Id))
                    this.WriteObject(relationship);
            }
            else if (this.ParameterSetName == "byPage")
            {
                foreach (var relationship in this.relationshipRepository.GetAll().Skip(this.Skip).Take(this.Take))
                {
                    this.WriteObject(relationship);
                }
            }
        }
    }
}
