using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdeaApp.Models;
using IdeaApp.Models.Repo.Base;

namespace IdeaApp.Controllers
{

    [Route("ideas")]
    [ApiController]
    public class IdeaController : Controller
    {

        IRepository<Idea> _repo=null;
        private readonly ILogger _logger;
        
        public IdeaController(ILogger<IdeaController> logger)
        {
            _repo=new MainRepository<Idea>(new IdeaDbContext());
            _logger=logger;
            
        }
        public ActionResult<IList<IdeaDTO>> Index(){

            _logger.LogInformation("Buen Perro");
            string getQueryString(string key){
                return this.Request.Query.Where(k => k.Key == key).Select(k => k.Value).FirstOrDefault();
            };
            
            var sortBy = getQueryString("sort_by") ?? "id";
            var pageIndex = int.Parse(getQueryString("page") ?? "1");
            var pageSize = int.Parse(getQueryString("page_size") ?? "10"); 
            var sortOrderAsc = (getQueryString("order") ?? "asc") =="asc"; 

            var res = _repo.GetByAnyPaging(k=>k.Id>0, k => k.Id, pageIndex, pageSize, sortOrderAsc);
            return res.Records.Select(e => new IdeaDTO(e)).ToList();

            // return new PagedListResult<IdeaDTO>{

            //   TotalRecords = res.TotalRecords,
            //   Records = 
            // };
        }

        [HttpGet("{id}")]
        public ActionResult<IdeaDTO> GetIdeaItem(int id){
            var obj = _repo.GetById(id);
            _logger.LogInformation($"{obj.Id}");
            // _logger.LogDebug($"{obj.Id}");

            if(obj==null){
                return NotFound();
            }
            return new IdeaDTO(obj);

        }

        [HttpPut("{id}")]
        public ActionResult<IdeaDTO> UpdateIdeaItem(int id,IdeaDTO ideaRecToUdp){
            if(id!=ideaRecToUdp.Id){
                return BadRequest();
            }

            try
            {
                var rec = _repo.GetById(id);
                rec.Confidence = ideaRecToUdp.confidence;
                rec.Content = ideaRecToUdp.content;
                rec.Ease = ideaRecToUdp.ease;
                rec.Impact = ideaRecToUdp.ease;
                rec.ModificationDate = DateTime.Now;
                _repo.Update(rec);

                return new IdeaDTO(rec);

            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true,VaryByHeader="Accept-Encoding, Origin")]
        public ActionResult<IdeaDTO> CreateIdeaItem(IdeaDTO ideacRecToCreate){
            try
            {
                var rec = new Idea();
                rec.Confidence = ideacRecToCreate.confidence;
                rec.Content = ideacRecToCreate.content;
                rec.Ease = ideacRecToCreate.ease;
                rec.Impact = ideacRecToCreate.ease;
                rec.CreationDate = DateTime.Now;
                rec=_repo.Add(rec);
                _logger.LogInformation($" about to send ==>{rec.Id}");
                return CreatedAtAction(nameof(GetIdeaItem), new { id = rec.Id }, new IdeaDTO(rec));

            }
            catch (System.Exception)
            {

                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<IdeaDTO> DeleteIdeaItem(int id)
        {
            try
            {
                var rec = _repo.GetById(id);
                _repo.Delete(rec);

                return null;

            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }


    }


}