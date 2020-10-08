using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SysDomain.IRepositories;
using SysDomain.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeghingSystemCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoistureSettingsController : ControllerBase
    {
        private readonly IMoistureSettingsRepository repository;

        public MoistureSettingsController(IMoistureSettingsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IQueryable<MoistureSettings> Get()
        {
            return repository.Get();
        }

        [HttpGet("{id}")]
        public MoistureSettings Get(long id)
        {
            return repository.GetById(id);
        }

        [HttpPost]
        public void Post([FromBody] MoistureSettings model)
        {
            repository.Create(model);
        }

        [HttpPut("{id}")]
        public void Put( [FromBody] MoistureSettings model)
        {
            repository.Update(model);
        }

        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            var model = repository.GetById(id);
            repository.Delete(model);
        }

    }
}
