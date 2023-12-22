using Dapper;
using MadiatrProject.Cache;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;

namespace MadiatrProject.Queries;

public class GetAllStudentsQuery : IRequest<List<StudentsDto>>
{


    private class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, List<StudentsDto>>
    {
        private readonly MDBContext _dbContext;
        private readonly ICacheService _cacheService;




        public GetAllStudentsQueryHandler(MDBContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        public async Task<List<StudentsDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {


            return await _cacheService.GetAsync("Students", async () =>
                        {
                            var connection = _dbContext.GetSqlConnection();
                            var query = await connection.QueryAsync<StudentsDto>("SELECT * FROM Students");
                            return query.ToList();
                        }, cancellationToken);


            //List<StudentsDto>? sdata =await _cacheService.GetAsync<List<StudentsDto>>("Students", cancellationToken);
            //if (sdata != null)
            //{
            //    return sdata;
            //}
            //var connection = _dbContext.GetSqlConnection();

            ////var studentsDto = await _dbConnection.QueryAsync<StudentsDto>("SELECT * FROM Students");
            ////return studentsDto.ToList();
            //var data = "SELECT * FROM Students";
            //var query = await connection.QueryAsync<StudentsDto>(data);

            //sdata=query.ToList();

            //await _cacheService.SetAsync("Students", sdata, cancellationToken);
            //return query.ToList();




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
