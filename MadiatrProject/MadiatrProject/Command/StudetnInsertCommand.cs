using MediatR;

namespace MadiatrProject.Command;

public class StudetnInsertCommand:IRequest<int>
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public int RollNo { get; set; }

    //private class StudetnInsertCommandHandler : IRequestHandler<StudetnInsertCommand, int>
    //{
    //    public Task<int> Handle(StudetnInsertCommand request, CancellationToken cancellationToken)
    //    {
            
    //    }
    //}
}


