﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Log.Context;
using WebAPI_Log.Entities;
using WebAPI_Log.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI_Log.Controllers
{
    //[Authorize(Roles = "ADMIN, MANAGER, COACH")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // Obtener lista de clientes
        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<List<ClienteDTO>>> Get()
        {
            var listaDTO = new List<ClienteDTO>();
            var listaDB = await _context.Clientes.Include(p => p.PerfilReferencia).ToListAsync();

            foreach (var item in listaDB)
            {
                listaDTO.Add(new ClienteDTO
                {
                    IdCliente = item.IdCliente,
                    NombreCompleto = item.NombreCompleto,
                    IdPerfil = item.IdPerfil,
                    FechaIngreso = item.FechaIngreso,
                    Email = item.Email,
                    ContactNro = item.ContactNro,
                    EstaPagada = item.EstaPagada,
                    FechaPago = item.FechaPago,
                    FechaVencimiento = item.FechaVencimiento
                });
            }

            return Ok(listaDTO);
        }

        // Obtener un cliente específico
        [HttpGet]
        [Route("buscar/{id}")]
        public async Task<ActionResult<ClienteDTO>> Get(int id)
        {
            var clienteDTO = new ClienteDTO();
            var clienteDB = await _context.Clientes.Include(p => p.PerfilReferencia)
                .Where(e => e.IdCliente == id).FirstAsync();

            clienteDTO.IdCliente = id;
            clienteDTO.NombreCompleto = clienteDB.NombreCompleto;
            clienteDTO.IdPerfil = clienteDB.IdPerfil;
            clienteDTO.FechaIngreso = clienteDB.FechaIngreso; 
            clienteDTO.Email = clienteDB.Email;
            clienteDTO.ContactNro = clienteDB.ContactNro;
            clienteDTO.EstaPagada = clienteDB.EstaPagada; 
            clienteDTO.FechaPago = clienteDB.FechaPago; 
            clienteDTO.FechaVencimiento = clienteDB.FechaVencimiento; 

            return Ok(clienteDTO);
        }

        // Guardar nuevo cliente
        [HttpPost]
        [Route("guardar")]
        public async Task<ActionResult<ClienteDTO>> Guardar(ClienteDTO clienteDTO)
        {
            try
            {
                var clienteDB = new Cliente
                {
                    NombreCompleto = clienteDTO.NombreCompleto,
                    IdPerfil = clienteDTO.IdPerfil,
                    FechaIngreso = clienteDTO.FechaIngreso,
                    Email = clienteDTO.Email,
                    ContactNro = clienteDTO.ContactNro,
                    EstaPagada = clienteDTO.EstaPagada, 
                    FechaPago = clienteDTO.FechaPago, 
                    FechaVencimiento = clienteDTO.FechaVencimiento
                };

                await _context.Clientes.AddAsync(clienteDB);
                await _context.SaveChangesAsync();

                return Ok("Cliente agregado");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al guardar el cliente: {ex.Message}");
            }
        }

        // Editar cliente
        [HttpPut]
        [Route("editar")]
        public async Task<ActionResult<ClienteDTO>> Editar(ClienteDTO clienteDTO)
        {
            var clienteDB = await _context.Clientes.Include(p => p.PerfilReferencia)
                .Where(e => e.IdCliente == clienteDTO.IdCliente).FirstOrDefaultAsync();

            if (clienteDB == null)
            {
                return NotFound("Cliente no encontrado");
            }

            clienteDB.NombreCompleto = clienteDTO.NombreCompleto;
            clienteDB.IdPerfil = clienteDTO.IdPerfil;
            clienteDB.EstaPagada = clienteDTO.EstaPagada; 
            clienteDB.FechaPago = clienteDTO.FechaPago; 
            clienteDB.FechaVencimiento = clienteDTO.FechaVencimiento;

            _context.Clientes.Update(clienteDB);
            await _context.SaveChangesAsync();

            return Ok("Cliente modificado");
        }

        // Eliminar cliente
        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var clienteDB = await _context.Clientes.FindAsync(id);

            if (clienteDB == null)
            {
                return NotFound("Cliente no encontrado");
            }

            _context.Clientes.Remove(clienteDB);
            await _context.SaveChangesAsync();

            return Ok("Cliente eliminado");
        }
    }
}