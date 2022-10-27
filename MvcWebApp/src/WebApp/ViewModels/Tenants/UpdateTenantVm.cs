using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApp.Mapping;

namespace WebApp.ViewModels.Tenants;

public class UpdateTenantVm
{
	public Guid Id { get; init; }

	[Required]
	[ModelBinder(BinderType = typeof(AntiXssBinder))]
	public string Name { get; set; }
}