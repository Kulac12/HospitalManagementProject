using Core.Domain_one.Abstract;



namespace Domain_one.HospitalDatabase.Tables
{
    public class OperationClaim:IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
