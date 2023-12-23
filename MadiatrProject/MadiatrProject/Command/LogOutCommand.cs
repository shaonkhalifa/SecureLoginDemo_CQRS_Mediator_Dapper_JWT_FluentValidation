using MadiatrProject.DbContexts;
using MediatR;

namespace MadiatrProject.Command;

public class LogOutCommand:IRequest<string>
{
    public string? Token { get; set; }
    private class LogOutCommandHandler : IRequestHandler<LogOutCommand, string>
    {
        private readonly SDBContext _sdbContext;

        public LogOutCommandHandler(SDBContext sdbContext)
        {
            _sdbContext = sdbContext;
        }

        
        public Task<string> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
