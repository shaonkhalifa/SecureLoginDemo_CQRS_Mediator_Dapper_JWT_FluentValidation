using Dapper;
using MadiatrProject.DbContext;
using MadiatrProject.Model;
using MediatR;

namespace MadiatrProject.Queries
{
    public class GetAllStudentsQuery:IRequest<List<StudentsDto>>
    {
        
      
        private class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, List<StudentsDto>>
        {
            private readonly MDBContext _dbContext;
            public GetAllStudentsQueryHandler(MDBContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<List<StudentsDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
            {
                var query = "SELECT * FROM StudentTbl";
                using (var connection = this._dbContext.CreateConnection())
                {
                    var result = await connection.QueryAsync<StudentsDto>(query);
                    return result.ToList();
                }
            }
                
        }
    }
    public class StudentsDto
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public int RollNo { get; set; }
    }
}
