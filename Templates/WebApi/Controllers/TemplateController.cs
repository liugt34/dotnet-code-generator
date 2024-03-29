using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using {{Namespace1}}.Core;
using {{Namespace1}}.Core.Application.Interfaces;
using {{Namespace2}}.Application.Dtos;
using {{Namespace2}}.Application.Filters;
using {{Namespace2}}.Application.Interfaces;

namespace {{Namespace2}}.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class {{Template}}Controller : ApiController
    {
        private readonly I{{Template}}AppService _{{template}}AppService;
        private readonly ICrudAppService _crudAppService;

        public {{Template}}Controller(I{{Template}}AppService {{template}}AppService, ICrudAppService crudAppService)
        {
            _{{template}}AppService = {{template}}AppService;
            _crudAppService = crudAppService;
        }

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType<HttpResponseResult<{{Template}}Dto>>(200)]
        public async Task<IActionResult> Find([GuidBase64String] string id)
        {
            var {{template}} = await _{{template}}AppService.FindAsync(Guid.Parse(id.Base64Decode()));
            if ({{template}} == null)
            {
                AddError("{{ENTITY_NAME_CN}}不存在");
                return CustomResponse<HttpResponseResult<{{Template}}Dto>>();
            }

            return CustomResponse(HttpResponseResult<{{Template}}Dto>.Succeed({{template}}));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType<HttpResponsePagedResult<{{Template}}Dto>>(200)]
        public async Task<IActionResult> Query([FromBody] {{Template}}QueryFilter filter)
        {
            var results = await _{{template}}AppService.QueryPagedResultAsync(filter);
            return CustomResponse(HttpResponsePagedResult<{{Template}}Dto>.Succeed(results));
        }

        /// <summary>
        /// 新增接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType<HttpResponseResult>(200)]
        public async Task<IActionResult> Add([FromBody] {{Template}}AddInput input)
        {
            var result = await _crudAppService.Add(input);
            return CustomResponse(result);
        }

        /// <summary>
        /// 更新接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType<HttpResponseResult>(200)]
        public async Task<IActionResult> Update([FromBody] {{Template}}UpdateInput input)
        {
            var result = await _crudAppService.Update(input);
            return CustomResponse(result);
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType<HttpResponseResult>(200)]
        public async Task<IActionResult> Delete([GuidBase64String] string id)
        {
            var result = await _crudAppService.Remove(new {{Template}}RemoveInput(id));
            return CustomResponse(result);
        }
    }
}