﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Licenta.API.Data;
using Licenta.API.Models;
using Licenta.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Licenta.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompanyPresentationsController : ControllerBase
    {
        private readonly ICompanyPresentationsService _companyPresentationsService;
        private readonly IGenericsRepository _genericsRepo;

        public CompanyPresentationsController(ICompanyPresentationsService companyPresentationsService, IGenericsRepository genericsRepo)
        {
            _companyPresentationsService = companyPresentationsService;
            _genericsRepo = genericsRepo;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("AddPresentation")]
        public async Task<IActionResult> AddPresentation(CompanyPresentation presentation)
        {
            if (await _companyPresentationsService.CompanyPresentationExists(presentation))
            {
                return BadRequest("There is a presentation already at this hour or in this class!");
            }

            _companyPresentationsService.AddCompanyPresentation(presentation);

            if (await _genericsRepo.SaveAll())
            {
                return Ok("Presentation was added!");
            }

            return BadRequest("Something went wrong!");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("getPresentations/{id}")]
        public async Task<IActionResult> GetCompanyPresentationsForAdmin(int id)
        {
            var presentations = await _companyPresentationsService.GetCompanyPresentationsForUser();

            var presentationsForAdmin = _companyPresentationsService.MapPresentationsForAdmin(presentations);

            return Ok(presentationsForAdmin);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePresentation(CompanyPresentation presentation)
        {
            var updatedPresentation = await _companyPresentationsService.UpdatePresentation(presentation);

            if (await _genericsRepo.SaveAll())
            {
                return Ok(updatedPresentation);
            }
            return BadRequest("Update Failed or the same presentation was sent");
        }
    }
}