using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazetyBack.Models;

namespace GazetyBack.Utils
{
    public class Crud<T> where T : class,IEntity
    {

        private DatabaseContext _ctx;
        private DbSet<T> Repos { get; set; }
        public Crud(DatabaseContext context)
        {
            _ctx = context;
            Repos = _ctx.Set<T>();
        }

        public DbSet<T> GetRepos() 
        {
            return Repos;
        }
        public async Task<List<T>> GetAll()
        {
            var data = await Repos.ToListAsync<T>();
            return data;
        }

        public async Task<List<T>> GetWithPage(int Page = 0, int NbrArticle = 25)
        {
            var data = await GetAll();
            data.GetRange((Page) * NbrArticle, NbrArticle);
            return data;
        }

        public async Task<T> GetOne(long Id)
        {
            try
            {
                var data = await Repos.SingleAsync<T>(v=>v.Id==Id);
                return data;            
            }
            catch (System.Exception)
            {
                return null;       
            }
        }

        public async Task<T> Insert(T item)
        {
            var new_item = await Repos.AddAsync(item);
            await _ctx.SaveChangesAsync();
            return new_item.Entity;
        }

        public async Task<Boolean> Delete(long Id)
        {
            try
            {
                var data = await GetOne(Id);
                Repos.Remove(data);
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }
}