namespace TMS.Core.Models.PaginationModels
{
    public class PaginationRequestModel
    {
        public string QueryParam { get; set; }
        public bool IsAscending { get; set; } = false;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
        public string OrderColumnName { get; set; }
        public Guid? BranchId { get; set; }

    }
}
