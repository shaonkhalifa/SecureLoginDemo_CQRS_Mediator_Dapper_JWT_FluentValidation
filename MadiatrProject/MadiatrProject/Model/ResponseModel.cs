namespace MadiatrProject.Model
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Description { get; set; }
        public string StatusType { get; set; }
        //public ResponseModel(int statusCode,string message,string? description,string statusType) 
        //{
        //    StatusCode = statusCode;
        //    Message = message;
        //    Description = description;
        //    StatusType = statusType;
        //}
       
    }
}
