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
           
            private readonly IDbConnection _dbConnection;


            public GetAllStudentsQueryHandler(MDBContext dbContext, IDbConnection dbConnection)
            {
                _dbContext = dbContext;
                _dbConnection = dbConnection;
                
            }
            public async Task<List<StudentsDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
            {
               

                var studentsDto = await _dbConnection.QueryAsync<StudentsDto>("SELECT * FROM Students");
                return studentsDto.ToList();

                //try
                //{
                //    // Dapper query
                //    var studentsDto = await _dbConnection.QueryAsync<StudentsDto>("SELECT * FROM Students");
                //    return studentsDto.ToList();
                //}
                //catch (Exception ex)
                //{
                //    // Handle exceptions (log, throw, etc.)
                //    Console.WriteLine($"Error executing Dapper query: {ex.Message}");
                //    throw;
                //}
                //finally
                //{
                //    // Ensure the Dapper connection is explicitly closed
                //   _dbConnection.Close();
                //}


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
