using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public abstract class WebUiControllerBase : Controller
	{
		private ISender _mediator;
		private IMapper _mapper;

		protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
		protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
	}
}