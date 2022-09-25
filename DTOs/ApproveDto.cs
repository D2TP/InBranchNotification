namespace InBranchNotification.DTOs
{
    public class ApproveDto
    {
        public string id { get; set; }
        public bool approved { get; set; }
      
    }

    public class Approve 
    {
        public string id { get; set; }
        public bool approved { get; set; }
        public string approver { get; set; }
        public bool completed { get; set; }
    }
}
