using Core.Domain_one.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain_one.HospitalDatabase.Tables
{
    public class UserOperationClaim:IEntity
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(OperationClaim))]
        public Guid OperationClaimId { get; set; }
    }
}
