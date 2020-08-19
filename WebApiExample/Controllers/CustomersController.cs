﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiExample.Contracts;
using WebApiExample.Models;
using WebApiExample.Repositories;

namespace WebApiExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        //private ApiExampleContext _ctx;
        //public CustomersController(ApiExampleContext context)
        //{
        //    _ctx = context;
        //}
        private readonly ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }


        [HttpGet]
        [ResponseCache(Duration = 60)]
        public IActionResult GetCostumer()
        {
            var result = new ObjectResult(_customerRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            Request.HttpContext.Response.Headers.Add("X-Count", _customerRepository.CountCustomer().ToString());
            Request.HttpContext.Response.Headers.Add("X-Name", "Hossein Hayati");

            return result;
            //return new ObjectResult(_ctx.Customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCostumer([FromRoute] int id)
        {
            if (await IsCustomerExists(id))
            {
                var customer = await _customerRepository.Find(id);
                return Ok(customer);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] Customers customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _customerRepository.Add(customer);
            return CreatedAtAction("GetCostumer", new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer([FromRoute] int id, [FromBody] Customers customers)
        {
            await _customerRepository.Update(customers);
            return Ok(customers);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            await _customerRepository.Remove(id);
            return Ok();
        }

        private async Task<bool> IsCustomerExists(int id)
        {
            return await _customerRepository.IsExists(id);
        }
    }
}