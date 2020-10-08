using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SysDomain.IRepositories;
using SysDomain.Models;
using SysUtility;
using SysUtility.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeghingSystemCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourcesController : ControllerBase
    {
        private readonly ISourceRepository repository;
        private readonly ILogger<SourcesController> logger;
        public SourcesController(ILogger<SourcesController> logger, ISourceRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(repository.Get().ToList()
                    .Select(a => { 
                        a.SourceCategoryDesc = (a.SourceCategory ?? new SourceCategory()).Description;  return a; 
                    }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.FetchError);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var model = repository.GetById(id);
                if (model == null) return NotFound(Constants.Messages.NotFoundEntity);
                return Ok(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.FetchError);
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] Source model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                return Accepted(repository.Create(model));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.CreateError);
            }


        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Put([FromBody] Source model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                if (repository.Get().Count(a => a.SourceId.Equals(model.SourceId)) == 0) return NotFound(Constants.Messages.NotFoundEntity);

                return Accepted(repository.Update(model));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.UpdateError);
            }

        }


        [HttpDelete]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult Delete(long id)
        {
            try
            {
                var model = repository.GetById(id);

                if (model == null)
                {
                    return BadRequest(Constants.Messages.NotFoundEntity);
                }

                repository.Delete(model);

                return Accepted(Constants.Messages.DeleteSucess(1));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.DeleteError);
            }
        }


        [HttpDelete("{name}/{ids}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status400BadRequest)]
        public IActionResult BulkDelete(string ids)
        {
            try
            {
                var arrayIds = ids.Split(",");
                if (arrayIds.Length == 0) return BadRequest(Constants.Messages.NoEntityOnDelete);

                repository.BulkDelete(arrayIds);

                return Ok(Constants.Messages.DeleteSucess(arrayIds.Count()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.DeleteError);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateCode([FromBody] Source model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var result = repository.ValidateCode(model);
            if (result) return Accepted(true);
            else return UnprocessableEntity(Constants.Messages.EntityExists("Name"));
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateName([FromBody] Source model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var result = repository.ValidateName(model);
            if (result) return Accepted(true);
            else return UnprocessableEntity(Constants.Messages.EntityExists("Name"));
        }

        private bool validateEntity(Source model)
        {
            var validCode = repository.ValidateCode(model);
            if (!validCode) ModelState.AddModelError(nameof(Source.SourceCode), Constants.Messages.EntityExists("Code"));
            var validName = repository.ValidateName(model);
            if (!validName) ModelState.AddModelError(nameof(Source.SourceDesc), Constants.Messages.EntityExists("Name"));
            return ModelState.ErrorCount == 0;
        }

        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }
    }
}
