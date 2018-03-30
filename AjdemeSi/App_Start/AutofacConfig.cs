using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using AjdemeSi.Services;
using AjdemeSi.Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using AjdemeSi.Services.Interfaces.Identity;
using AjdemeSi.Services.Logic;
using AjdemeSi.Controllers;
using AjdemeSi.Domain.Models.Identity;

namespace AjdemeSi.App_Start
{
    public class AutofacConfig
    {
        public static void Configure()
        {
            var test = AppDomain.CurrentDomain.GetAssemblies().OrderBy(x => x.FullName);

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AjdemeSi.Services")).AsImplementedInterfaces();
            builder.RegisterType<DataContext>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register<IMapper>(c => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AspNetUser, IdentityUserViewModel>()
                   .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.AspNetRoles.Select(ur => ur.Name)));
                cfg.CreateMap<IdentityUserViewModel, AspNetUser>();

                //cfg.CreateMap<QuestionCategory, QuestionCategoryViewModel>()
                //                                                .ForMember(dest => dest.QuestionAnswerType, opt => opt.MapFrom(src => src.AnswerType))
                //                                                .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.QuestionCategory2.Name));
                //cfg.CreateMap<QuestionCategoryViewModel, QuestionCategory>();
            }).CreateMapper());

            var container = builder.Build();
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);
        }
    }
}