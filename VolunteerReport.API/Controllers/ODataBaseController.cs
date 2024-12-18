﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VolunteerReport.Domain.Models.Interfaces;
using VolunteerReport.Domain;
using VolunteerReport.Domain.Utils;

namespace VolunteerReport.API.Controllers
{
    [AllowAnonymous]
    public class ODataControllerBase<T> : ODataController where T : class, IODataEntity
    {
        protected readonly ApplicationDbContext AppDbContext;
        protected readonly DbSet<T> CurrentDbSet;

        public ODataControllerBase(ApplicationDbContext appDbContext)
        {
            AppDbContext = appDbContext;
            CurrentDbSet = appDbContext.Set<T>();
        }

        [EnableQuery]
        public virtual IActionResult Get()
        {
            return Ok(CurrentDbSet);
        }

        [EnableQuery]
        public virtual async Task<IActionResult> Get(Guid key)
        {
            var item = await CurrentDbSet.FirstOrDefaultAsync(e => e.Id == key);
            return item is null ? NotFound() : (IActionResult)Ok(item);
        }

        [EnableQuery]
        public virtual async Task<IActionResult> Post([FromBody] T entity)
        {
            var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToArray();
            if (allErrors.Any()) return BadRequest(string.Join(", ", allErrors.Select(err => err.Exception)));
            CurrentDbSet.Add(entity);
            await AppDbContext.SaveChangesAsync();
            return Created(entity);
        }

        public virtual async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] T entity)
        {
            var dbEntity = await CurrentDbSet.FindAsync(key);
            if (dbEntity is null) return NotFound();
            entity.Id = key;
            AppDbContext.DetachLocal(entity, key);
            CurrentDbSet.Update(entity);
            await AppDbContext.SaveChangesAsync();
            return Updated(entity);
        }

        public virtual async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var entityFromStorage = await CurrentDbSet.FirstOrDefaultAsync(l => l.Id == key);
            if (entityFromStorage is null) return NotFound();
            CurrentDbSet.Remove(entityFromStorage);
            await AppDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}