using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ABATON.Models;
using ABATON.Services;

namespace ABATON
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var PatientOptions = new DbContextOptionsBuilder<PatientContext>()
                .UseInMemoryDatabase("Patients").Options;
            var DrugOptions = new DbContextOptionsBuilder<DrugContext>()
                .UseInMemoryDatabase("Drugs").Options;
            var DosagesOptions = new DbContextOptionsBuilder<DosageContext>()
                .UseInMemoryDatabase("Dosage").Options;

            var PatientCont = new PatientContext(PatientOptions);
            var DrugCont = new DrugContext(DrugOptions);
            var DosageCont = new DosageContext(DosagesOptions);

            IRelationshipService Relationship = new RelationshipService(PatientCont, 
                DrugCont,
                DosageCont);

            services.AddSingleton(Relationship);

            services.AddDbContext<PatientContext>(opt =>
                opt.UseInMemoryDatabase("Patients"));
            services.AddDbContext<DrugContext>(opt =>
                opt.UseInMemoryDatabase("Drugs"));
            services.AddDbContext<DosageContext>(opt =>
                opt.UseInMemoryDatabase("Dosage"));
            services.AddControllers();
                        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
