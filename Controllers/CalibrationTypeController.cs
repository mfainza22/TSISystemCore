﻿using System;
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
    public class CalibrationTypesController : ControllerBase
    {
        private readonly ICalibrationTypeRepository repository;
        private readonly ILogger<CalibrationTypesController> logger;
        public CalibrationTypesController(ILogger<CalibrationTypesController> logger, ICalibrationTypeRepository repository)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var model = repository.Get();
                return Ok(model);
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
        public IActionResult Post([FromBody] CalibrationType model)
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
        public IActionResult Put([FromBody] CalibrationType model)
        {
            try
            {
                if (!ModelState.IsValid) return InvalidModelStateResult();
                if (!validateEntity(model)) return InvalidModelStateResult();
                if (repository.Get().Count(a => a.CalibrationTypeId.Equals(model.CalibrationTypeId)) == 0) return NotFound(Constants.Messages.NotFoundEntity);
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
                logger.LogDebug(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, Constants.Messages.DeleteError);
            }
        }

      

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ValidateName([FromBody] CalibrationType model)
        {
            if (model == null) return NotFound();
            if (General.IsDevelopment) logger.LogDebug(ModelState.ToJson());
            var existing = repository.Get().FirstOrDefault(a => a.CalibrationTypeDesc == model.CalibrationTypeDesc);
            if (existing == null) return Accepted(true);
            if (existing.CalibrationTypeId != model.CalibrationTypeId)
            {
                return UnprocessableEntity(Constants.Messages.EntityExists("Description"));
            }
            else
            {
                return Accepted(true);
            }
        }

        private bool validateEntity(CalibrationType model)
        {
           var validName = repository.ValidateName(model);
            if (!validName) ModelState.AddModelError(nameof(CalibrationType.CalibrationTypeDesc), Constants.Messages.EntityExists("Name"));
            return (ModelState.ErrorCount == 0);
        }
        private IActionResult InvalidModelStateResult()
        {
            var jsonModelState = ModelState.ToJson();
            if (General.IsDevelopment) logger.LogDebug(jsonModelState);
            return UnprocessableEntity(jsonModelState);
        }

    }
}