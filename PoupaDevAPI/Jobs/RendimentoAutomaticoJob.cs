using Microsoft.EntityFrameworkCore;
using PoupaDev.API.Persistence;

namespace PoupaDevAPI.Jobs
{
    public class RendimentoAutomaticoJob : IHostedService
    {
        private Timer _timer;
        public IServiceProvider ServiceProvider { get; set; }

        public RendimentoAutomaticoJob(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RenderSaldo, null, 0, 10000);

            return Task.CompletedTask;
        }

        public void RenderSaldo(object? state)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PoupaDevDbContext>();
                var objetivos = context.Objetivos.Include(o => o.Operacoes);

                foreach(var objetivo in objetivos)
                {
                    objetivo.Render();
                }

                context.SaveChanges();
            }
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
