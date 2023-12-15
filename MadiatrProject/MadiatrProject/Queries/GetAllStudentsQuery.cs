using Dapper;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;

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
                var connection = _dbContext.GetSqlConnection();

                //var studentsDto = await _dbConnection.QueryAsync<StudentsDto>("SELECT * FROM Students");
                //return studentsDto.ToList();
                var data = "SELECT * FROM Students";
                var query = await connection.QueryAsync<StudentsDto>(data);
                return query.ToList();
               



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
