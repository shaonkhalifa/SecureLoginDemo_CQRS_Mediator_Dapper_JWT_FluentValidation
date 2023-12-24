using Dapper;
using MadiatrProject.Cache;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MadiatrProject.Queries;

public class GetAllStudentsQuery : IRequest<ApiResponse>
{


    private class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, ApiResponse>
    {
        private readonly MDBContext _dbContext;
        private readonly ICacheService _cacheService;




        public GetAllStudentsQueryHandler(MDBContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        public async Task<ApiResponse<List<StudentsDto>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _cacheService.GetAsync("Students", async () =>
                {
                    try
                    {
                        var connection = _dbContext.GetSqlConnection();
                        var query = await connection.QueryAsync<StudentsDto>("SELECT * FROM Students");
                        var data1 = query.ToList();
                        return new ApiResponse<List<StudentsDto>> { IsSuccess = true, Message = "Request is Successful", Data = data1 };

                    }
                    catch (DbException ex)
                    {

                        return new ApiResponse<List<StudentsDto>> { IsSuccess = false, Message = "Request is UnSuccessful" + ex.Message };
                    }
                   
                }, cancellationToken);
                return new ApiResponse<List<StudentsDto>> { IsSuccess=true,Message="Request is Successful", Data = data };

            }
            catch (Exception ex)
            {

                return new ApiResponse <List<StudentsDto>>
                {
                    IsSuccess = false,
                    Code=500,
                    Message= ex.Message,

                };
            }



            //List<StudentsDto>? sdata = await _cacheService.GetAsync<List<StudentsDto>>("Students", cancellationToken);
            //if (sdata != null)
            //{
            //    return sdata;
            //}
            //var connection = _dbContext.GetSqlConnection();

            ////var studentsDto = await _dbConnection.QueryAsync<StudentsDto>("SELECT * FROM Students");
            ////return studentsDto.ToList();
            //var data = "SELECT * FROM Students";
            //var query = await connection.QueryAsync<StudentsDto>(data);

            //sdata = query.ToList();

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
