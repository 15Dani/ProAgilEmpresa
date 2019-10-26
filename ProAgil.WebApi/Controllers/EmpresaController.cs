using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;
using ProAgil.WebApi.Dtos;

namespace ProAgil.WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly IProAgilRepository _repo;
        private readonly IMapper _mapper;
        public EmpresaController(IProAgilRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        //GET api/Empresa
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var empresas = await _repo.GetAllEmpresaAsync();

                var results = _mapper.Map<EmpresaDto[]>(empresas);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um falha ao acessar o banco de dados");
            }
        }

        // GET api/Empresa/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var empresa = await _repo.GetEmpresaAsyncById(id);
                var result = _mapper.Map<EmpresaDto>(empresa); 
                return Ok(result);
            }
            catch (System.Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro no banco de dados");
            }
        }

        [HttpGet("getByNome/{nome}")]
        public async Task<IActionResult> Get(string nome)
        {
            try
            {
                var empresas = await _repo.GetAllEmpresaAsyncByNome(nome);
                var result = _mapper.Map<IEnumerable<EmpresaDto>>(empresas); 
                return Ok(result);
            }
            catch (System.Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro no banco de dados");
            }
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage()
        {
            try{
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources","Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(),folderName);

                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullPath = Path.Combine(pathToSave,fileName);
                    using (var stream = new FileStream(fullPath,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                return Ok();
            } catch (System.Exception ex){
                return StatusCode(StatusCodes.Status500InternalServerError,$"Ocorreu um erro ao fazer upload da imagem {ex}");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(EmpresaDto model)
        {
            try
            {
                var empresa = _mapper.Map<Empresa>(model);
                _repo.Add(empresa);
                if (await _repo.SaveChangesAsync())
                {
                    if (empresa.ImagemURL != "")
                    {
                        empresa.ImagemURL = empresa.Id+".jpg";
                    }
                    
                    _repo.Update(empresa);
                    if (await _repo.SaveChangesAsync())
                    {   
                        return Created($"/api/empresa/{model.Id}", _mapper.Map<EmpresaDto>(empresa));
                    }
                }
            }
            catch (System.Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro no banco de dados");
            }

            return BadRequest();
        }

        [HttpPut("{empresaId}")]
        public async Task<IActionResult> Put(int empresaid, EmpresaDto model)
        {
            try
            {
                var empresa = await _repo.GetEmpresaAsyncById(empresaid);
                if (empresa == null) return NotFound();
                
       
                var idRedes = new List<int>();

   
                model.RedesSociais.ForEach(rede => idRedes.Add(rede.Id));
                
                var redesSociais = empresa.RedesSociais.Where( rede => !idRedes.Contains(rede.Id)).ToArray();

                if (redesSociais.Length > 0) _repo.DeleteRange(redesSociais);
                
                _mapper.Map(model,empresa);
                empresa.ImagemURL = $"{empresa.Id.ToString()}.jpg";
                _repo.Update(empresa);
                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/empresa/{empresa.Id}", _mapper.Map<EmpresaDto>(empresa));
                }
            }
            catch (System.Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro no banco de dados");
            }

            return BadRequest();
        }


        [HttpDelete("{empresaid}")]
        public async Task<IActionResult> Delete(int empresaid)
        {
            try
            {
                var empresaDB = await _repo.GetEmpresaAsyncById(empresaid);
                if (empresaDB == null) return NotFound();

                _repo.Delete(empresaDB);
                if (await _repo.SaveChangesAsync()) return Ok();
            }
            catch (System.Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro no banco de dados");
            }

            return BadRequest();
        }
    }

}